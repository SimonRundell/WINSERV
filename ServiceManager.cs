// ServiceManager.cs
// ─────────────────────────────────────────────────────────────────────────────
// Orchestrates the PhpServer and MariaDbServer: start, stop, and status.
//
// The MainForm (UI) talks to ServiceManager rather than directly to
// PhpServer or MariaDbServer. This separation means the UI doesn't need
// to know the details of how each service works — it just calls
// StartAll(), StopAll(), etc.
//
// C# concept — async/await:
//   Methods marked 'async' can contain 'await' expressions that pause
//   execution without blocking the calling thread. This is essential for
//   keeping the UI responsive during long operations (like initialising a
//   database, which takes several seconds).
//
//   Task          = a unit of async work with no return value (like void)
//   Task<T>       = a unit of async work that returns a value of type T
//   await someTask = "pause here and resume when someTask completes,
//                    but release the thread in the meantime"
//
//   Rule of thumb: any method that touches the network, file system, or
//   launches a process should be async so it doesn't freeze the UI.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>
/// Provides start/stop/status operations for the PHP and MariaDB services
/// as a coordinated pair. Aggregates their log output into a single stream.
/// </summary>
public class ServiceManager
{
    // ── Services ─────────────────────────────────────────────────────────────

    /// <summary>The PHP built-in web server.</summary>
    public PhpServer Php { get; }

    /// <summary>The MariaDB database server.</summary>
    public MariaDbServer MariaDb { get; }

    // ── Events ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Raised when any service produces log output.
    /// Subscribe here in the UI to receive all log lines from both services.
    /// </summary>
    public event EventHandler<OutputEventArgs>? LogOutput;

    /// <summary>
    /// Raised when either service changes state (started or stopped).
    /// The UI subscribes to this to update its status indicators.
    /// </summary>
    public event EventHandler? StatusChanged;

    // ── Constructor ──────────────────────────────────────────────────────────

    public ServiceManager(AppConfig config)
    {
        Php     = new PhpServer(config);
        MariaDb = new MariaDbServer(config);

        // Subscribe to each service's events and forward them through our own events.
        // This means the UI only needs to subscribe to ServiceManager, not to each
        // service individually.

        // C# concept — '+=' for events:
        //   Subscribing to an event adds a handler to its invocation list.
        //   Multiple handlers can subscribe; all are called when the event fires.
        //   We use a method group here: 'ForwardLog' is the method to call.

        Php.OutputReceived     += ForwardLog;
        Php.Started            += (s, e) => StatusChanged?.Invoke(this, EventArgs.Empty);
        Php.Stopped            += (s, e) => StatusChanged?.Invoke(this, EventArgs.Empty);

        MariaDb.OutputReceived += ForwardLog;
        MariaDb.Started        += (s, e) => StatusChanged?.Invoke(this, EventArgs.Empty);
        MariaDb.Stopped        += (s, e) => StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Starts both services. MariaDB is started first because PHP applications
    /// typically connect to the database on startup; starting PHP first could
    /// lead to connection errors in the first few requests.
    ///
    /// Long-running initialisation (first-run DB setup) is handled asynchronously
    /// on a background thread so the UI remains responsive.
    /// </summary>
    public async Task StartAllAsync()
    {
        // Task.Run() executes the lambda on a ThreadPool background thread.
        // 'await' pauses this method until the background work finishes,
        // but does NOT block the UI thread — WinForms can still repaint etc.
        // Cap overall MariaDB init to 5 minutes. Individual steps (mysqld
        // --initialize, WaitForPort, etc.) have their own shorter timeouts,
        // but this outer guard prevents an indefinite hang between steps.
        using var initCts = new System.Threading.CancellationTokenSource(
            TimeSpan.FromMinutes(5));

        try
        {
            await Task.Run(() =>
            {
                // Write my.ini each time so config changes take effect.
                MariaDb.WriteMyIni();

                // First-run: initialise the data directory if needed.
                if (!MariaDb.IsDataDirectoryInitialised())
                {
                    bool ok = MariaDb.InitialiseDataDirectory();
                    if (!ok)
                    {
                        EmitLog("[WINSERV] MariaDB initialisation failed. Database will not start.", isError: true);
                        return;
                    }
                }

                MariaDb.Start();
            }, initCts.Token);
        }
        catch (OperationCanceledException)
        {
            EmitLog("[WINSERV] MariaDB initialisation timed out after 5 minutes.", isError: true);
            return;
        }
        catch (Exception ex)
        {
            EmitLog($"[WINSERV] MariaDB start error: {ex.Message}", isError: true);
        }

        // Short pause to allow MariaDB to begin accepting connections before PHP starts.
        // PHP applications often connect to the database at startup, so we want
        // the database to be ready first.
        await Task.Delay(1500);

        await Task.Run(() =>
        {
            try
            {
                Php.EnsurePhpIni();
                Php.EnsureRouterPhp();
                Php.Start();
            }
            catch (Exception ex)
            {
                EmitLog($"[WINSERV] PHP start error: {ex.Message}", isError: true);
            }
        });
    }

    /// <summary>
    /// Stops both services. PHP is stopped first (no new requests accepted),
    /// then MariaDB (database shut down cleanly).
    /// </summary>
    public async Task StopAllAsync()
    {
        await Task.Run(() =>
        {
            try { Php.Stop(); }
            catch (Exception ex) { EmitLog($"[WINSERV] PHP stop error: {ex.Message}", isError: true); }
        });

        await Task.Run(() =>
        {
            try { MariaDb.Stop(); }
            catch (Exception ex) { EmitLog($"[WINSERV] MariaDB stop error: {ex.Message}", isError: true); }
        });
    }

    /// <summary>Starts only the PHP server.</summary>
    public async Task StartPhpAsync()
    {
        await Task.Run(() =>
        {
            try { Php.EnsurePhpIni(); Php.EnsureRouterPhp(); Php.Start(); }
            catch (Exception ex) { EmitLog($"[WINSERV] PHP start error: {ex.Message}", isError: true); }
        });
    }

    /// <summary>Starts only the MariaDB server.</summary>
    public async Task StartMariaDbAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                MariaDb.WriteMyIni();
                if (!MariaDb.IsDataDirectoryInitialised())
                    MariaDb.InitialiseDataDirectory();
                MariaDb.Start();
            }
            catch (Exception ex) { EmitLog($"[WINSERV] MariaDB start error: {ex.Message}", isError: true); }
        });
    }

    /// <summary>Stops only the PHP server.</summary>
    public async Task StopPhpAsync()
    {
        await Task.Run(() =>
        {
            try { Php.Stop(); }
            catch (Exception ex) { EmitLog($"[WINSERV] PHP stop error: {ex.Message}", isError: true); }
        });
    }

    /// <summary>Stops only the MariaDB server.</summary>
    public async Task StopMariaDbAsync()
    {
        await Task.Run(() =>
        {
            try { MariaDb.Stop(); }
            catch (Exception ex) { EmitLog($"[WINSERV] MariaDB stop error: {ex.Message}", isError: true); }
        });
    }

    // ── Private Helpers ──────────────────────────────────────────────────────

    // EventHandler<OutputEventArgs> matches the signature expected by
    // ProcessWrapper's OutputReceived event, so we can use this method
    // directly as the event handler.
    private void ForwardLog(object? sender, OutputEventArgs e)
    {
        LogOutput?.Invoke(sender, e);
    }

    private void EmitLog(string message, bool isError)
    {
        LogOutput?.Invoke(this, new OutputEventArgs(message, isError));
    }
}
