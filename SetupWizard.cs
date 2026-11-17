// SetupWizard.cs
// ─────────────────────────────────────────────────────────────────────────────
// Logic and event handlers for the first-run setup wizard.
// Control layout lives in SetupWizard.Designer.cs.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>
/// First-run setup wizard. Guides the user through choosing an install path
/// and extracts the runtime.
/// </summary>
public partial class SetupWizard : Form
{
    private readonly AppConfig _config;

    /// <summary>Parameterless constructor for the WinForms designer only.</summary>
    public SetupWizard() : this(new AppConfig()) { }

    public SetupWizard(AppConfig config)
    {
        _config = config;

        InitializeComponent();

        // Populate fields from config.
        _txtInstallPath.Text = config.InstallPath;
        _txtWebRoot.Text     = config.WebRoot;
        _txtHttpPort.Text    = config.HttpPort.ToString();
        _txtDbPassword.Text  = config.MariaDbRootPassword;

        // Runtime zip check — dynamic so must happen after InitializeComponent.
        bool zipExists = RuntimeInstaller.IsRuntimeZipPresent();
        _lblZipStatus.Text      = zipExists
            ? $"✓ Runtime zip found: {RuntimeInstaller.RuntimeZipName}"
            : $"✗ Runtime zip NOT found. Place {RuntimeInstaller.RuntimeZipName} next to WINSERV.exe.";
        _lblZipStatus.ForeColor = zipExists ? Color.Green : Color.Red;
        _btnInstall.Enabled     = zipExists;
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description         = "Choose the WINSERV install folder (must be user-writable).",
            ShowNewFolderButton = true,
            SelectedPath        = _txtInstallPath.Text
        };

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            string chosen = dlg.SelectedPath;
            _txtInstallPath.Text = chosen;

            // Auto-fill webroot if it still looks like the default.
            if (_txtWebRoot.Text == _config.WebRoot ||
                _txtWebRoot.Text.StartsWith(_config.InstallPath))
            {
                _txtWebRoot.Text = Path.Combine(chosen, "webroot");
            }
        }
    }

    private async void BtnInstall_Click(object? sender, EventArgs e)
    {
        // ── Validate inputs ───────────────────────────────────────────────────
        if (!int.TryParse(_txtHttpPort.Text, out int port) || port <= 1024 || port > 65535)
        {
            MessageBox.Show("HTTP port must be a number between 1025 and 65535.",
                "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_txtInstallPath.Text))
        {
            MessageBox.Show("Please specify an install path.",
                "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // ── Apply config ──────────────────────────────────────────────────────
        _config.InstallPath         = _txtInstallPath.Text.Trim();
        _config.WebRoot             = _txtWebRoot.Text.Trim();
        _config.HttpPort            = port;
        _config.MariaDbRootPassword = _txtDbPassword.Text;
        _config.DataDirectory       = Path.Combine(_config.InstallPath, "data");
        _config.LogDirectory        = Path.Combine(_config.InstallPath, "logs");

        // ── Disable UI during install ─────────────────────────────────────────
        _btnInstall.Enabled = false;
        _btnBrowse.Enabled  = false;
        _btnClose.Enabled   = false;

        AppendLog("Starting installation...");

        var progressReporter = new Progress<int>(pct => _progressBar.Value = Math.Min(pct, 100));
        var logReporter      = new Progress<string>(AppendLog);

        bool ok = await RuntimeInstaller.ExtractAsync(_config, progressReporter, logReporter);

        if (!ok)
        {
            AppendLog("Installation failed. See messages above.");
            _btnInstall.Enabled = true;
            _btnBrowse.Enabled  = true;
            _btnClose.Enabled   = true;
            return;
        }

        // ── Save config ───────────────────────────────────────────────────────
        _config.SetupComplete = true;
        ConfigManager.Save(_config);
        AppendLog($"Configuration saved to {ConfigManager.GetConfigFilePath()}");

        AppendLog("");
        AppendLog("Setup complete! Click Close to launch the WINSERV control panel.");
        _btnClose.Enabled = true;

        DialogResult = DialogResult.OK;
    }

    private void BtnClose_Click(object? sender, EventArgs e) => Close();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void AppendLog(string message)
    {
        if (InvokeRequired) { Invoke(() => AppendLog(message)); return; }
        _logBox.AppendText(message + "\n");
        _logBox.ScrollToCaret();
    }
}
