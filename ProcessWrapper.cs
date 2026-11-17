// ProcessWrapper.cs
// ─────────────────────────────────────────────────────────────────────────────
// Abstract base class that manages a single child process (php.exe or mysqld.exe).
//
// This class handles the common mechanics of:
//   - Launching a process with captured stdout/stderr
//   - Receiving output lines asynchronously (without blocking the UI)
//   - Stopping the process gracefully (then forcefully if needed)
//   - Raising events so the UI can react to state changes
//
// C# concept — abstract classes:
//   An 'abstract' class cannot be instantiated directly. It defines a template
//   that subclasses must fill in. Abstract methods have no body here; each
//   subclass provides its own implementation. Compare to an interface, which
//   only declares method signatures; an abstract class can also contain
//   fully-implemented ("concrete") methods that subclasses inherit.
//
// C# concept — events:
//   Events are the standard C# publish/subscribe mechanism.
//   A class declares 'public event SomeDelegate MyEvent;'
//   Other objects subscribe: 'instance.MyEvent += MyHandler;'
//   The class raises it: 'MyEvent?.Invoke(args);'
//   The '?' (null-conditional) means: only invoke if anyone has subscribed.
//
// C# concept — EventHandler<T>:
//   The standard delegate for events in .NET. First parameter is always
//   'object sender' (who raised the event), second is a T carrying the data.
// ─────────────────────────────────────────────────────────────────────────────

using System.Diagnostics;

namespace WINSERV;

/// <summary>
/// Data carried by the OutputReceived event: one line of text from the process.
/// </summary>
public class OutputEventArgs : EventArgs
{
    // C# concept — primary constructor shorthand for record/EventArgs:
    // We use a plain constructor to set the properties clearly.

    public string Line { get; }
    public bool IsError { get; }  // true if the line came from stderr

    public OutputEventArgs(string line, bool isError)
    {
        Line = line;
        IsError = isError;
    }
}

/// <summary>
/// Abstract base class managing a single long-running child process.
/// Subclasses (PhpServer, MariaDbServer) override the abstract members
/// to supply the correct executable path and command-line arguments.
/// </summary>
public abstract class ProcessWrapper
{
    // ── Fields ───────────────────────────────────────────────────────────────

    // The System.Diagnostics.Process object representing the child process.
    // Nullable (Process?) because it doesn't exist until Start() is called.
    private Process? _process;

    // Thread-safe lock object. We use 'lock (_lock)' to prevent race conditions
    // when Start/Stop are called from different threads simultaneously.
    //
    // C# concept — lock statement:
    //   lock(obj) { ... } acquires a mutual-exclusion lock on 'obj' for the
    //   duration of the block. Any other thread trying to lock the same object
    //   will wait. Use a dedicated private object (not 'this') to avoid
    //   accidental deadlocks.
    private readonly object _lock = new();

    // ── Events ───────────────────────────────────────────────────────────────

    /// <summary>Raised whenever the child process writes a line to stdout or stderr.</summary>
    public event EventHandler<OutputEventArgs>? OutputReceived;

    /// <summary>Raised when the process starts successfully.</summary>
    public event EventHandler? Started;

    /// <summary>Raised when the process exits (for any reason).</summary>
    public event EventHandler? Stopped;

    // ── Properties ───────────────────────────────────────────────────────────

    /// <summary>Human-readable name for this service (e.g. "PHP", "MariaDB").</summary>
    public abstract string ServiceName { get; }

    /// <summary>Full path to the executable to launch.</summary>
    protected abstract string ExecutablePath { get; }

    /// <summary>Command-line arguments to pass to the executable.</summary>
    protected abstract string BuildArguments();

    /// <summary>
    /// Optional: working directory for the process.
    /// Defaults to the directory containing the executable.
    /// Override in subclasses if needed.
    /// </summary>
    protected virtual string WorkingDirectory =>
        Path.GetDirectoryName(ExecutablePath) ?? AppContext.BaseDirectory;

    /// <summary>Returns true if the child process is currently running.</summary>
    public bool IsRunning
    {
        get
        {
            // Lock because _process can be set/cleared from background threads.
            lock (_lock)
            {
                // _process is null before Start() or after Stop().
                // HasExited checks whether the OS process has terminated.
                return _process != null && !_process.HasExited;
            }
        }
    }

    // ── Public Methods ───────────────────────────────────────────────────────

