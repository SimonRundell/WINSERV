// Program.cs
// ─────────────────────────────────────────────────────────────────────────────
// Application entry point.
//
// This file is intentionally minimal. All startup logic lives in
// TrayApplicationContext, keeping Program.cs easy to understand at a glance.
//
// C# concept — top-level statements (C# 9+):
//   In modern C# you no longer need to write 'class Program { static void Main() }'.
//   You can write statements directly at the top of a file and the compiler
//   wraps them in a Main() method automatically. We use the traditional style
//   here (explicit class + Main) to make the structure visible for learning.
//
// C# concept — [STAThread]:
//   WinForms requires the main thread to be in Single-Threaded Apartment (STA)
//   mode, which is a COM threading model. This attribute instructs the .NET
//   runtime to initialise COM in STA mode for this thread. If you omit it,
//   some WinForms features (like drag-and-drop and the clipboard) will fail.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // ── Single-instance guard ─────────────────────────────────────────────
        //
        // A named Mutex is visible across all processes on the same desktop
        // session. If another instance already owns it, we surface a message
        // and exit immediately — this also prevents MSBuild from hitting the
        // "file locked by WINSERV" error because the old instance must be
        // closed before a new build can run.
        //
        // The GUID in the name makes it unique to this application so it
        // cannot collide with any other program's mutex.
        using var mutex = new System.Threading.Mutex(
            initiallyOwned: true,
            name: "Global\\WINSERV-{A3F2C1B0-4D7E-4A9F-8E3C-1B2D5F6A7E8C}",
            out bool createdNew
        );

        if (!createdNew)
        {
            MessageBox.Show(
                "WINSERV is already running.\n\nCheck the system tray.",
                "WINSERV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return;
        }

        // ── Global exception handling ─────────────────────────────────────────
        //
        // These handlers catch unhandled exceptions that slip through normal
        // try/catch blocks. Without them, the app would silently crash.
        // We log a message box and exit gracefully.
        //
        // ThreadException fires for unhandled exceptions on the UI thread.
        Application.ThreadException += (sender, e) =>
        {
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\n" +
                $"WINSERV will attempt to continue, but you may want to restart it.\n\n" +
                $"Details:\n{e.Exception}",
                "WINSERV — Unexpected Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        };

        // UnhandledException fires for unhandled exceptions on background threads.
        // isTerminating=true means the runtime is about to abort the process.
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            string message = e.ExceptionObject is Exception ex
                ? ex.ToString()
                : e.ExceptionObject?.ToString() ?? "Unknown error";

            MessageBox.Show(
                $"A fatal error has occurred and WINSERV must close.\n\n{message}",
                "WINSERV — Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        };

        // ── Visual styles and text rendering ──────────────────────────────────
        //
        // EnableVisualStyles() applies the current Windows visual theme to
        // WinForms controls (rounded buttons, themed checkboxes, etc.).
        // Without it, controls look like Windows 95.
        Application.EnableVisualStyles();

        // SetCompatibleTextRenderingDefault(false) uses GDI+ for text rendering
        // rather than the older GDI method. 'false' is the recommended modern setting.
        Application.SetCompatibleTextRenderingDefault(false);

        // ── High-DPI support ──────────────────────────────────────────────────
        // DPI awareness is declared in app.manifest (dpiAware/dpiAwareness).
        // SetHighDpiMode() is a .NET 5+ API and is not available on .NET Framework.

        // ── Start the application ─────────────────────────────────────────────
        //
        // Application.Run(context) starts the Windows message loop.
        // The loop processes mouse clicks, keyboard events, paint messages etc.
        // It runs until TrayApplicationContext calls ExitThread().
        Application.Run(new TrayApplicationContext());
    }
}
