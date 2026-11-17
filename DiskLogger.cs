// DiskLogger.cs
// ─────────────────────────────────────────────────────────────────────────────
// Writes all service log output to a daily rotating log file on disk.
//
// Subscribes to ServiceManager.LogOutput and appends each line to
// {LogDirectory}\winserv_YYYY-MM-DD.log. A new file is started each day
// automatically — no manual log rotation needed.
//
// All file writes are fire-and-forget: IO errors are swallowed so a full
// disk or permission problem never crashes the application.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>
/// Appends timestamped service output to a daily log file in LogDirectory.
/// </summary>
public sealed class DiskLogger : IDisposable
{
    private readonly AppConfig _config;
    private readonly object    _lock = new();

    // Track which date the current log file belongs to so we rotate at midnight.
    private DateTime _currentDate = DateTime.MinValue;
    private string   _currentPath = string.Empty;

    public DiskLogger(AppConfig config, ServiceManager services)
    {
        _config = config;
        services.LogOutput += OnLogOutput;
    }

    // ── Event Handler ────────────────────────────────────────────────────────

    private void OnLogOutput(object? sender, OutputEventArgs e)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string prefix    = e.IsError ? "ERROR" : "INFO ";
            string line      = $"[{timestamp}] [{prefix}] {e.Line}";

            lock (_lock)
            {
                EnsureLogFile();
                File.AppendAllText(_currentPath, line + Environment.NewLine);
            }
        }
        catch
        {
            // Never let a logging failure propagate to the UI thread.
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates or rotates the log file when the date has changed.
    /// Must be called inside _lock.
    /// </summary>
    private void EnsureLogFile()
    {
        DateTime today = DateTime.Now.Date;

        if (today == _currentDate && !string.IsNullOrEmpty(_currentPath))
            return;

        _currentDate = today;

        // LogDirectory may not exist yet (e.g. first launch before services start).
        string logDir = _config.LogDirectory;
        Directory.CreateDirectory(logDir);

        _currentPath = Path.Combine(logDir, $"winserv_{today:yyyy-MM-dd}.log");

        // Write a session-start marker so restarts are visible in the file.
        File.AppendAllText(_currentPath,
            $"{Environment.NewLine}=== WINSERV session started {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
    }

    // ── IDisposable ──────────────────────────────────────────────────────────

    public void Dispose()
    {
        // Nothing to dispose — File.AppendAllText opens and closes per write.
        // IDisposable is here so callers can use 'using' if they choose.
    }
}