    /// <summary>
    /// Launches the child process. Does nothing if already running.
    /// Throws if the executable does not exist.
    /// </summary>
    public void Start()
    {
        lock (_lock)
        {
            if (IsRunning)
                return;

            if (!File.Exists(ExecutablePath))
                throw new FileNotFoundException(
                    $"{ServiceName}: executable not found at {ExecutablePath}. " +
                    "Has the runtime been extracted?",
                    ExecutablePath
                );

            // ProcessStartInfo configures HOW to launch the process.
            var startInfo = new ProcessStartInfo
            {
                FileName               = ExecutablePath,
                Arguments              = BuildArguments(),
                WorkingDirectory       = WorkingDirectory,

                // Redirect stdout and stderr so we can read them in our app.
                // Without redirection, the output goes to a console window
                // (or nowhere, for a WinExe app).
                RedirectStandardOutput = true,
                RedirectStandardError  = true,

                // UseShellExecute must be false when redirecting streams.
                // 'true' would use the Windows shell to open the file (like
                // double-clicking), which doesn't support stream redirection.
                UseShellExecute        = false,

                // Don't create a visible console window for the child process.
                CreateNoWindow         = true,
            };

            _process = new Process { StartInfo = startInfo };

            // EnableRaisingEvents = true means the Process object will fire
            // the 'Exited' event when the process terminates. Without this,
            // you would have to poll HasExited manually.
            _process.EnableRaisingEvents = true;

            // Subscribe to output events. These fire on a ThreadPool thread
            // (NOT the UI thread), so the UI must use Invoke() to update controls.
            // The lambda '(s, e) => ...' is an anonymous method subscribed inline.
            _process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    OnOutputReceived(e.Data, isError: false);
            };

            _process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    OnOutputReceived(e.Data, isError: true);
            };

            // When the process exits unexpectedly, raise our Stopped event.
            _process.Exited += (s, e) =>
            {
                lock (_lock) { _process = null; }
                Stopped?.Invoke(this, EventArgs.Empty);
                OnOutputReceived($"[{ServiceName}] Process exited.", isError: false);
            };

            // Start() launches the process. Returns false if it fails to start.
            bool started = _process.Start();
            if (!started)
                throw new InvalidOperationException($"Failed to start {ServiceName}.");

            // Begin asynchronous reading of stdout and stderr.
            // This starts background threads that call our event handlers above.
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        // Raise the Started event OUTSIDE the lock to avoid potential deadlocks
        // if event handlers try to call back into this object.
        Started?.Invoke(this, EventArgs.Empty);
        OnOutputReceived($"[{ServiceName}] Started (PID {_process?.Id}).", isError: false);
    }

    /// <summary>
    /// Stops the child process.
    /// Tries a graceful shutdown first; force-kills after a timeout.
    /// </summary>
    /// <param name="gracefulTimeoutMs">
    /// How long to wait for the process to exit cleanly before killing it.
    /// Default is 5 seconds.
    /// </param>
    public void Stop(int gracefulTimeoutMs = 5000)
    {
        Process? processToStop;

        lock (_lock)
        {
            if (_process == null || _process.HasExited)
                return;

            processToStop = _process;
            _process = null;  // Clear reference so IsRunning returns false immediately
        }

        OnOutputReceived($"[{ServiceName}] Stopping...", isError: false);

        try
        {
            // Ask the process to exit by closing its stdin stream (some CLI apps
            // respond to this). For most server processes this has no effect,
            // so we also send CloseMainWindow() and then Kill().
            processToStop.CloseMainWindow();

            // Wait up to gracefulTimeoutMs for the process to exit on its own.
            // WaitForExit returns true if the process exited within the timeout.
            bool exited = processToStop.WaitForExit(gracefulTimeoutMs);

            if (!exited)
            {
                // Process didn't exit cleanly — force terminate it.
                OnOutputReceived($"[{ServiceName}] Graceful stop timed out, killing process.", isError: true);
                processToStop.Kill();
                // .NET Framework does not have Kill(entireProcessTree).
                // mysqld.exe and php.exe don't normally spawn children,
                // but this is good practice.
            }
        }
        catch (Exception ex)
        {
            // The process may have already exited between our check and Kill(),
            // which throws InvalidOperationException. Catch broadly and log.
            OnOutputReceived($"[{ServiceName}] Stop error: {ex.Message}", isError: true);
        }
        finally
        {
            processToStop.Dispose();  // Release OS handles
        }

        Stopped?.Invoke(this, EventArgs.Empty);
        OnOutputReceived($"[{ServiceName}] Stopped.", isError: false);
    }

    // ── Protected Helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Raises the OutputReceived event. Called by this class and by subclasses
    /// that want to inject log messages (e.g. during initialisation).
    /// </summary>
    protected void OnOutputReceived(string line, bool isError)
    {
        // C# concept — ?. (null-conditional invoke):
        //   'event?.Invoke(...)' only calls Invoke if there is at least one subscriber.
        //   Without this check, raising an event with no subscribers throws
        //   a NullReferenceException.
        OutputReceived?.Invoke(this, new OutputEventArgs(line, isError));
    }
}
