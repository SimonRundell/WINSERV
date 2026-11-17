// SettingsForm.Designer.cs
// Auto-maintained by the Visual Studio WinForms Designer. Do not hand-edit.

namespace WINSERV;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
        _table = new TableLayoutPanel();
        _lblInstallPath = new Label();
        _txtInstallPath = new TextBox();
        _lblWebRoot = new Label();
        _txtWebRoot = new TextBox();
        _lblDataDir = new Label();
        _txtDataDir = new TextBox();
        _lblLogDir = new Label();
        _txtLogDir = new TextBox();
        _lblHttpPort = new Label();
        _nudHttpPort = new NumericUpDown();
        _lblDbPort = new Label();
        _nudDbPort = new NumericUpDown();
        _lblDbPassword = new Label();
        _txtDbPassword = new TextBox();
        _lblAutoStart = new Label();
        _chkAutoStart = new CheckBox();
        _lblShowWindow = new Label();
        _chkShowWindow = new CheckBox();
        _lblConfigPath = new Label();
        _lblWarning = new Label();
        _btnSave = new Button();
        _btnCancel = new Button();
        _btnDeployAdminer = new Button();
        pictureBox1 = new PictureBox();
        _table.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_nudHttpPort).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_nudDbPort).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // _table
        // 
        _table.ColumnCount = 2;
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _table.Controls.Add(_lblInstallPath, 0, 0);
        _table.Controls.Add(_txtInstallPath, 1, 0);
        _table.Controls.Add(_lblWebRoot, 0, 1);
        _table.Controls.Add(_txtWebRoot, 1, 1);
        _table.Controls.Add(_lblDataDir, 0, 2);
        _table.Controls.Add(_txtDataDir, 1, 2);
        _table.Controls.Add(_lblLogDir, 0, 3);
        _table.Controls.Add(_txtLogDir, 1, 3);
        _table.Controls.Add(_lblHttpPort, 0, 4);
        _table.Controls.Add(_nudHttpPort, 1, 4);
        _table.Controls.Add(_lblDbPort, 0, 5);
        _table.Controls.Add(_nudDbPort, 1, 5);
        _table.Controls.Add(_lblDbPassword, 0, 6);
        _table.Controls.Add(_txtDbPassword, 1, 6);
        _table.Controls.Add(_lblAutoStart, 0, 7);
        _table.Controls.Add(_chkAutoStart, 1, 7);
        _table.Controls.Add(_lblShowWindow, 0, 8);
        _table.Controls.Add(_chkShowWindow, 1, 8);
        _table.Location = new Point(12, 12);
        _table.Name = "_table";
        _table.RowCount = 9;
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        _table.Size = new Size(534, 324);
        _table.TabIndex = 0;
        // 
        // _lblInstallPath
        // 
        _lblInstallPath.Dock = DockStyle.Fill;
        _lblInstallPath.Font = new Font("Segoe UI", 9F);
        _lblInstallPath.Location = new Point(3, 0);
        _lblInstallPath.Name = "_lblInstallPath";
        _lblInstallPath.Size = new Size(144, 36);
        _lblInstallPath.TabIndex = 0;
        _lblInstallPath.Text = "Install Path:";
        _lblInstallPath.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _txtInstallPath
        // 
        _txtInstallPath.Dock = DockStyle.Fill;
        _txtInstallPath.Font = new Font("Segoe UI", 9F);
        _txtInstallPath.Location = new Point(153, 3);
        _txtInstallPath.Name = "_txtInstallPath";
        _txtInstallPath.Size = new Size(378, 23);
        _txtInstallPath.TabIndex = 1;
        // 
        // _lblWebRoot
        // 
        _lblWebRoot.Dock = DockStyle.Fill;
        _lblWebRoot.Font = new Font("Segoe UI", 9F);
        _lblWebRoot.Location = new Point(3, 36);
        _lblWebRoot.Name = "_lblWebRoot";
        _lblWebRoot.Size = new Size(144, 36);
        _lblWebRoot.TabIndex = 2;
        _lblWebRoot.Text = "Web Root:";
        _lblWebRoot.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _txtWebRoot
        // 
        _txtWebRoot.Dock = DockStyle.Fill;
        _txtWebRoot.Font = new Font("Segoe UI", 9F);
        _txtWebRoot.Location = new Point(153, 39);
        _txtWebRoot.Name = "_txtWebRoot";
        _txtWebRoot.Size = new Size(378, 23);
        _txtWebRoot.TabIndex = 3;
        // 
        // _lblDataDir
        // 
        _lblDataDir.Dock = DockStyle.Fill;
        _lblDataDir.Font = new Font("Segoe UI", 9F);
        _lblDataDir.Location = new Point(3, 72);
        _lblDataDir.Name = "_lblDataDir";
        _lblDataDir.Size = new Size(144, 36);
        _lblDataDir.TabIndex = 4;
        _lblDataDir.Text = "DB Data Directory:";
        _lblDataDir.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _txtDataDir
        // 
        _txtDataDir.Dock = DockStyle.Fill;
        _txtDataDir.Font = new Font("Segoe UI", 9F);
        _txtDataDir.Location = new Point(153, 75);
        _txtDataDir.Name = "_txtDataDir";
        _txtDataDir.Size = new Size(378, 23);
        _txtDataDir.TabIndex = 5;
        // 
        // _lblLogDir
        // 
        _lblLogDir.Dock = DockStyle.Fill;
        _lblLogDir.Font = new Font("Segoe UI", 9F);
        _lblLogDir.Location = new Point(3, 108);
        _lblLogDir.Name = "_lblLogDir";
        _lblLogDir.Size = new Size(144, 36);
        _lblLogDir.TabIndex = 6;
        _lblLogDir.Text = "Log Directory:";
        _lblLogDir.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _txtLogDir
        // 
        _txtLogDir.Dock = DockStyle.Fill;
        _txtLogDir.Font = new Font("Segoe UI", 9F);
        _txtLogDir.Location = new Point(153, 111);
        _txtLogDir.Name = "_txtLogDir";
        _txtLogDir.Size = new Size(378, 23);
        _txtLogDir.TabIndex = 7;
        // 
        // _lblHttpPort
        // 
        _lblHttpPort.Dock = DockStyle.Fill;
        _lblHttpPort.Font = new Font("Segoe UI", 9F);
        _lblHttpPort.Location = new Point(3, 144);
        _lblHttpPort.Name = "_lblHttpPort";
        _lblHttpPort.Size = new Size(144, 36);
        _lblHttpPort.TabIndex = 8;
        _lblHttpPort.Text = "HTTP Port:";
        _lblHttpPort.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _nudHttpPort
        // 
        _nudHttpPort.Dock = DockStyle.Fill;
        _nudHttpPort.Font = new Font("Segoe UI", 9F);
        _nudHttpPort.Location = new Point(153, 147);
        _nudHttpPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        _nudHttpPort.Minimum = new decimal(new int[] { 1025, 0, 0, 0 });
        _nudHttpPort.Name = "_nudHttpPort";
        _nudHttpPort.Size = new Size(378, 23);
        _nudHttpPort.TabIndex = 9;
        _nudHttpPort.Value = new decimal(new int[] { 8080, 0, 0, 0 });
        // 
        // _lblDbPort
        // 
        _lblDbPort.Dock = DockStyle.Fill;
        _lblDbPort.Font = new Font("Segoe UI", 9F);
        _lblDbPort.Location = new Point(3, 180);
        _lblDbPort.Name = "_lblDbPort";
        _lblDbPort.Size = new Size(144, 36);
        _lblDbPort.TabIndex = 10;
        _lblDbPort.Text = "MariaDB Port:";
        _lblDbPort.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _nudDbPort
        // 
        _nudDbPort.Dock = DockStyle.Fill;
        _nudDbPort.Font = new Font("Segoe UI", 9F);
        _nudDbPort.Location = new Point(153, 183);
        _nudDbPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        _nudDbPort.Minimum = new decimal(new int[] { 1025, 0, 0, 0 });
        _nudDbPort.Name = "_nudDbPort";
        _nudDbPort.Size = new Size(378, 23);
        _nudDbPort.TabIndex = 11;
        _nudDbPort.Value = new decimal(new int[] { 3307, 0, 0, 0 });
        // 
        // _lblDbPassword
        // 
        _lblDbPassword.Dock = DockStyle.Fill;
        _lblDbPassword.Font = new Font("Segoe UI", 9F);
        _lblDbPassword.Location = new Point(3, 216);
        _lblDbPassword.Name = "_lblDbPassword";
        _lblDbPassword.Size = new Size(144, 36);
        _lblDbPassword.TabIndex = 12;
        _lblDbPassword.Text = "DB Root Password:";
        _lblDbPassword.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _txtDbPassword
        // 
        _txtDbPassword.Dock = DockStyle.Fill;
        _txtDbPassword.Font = new Font("Segoe UI", 9F);
        _txtDbPassword.Location = new Point(153, 219);
        _txtDbPassword.Name = "_txtDbPassword";
        _txtDbPassword.PasswordChar = '●';
        _txtDbPassword.Size = new Size(378, 23);
        _txtDbPassword.TabIndex = 13;
        // 
        // _lblAutoStart
        // 
        _lblAutoStart.Dock = DockStyle.Fill;
        _lblAutoStart.Font = new Font("Segoe UI", 9F);
        _lblAutoStart.Location = new Point(3, 252);
        _lblAutoStart.Name = "_lblAutoStart";
        _lblAutoStart.Size = new Size(144, 36);
        _lblAutoStart.TabIndex = 14;
        _lblAutoStart.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _chkAutoStart
        // 
        _chkAutoStart.Dock = DockStyle.Fill;
        _chkAutoStart.Font = new Font("Segoe UI", 9F);
        _chkAutoStart.Location = new Point(153, 255);
        _chkAutoStart.Name = "_chkAutoStart";
        _chkAutoStart.Size = new Size(378, 30);
        _chkAutoStart.TabIndex = 15;
        _chkAutoStart.Text = "Auto-start services on launch";
        // 
        // _lblShowWindow
        // 
        _lblShowWindow.Dock = DockStyle.Fill;
        _lblShowWindow.Font = new Font("Segoe UI", 9F);
        _lblShowWindow.Location = new Point(3, 288);
        _lblShowWindow.Name = "_lblShowWindow";
        _lblShowWindow.Size = new Size(144, 36);
        _lblShowWindow.TabIndex = 16;
        _lblShowWindow.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _chkShowWindow
        // 
        _chkShowWindow.Dock = DockStyle.Fill;
        _chkShowWindow.Font = new Font("Segoe UI", 9F);
        _chkShowWindow.Location = new Point(153, 291);
        _chkShowWindow.Name = "_chkShowWindow";
        _chkShowWindow.Size = new Size(378, 30);
        _chkShowWindow.TabIndex = 17;
        _chkShowWindow.Text = "Show window on startup (not just tray)";
        // 
        // _lblConfigPath
        // 
        _lblConfigPath.Font = new Font("Segoe UI", 8F);
        _lblConfigPath.ForeColor = Color.Gray;
        _lblConfigPath.Location = new Point(12, 342);
        _lblConfigPath.Name = "_lblConfigPath";
        _lblConfigPath.Size = new Size(544, 20);
        _lblConfigPath.TabIndex = 1;
        _lblConfigPath.Text = "Config file:";
        // 
        // _lblWarning
        // 
        _lblWarning.Font = new Font("Segoe UI", 8F);
        _lblWarning.ForeColor = Color.DarkOrange;
        _lblWarning.Location = new Point(12, 364);
        _lblWarning.Name = "_lblWarning";
        _lblWarning.Size = new Size(530, 20);
        _lblWarning.TabIndex = 2;
        _lblWarning.Text = "Changes to paths or ports require services to be restarted.";
        // 
        // _btnSave
        // 
        _btnSave.Font = new Font("Segoe UI", 9F);
        _btnSave.Location = new Point(346, 371);
        _btnSave.Name = "_btnSave";
        _btnSave.Size = new Size(88, 28);
        _btnSave.TabIndex = 1;
        _btnSave.Text = "Save";
        _btnSave.Click += BtnSave_Click;
        // 
        // _btnCancel
        // 
        _btnCancel.DialogResult = DialogResult.Cancel;
        _btnCancel.Font = new Font("Segoe UI", 9F);
        _btnCancel.Location = new Point(346, 408);
        _btnCancel.Name = "_btnCancel";
        _btnCancel.Size = new Size(88, 28);
        _btnCancel.TabIndex = 2;
        _btnCancel.Text = "Cancel";
        //
        // pictureBox1
        //
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(450, 371);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(106, 65);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 10;
        pictureBox1.TabStop = false;
        //
        // _btnDeployAdminer
        //
        _btnDeployAdminer.Font = new Font("Segoe UI", 9F);
        _btnDeployAdminer.Location = new Point(12, 440);
        _btnDeployAdminer.Name = "_btnDeployAdminer";
        _btnDeployAdminer.Size = new Size(160, 28);
        _btnDeployAdminer.TabIndex = 20;
        _btnDeployAdminer.Text = "Deploy Adminer";
        _btnDeployAdminer.Click += BtnDeployAdminer_Click;
        //
        // SettingsForm
        //
        AcceptButton = _btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _btnCancel;
        ClientSize = new Size(578, 484);
        Controls.Add(pictureBox1);
        Controls.Add(_table);
        Controls.Add(_lblConfigPath);
        Controls.Add(_btnSave);
        Controls.Add(_btnCancel);
        Controls.Add(_lblWarning);
        Controls.Add(_btnDeployAdminer);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(560, 496);
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "WINSERV — Settings";
        _table.ResumeLayout(false);
        _table.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_nudHttpPort).EndInit();
        ((System.ComponentModel.ISupportInitialize)_nudDbPort).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
    }

    // ── Control Fields ────────────────────────────────────────────────────────
    private System.Windows.Forms.TableLayoutPanel  _table;
    private System.Windows.Forms.Label             _lblInstallPath;
    private System.Windows.Forms.TextBox           _txtInstallPath;
    private System.Windows.Forms.Label             _lblWebRoot;
    private System.Windows.Forms.TextBox           _txtWebRoot;
    private System.Windows.Forms.Label             _lblDataDir;
    private System.Windows.Forms.TextBox           _txtDataDir;
    private System.Windows.Forms.Label             _lblLogDir;
    private System.Windows.Forms.TextBox           _txtLogDir;
    private System.Windows.Forms.Label             _lblHttpPort;
    private System.Windows.Forms.NumericUpDown     _nudHttpPort;
    private System.Windows.Forms.Label             _lblDbPort;
    private System.Windows.Forms.NumericUpDown     _nudDbPort;
    private System.Windows.Forms.Label             _lblDbPassword;
    private System.Windows.Forms.TextBox           _txtDbPassword;
    private System.Windows.Forms.Label             _lblAutoStart;
    private System.Windows.Forms.CheckBox          _chkAutoStart;
    private System.Windows.Forms.Label             _lblShowWindow;
    private System.Windows.Forms.CheckBox          _chkShowWindow;
    private System.Windows.Forms.Label             _lblConfigPath;
    private System.Windows.Forms.Label             _lblWarning;
    private System.Windows.Forms.Button            _btnSave;
    private System.Windows.Forms.Button            _btnCancel;
    private System.Windows.Forms.Button            _btnDeployAdminer;
    private PictureBox pictureBox1;
}
