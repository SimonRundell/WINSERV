// MainForm.cs
// ─────────────────────────────────────────────────────────────────────────────
// Logic and event handlers for the WINSERV control panel.
// Control layout lives in MainForm.Designer.cs — open that file in the
// VS Form Designer by double-clicking MainForm.cs in Solution Explorer.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>The WINSERV main control panel window.</summary>
public partial class MainForm : Form
{
    // ── Non-control fields ────────────────────────────────────────────────────
    private readonly ServiceManager _services;
    private AppConfig _config;

    // ── Constructor ──────────────────────────────────────────────────────────

    /// <summary>Parameterless constructor for the WinForms designer only.</summary>
    public MainForm() : this(new AppConfig(), new ServiceManager(new AppConfig())) { }

    public MainForm(AppConfig config, ServiceManager services)
    {
        _config   = config;
        _services = services;

        InitializeComponent();

        // Load icon from embedded resource — done here rather than in the
        // designer file because the designer cannot parse Assembly calls.
        var iconStream = System.Reflection.Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("WINSERV.elephant.ico");
        if (iconStream != null)
            Icon = new Icon(iconStream);

        _services.LogOutput     += (s, e) => SafeInvoke(() => AppendLog(e.Line, e.IsError));
        _services.StatusChanged += (s, e) => SafeInvoke(UpdateStatusIndicators);

        UpdateStatusIndicators();

        if (_config.AutoStartServices)
            Load += async (s, e) => await _services.StartAllAsync();
    }

    // ── Status Indicators ────────────────────────────────────────────────────

    private void UpdateStatusIndicators()
    {
        bool phpRunning = _services.Php.IsRunning;
        _lblPhpDot.ForeColor = phpRunning ? Color.LimeGreen : Color.Gray;
        _lblPhpStatus.Text   = phpRunning
            ? $"Running  —  http://localhost:{_config.HttpPort}"
            : "Stopped";
        _btnPhpStart.Enabled = !phpRunning;
        _btnPhpStop.Enabled  = phpRunning;

        bool dbRunning = _services.MariaDb.IsRunning;
        _lblDbDot.ForeColor  = dbRunning ? Color.LimeGreen : Color.Gray;
        _lblDbStatus.Text    = dbRunning
            ? $"Running  —  localhost:{_config.MariaDbPort}  (root / {MaskPassword(_config.MariaDbRootPassword)})"
            : "Stopped";
        _btnDbStart.Enabled = !dbRunning;
        _btnDbStop.Enabled  = dbRunning;

        bool anyRunning = phpRunning || dbRunning;
        bool allRunning = phpRunning && dbRunning;
        _btnStartAll.Enabled = !allRunning;
        _btnStopAll.Enabled  = anyRunning;
    }

    // ── Log Output ────────────────────────────────────────────────────────────

    private void AppendLog(string message, bool isError)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string line      = $"[{timestamp}] {message}";

        _logBox.SelectionStart  = _logBox.TextLength;
        _logBox.SelectionLength = 0;
        _logBox.SelectionColor  = isError ? Color.Tomato : Color.White;
        _logBox.AppendText(line + "\n");
        _logBox.ScrollToCaret();

        const int maxLines = 2000;
        if (_logBox.Lines.Length > maxLines)
        {
            int removeUpTo = _logBox.GetFirstCharIndexFromLine(
                _logBox.Lines.Length - maxLines);
            _logBox.Select(0, removeUpTo);
            _logBox.SelectedText = "";
        }
    }

    // ── Button Handlers ───────────────────────────────────────────────────────

    private async void BtnPhpStart_Click(object? sender, EventArgs e)   => await _services.StartPhpAsync();
    private async void BtnPhpStop_Click(object? sender, EventArgs e)    => await _services.StopPhpAsync();
    private async void BtnDbStart_Click(object? sender, EventArgs e)    => await _services.StartMariaDbAsync();
    private async void BtnDbStop_Click(object? sender, EventArgs e)     => await _services.StopMariaDbAsync();
    private async void BtnStartAll_Click(object? sender, EventArgs e)   => await _services.StartAllAsync();
    private async void BtnStopAll_Click(object? sender, EventArgs e)    => await _services.StopAllAsync();

    private void BtnClearLog_Click(object? sender, EventArgs e) => _logBox.Clear();
    private void BtnHide_Click(object? sender, EventArgs e)     => Hide();

    private void BtnOpenWebRoot_Click(object? sender, EventArgs e)
    {
        try
        {
            Directory.CreateDirectory(_config.WebRoot);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = _config.WebRoot,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open WebRoot folder:\n{ex.Message}",
                "WINSERV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnOpenBrowser_Click(object? sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = $"http://localhost:{_config.HttpPort}",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open browser:\n{ex.Message}",
                "WINSERV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        using var dlg = new SettingsForm(_config);

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            ConfigManager.Save(_config);
            UpdateStatusIndicators();
            AppendLog("[WINSERV] Settings saved. Restart services for changes to take effect.", false);
        }
    }

    // ── Form Behaviour ────────────────────────────────────────────────────────

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            return;
        }
        base.OnFormClosing(e);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SafeInvoke(Action action)
    {
        if (IsDisposed) return;
        if (InvokeRequired) BeginInvoke(action);
        else action();
    }

    private static string MaskPassword(string pwd)
    {
        if (string.IsNullOrEmpty(pwd)) return "(empty)";
        return pwd[0] + new string('●', Math.Max(0, pwd.Length - 1));
    }
}
