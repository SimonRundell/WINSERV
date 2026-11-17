// SettingsForm.cs
// ─────────────────────────────────────────────────────────────────────────────
// Logic for the WINSERV settings dialog.
// Control layout lives in SettingsForm.Designer.cs.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>
/// Settings dialog for editing WINSERV configuration.
/// Changes only take effect after restarting the services.
/// </summary>
public partial class SettingsForm : Form
{
    private readonly AppConfig _config;

    /// <summary>Parameterless constructor for the WinForms designer only.</summary>
    public SettingsForm() : this(new AppConfig()) { }

    public SettingsForm(AppConfig config)
    {
        _config = config;

        InitializeComponent();

        // Populate controls from config after the designer has created them.
        _txtInstallPath.Text    = config.InstallPath;
        _txtWebRoot.Text        = config.WebRoot;
        _txtDataDir.Text        = config.DataDirectory;
        _txtLogDir.Text         = config.LogDirectory;
        _nudHttpPort.Value      = config.HttpPort;
        _nudDbPort.Value        = config.MariaDbPort;
        _txtDbPassword.Text     = config.MariaDbRootPassword;
        _chkAutoStart.Checked   = config.AutoStartServices;
        _chkShowWindow.Checked  = config.ShowWindowOnStartup;
        _lblConfigPath.Text     = $"Config file: {ConfigManager.GetConfigFilePath()}";
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        // ── Validation ────────────────────────────────────────────────────────
        var errors = new System.Text.StringBuilder();

        string installPath = _txtInstallPath.Text.Trim();
        string webRoot     = _txtWebRoot.Text.Trim();
        string dataDir     = _txtDataDir.Text.Trim();
        string logDir      = _txtLogDir.Text.Trim();
        string password    = _txtDbPassword.Text;

        if (string.IsNullOrEmpty(installPath)) errors.AppendLine("• Install Path must not be empty.");
        if (string.IsNullOrEmpty(webRoot))     errors.AppendLine("• Web Root must not be empty.");
        if (string.IsNullOrEmpty(dataDir))     errors.AppendLine("• DB Data Directory must not be empty.");
        if (string.IsNullOrEmpty(logDir))      errors.AppendLine("• Log Directory must not be empty.");
        if (string.IsNullOrEmpty(password))    errors.AppendLine("• DB Root Password must not be empty.");

        if (_nudHttpPort.Value == _nudDbPort.Value)
            errors.AppendLine("• HTTP Port and MariaDB Port must be different.");

        if (errors.Length > 0)
        {
            MessageBox.Show(
                "Please fix the following before saving:\n\n" + errors,
                "WINSERV — Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        // ── Apply ─────────────────────────────────────────────────────────────
        _config.InstallPath         = installPath;
        _config.WebRoot             = webRoot;
        _config.DataDirectory       = dataDir;
        _config.LogDirectory        = logDir;
        _config.HttpPort            = (int)_nudHttpPort.Value;
        _config.MariaDbPort         = (int)_nudDbPort.Value;
        _config.MariaDbRootPassword = password;
        _config.AutoStartServices   = _chkAutoStart.Checked;
        _config.ShowWindowOnStartup = _chkShowWindow.Checked;

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>
    /// Deploys (or re-checks) the vendored Adminer build into
    /// {InstallPath}\tools\adminer\adminer.php. Useful when adminer.php was
    /// added to the deployment package, or dropped next to WINSERV.exe,
    /// after the initial setup wizard already ran — see RuntimeInstaller.DeployAdminer().
    /// </summary>
    private void BtnDeployAdminer_Click(object? sender, EventArgs e)
    {
        var result = RuntimeInstaller.DeployAdminer(_config);

        switch (result)
        {
            case RuntimeInstaller.AdminerDeployResult.AlreadyPresent:
                MessageBox.Show(
                    $"Adminer is already deployed at:\n{_config.AdminerPhpPath}",
                    "WINSERV — Deploy Adminer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;

            case RuntimeInstaller.AdminerDeployResult.Deployed:
                MessageBox.Show(
                    $"Adminer deployed to:\n{_config.AdminerPhpPath}\n\n" +
                    "If PHP is currently running, restart it once so the router script picks it up.",
                    "WINSERV — Deploy Adminer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;

            case RuntimeInstaller.AdminerDeployResult.SourceNotFound:
                MessageBox.Show(
                    "Could not find adminer.php to deploy.\n\nPlace it at either:\n" +
                    $"  {Path.Combine(_config.InstallPath, "adminer.php")}\n" +
                    $"  {Path.Combine(AppContext.BaseDirectory, "adminer.php")}\n\n" +
                    $"or add it to the root of {RuntimeInstaller.RuntimeZipName} and re-run setup.",
                    "WINSERV — Deploy Adminer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                break;
        }
    }
}
