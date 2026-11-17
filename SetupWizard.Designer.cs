// SetupWizard.Designer.cs
// Auto-maintained by the Visual Studio WinForms Designer. Do not hand-edit.

namespace WINSERV;

partial class SetupWizard
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupWizard));
        lblHeader = new Label();
        lblSub = new Label();
        lblInstall = new Label();
        _txtInstallPath = new TextBox();
        _btnBrowse = new Button();
        lblWebRoot = new Label();
        _txtWebRoot = new TextBox();
        lblPort = new Label();
        _txtHttpPort = new TextBox();
        lblPortNote = new Label();
        lblPwd = new Label();
        _txtDbPassword = new TextBox();
        lblPwdNote = new Label();
        _lblZipStatus = new Label();
        _progressBar = new ProgressBar();
        _logBox = new RichTextBox();
        _btnInstall = new Button();
        _btnClose = new Button();
        pictureBox1 = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // lblHeader
        // 
        lblHeader.AutoSize = true;
        lblHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblHeader.ForeColor = Color.FromArgb(0, 120, 215);
        lblHeader.Location = new Point(16, 16);
        lblHeader.Name = "lblHeader";
        lblHeader.Size = new Size(155, 25);
        lblHeader.TabIndex = 0;
        lblHeader.Text = "WINSERV Setup";
        // 
        // lblSub
        // 
        lblSub.Font = new Font("Segoe UI", 9F);
        lblSub.Location = new Point(16, 48);
        lblSub.Name = "lblSub";
        lblSub.Size = new Size(560, 20);
        lblSub.TabIndex = 1;
        lblSub.Text = "Choose where PHP and MariaDB will be installed, then click Install.";
        // 
        // lblInstall
        // 
        lblInstall.AutoSize = true;
        lblInstall.Font = new Font("Segoe UI", 9F);
        lblInstall.Location = new Point(16, 90);
        lblInstall.Name = "lblInstall";
        lblInstall.Size = new Size(68, 15);
        lblInstall.TabIndex = 2;
        lblInstall.Text = "Install Path:";
        // 
        // _txtInstallPath
        // 
        _txtInstallPath.Font = new Font("Segoe UI", 9F);
        _txtInstallPath.Location = new Point(120, 87);
        _txtInstallPath.Name = "_txtInstallPath";
        _txtInstallPath.Size = new Size(408, 23);
        _txtInstallPath.TabIndex = 0;
        // 
        // _btnBrowse
        // 
        _btnBrowse.Font = new Font("Segoe UI", 9F);
        _btnBrowse.Location = new Point(534, 85);
        _btnBrowse.Name = "_btnBrowse";
        _btnBrowse.Size = new Size(88, 25);
        _btnBrowse.TabIndex = 1;
        _btnBrowse.Text = "Browse...";
        _btnBrowse.Click += BtnBrowse_Click;
        // 
        // lblWebRoot
        // 
        lblWebRoot.AutoSize = true;
        lblWebRoot.Font = new Font("Segoe UI", 9F);
        lblWebRoot.Location = new Point(16, 126);
        lblWebRoot.Name = "lblWebRoot";
        lblWebRoot.Size = new Size(62, 15);
        lblWebRoot.TabIndex = 3;
        lblWebRoot.Text = "Web Root:";
        // 
        // _txtWebRoot
        // 
        _txtWebRoot.Font = new Font("Segoe UI", 9F);
        _txtWebRoot.Location = new Point(120, 123);
        _txtWebRoot.Name = "_txtWebRoot";
        _txtWebRoot.Size = new Size(502, 23);
        _txtWebRoot.TabIndex = 2;
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Font = new Font("Segoe UI", 9F);
        lblPort.Location = new Point(16, 162);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(65, 15);
        lblPort.TabIndex = 4;
        lblPort.Text = "HTTP Port:";
        // 
        // _txtHttpPort
        // 
        _txtHttpPort.Font = new Font("Segoe UI", 9F);
        _txtHttpPort.Location = new Point(120, 159);
        _txtHttpPort.Name = "_txtHttpPort";
        _txtHttpPort.Size = new Size(80, 23);
        _txtHttpPort.TabIndex = 3;
        // 
        // lblPortNote
        // 
        lblPortNote.AutoSize = true;
        lblPortNote.Font = new Font("Segoe UI", 8F);
        lblPortNote.ForeColor = Color.Gray;
        lblPortNote.Location = new Point(210, 162);
        lblPortNote.Name = "lblPortNote";
        lblPortNote.Size = new Size(161, 13);
        lblPortNote.TabIndex = 5;
        lblPortNote.Text = "Must be > 1024. Default: 8080";
        // 
        // lblPwd
        // 
        lblPwd.AutoSize = true;
        lblPwd.Font = new Font("Segoe UI", 9F);
        lblPwd.Location = new Point(16, 198);
        lblPwd.Name = "lblPwd";
        lblPwd.Size = new Size(106, 15);
        lblPwd.TabIndex = 6;
        lblPwd.Text = "DB Root Password:";
        // 
        // _txtDbPassword
        // 
        _txtDbPassword.Font = new Font("Segoe UI", 9F);
        _txtDbPassword.Location = new Point(140, 195);
        _txtDbPassword.Name = "_txtDbPassword";
        _txtDbPassword.PasswordChar = '●';
        _txtDbPassword.Size = new Size(200, 23);
        _txtDbPassword.TabIndex = 4;
        // 
        // lblPwdNote
        // 
        lblPwdNote.AutoSize = true;
        lblPwdNote.Font = new Font("Segoe UI", 8F);
        lblPwdNote.ForeColor = Color.Gray;
        lblPwdNote.Location = new Point(346, 200);
        lblPwdNote.Name = "lblPwdNote";
        lblPwdNote.Size = new Size(258, 13);
        lblPwdNote.TabIndex = 7;
        lblPwdNote.Text = "Stored in plaintext in config.json (local dev only).";
        // 
        // _lblZipStatus
        // 
        _lblZipStatus.Font = new Font("Segoe UI", 9F);
        _lblZipStatus.Location = new Point(16, 232);
        _lblZipStatus.Name = "_lblZipStatus";
        _lblZipStatus.Size = new Size(560, 20);
        _lblZipStatus.TabIndex = 8;
        _lblZipStatus.Text = "Checking runtime zip...";
        // 
        // _progressBar
        // 
        _progressBar.Location = new Point(16, 260);
        _progressBar.Name = "_progressBar";
        _progressBar.Size = new Size(606, 20);
        _progressBar.TabIndex = 5;
        // 
        // _logBox
        // 
        _logBox.BackColor = Color.FromArgb(30, 30, 30);
        _logBox.Font = new Font("Consolas", 8.5F);
        _logBox.ForeColor = Color.LightGray;
        _logBox.Location = new Point(16, 290);
        _logBox.Name = "_logBox";
        _logBox.ReadOnly = true;
        _logBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        _logBox.Size = new Size(606, 150);
        _logBox.TabIndex = 6;
        _logBox.Text = "";
        // 
        // _btnInstall
        // 
        _btnInstall.Font = new Font("Segoe UI", 9F);
        _btnInstall.Location = new Point(440, 455);
        _btnInstall.Name = "_btnInstall";
        _btnInstall.Size = new Size(88, 30);
        _btnInstall.TabIndex = 7;
        _btnInstall.Text = "Install";
        _btnInstall.Click += BtnInstall_Click;
        // 
        // _btnClose
        // 
        _btnClose.Font = new Font("Segoe UI", 9F);
        _btnClose.Location = new Point(534, 455);
        _btnClose.Name = "_btnClose";
        _btnClose.Size = new Size(88, 30);
        _btnClose.TabIndex = 8;
        _btnClose.Text = "Close";
        _btnClose.Click += BtnClose_Click;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(513, 12);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(106, 65);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 9;
        pictureBox1.TabStop = false;
        // 
        // SetupWizard
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(634, 501);
        Controls.Add(pictureBox1);
        Controls.Add(lblHeader);
        Controls.Add(lblSub);
        Controls.Add(lblInstall);
        Controls.Add(_txtInstallPath);
        Controls.Add(_btnBrowse);
        Controls.Add(lblWebRoot);
        Controls.Add(_txtWebRoot);
        Controls.Add(lblPort);
        Controls.Add(_txtHttpPort);
        Controls.Add(lblPortNote);
        Controls.Add(lblPwd);
        Controls.Add(_txtDbPassword);
        Controls.Add(lblPwdNote);
        Controls.Add(_lblZipStatus);
        Controls.Add(_progressBar);
        Controls.Add(_logBox);
        Controls.Add(_btnInstall);
        Controls.Add(_btnClose);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimumSize = new Size(600, 540);
        Name = "SetupWizard";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WINSERV — First Run Setup";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    // ── Control Fields ────────────────────────────────────────────────────────
    private System.Windows.Forms.Label        lblHeader;
    private System.Windows.Forms.Label        lblSub;
    private System.Windows.Forms.Label        lblInstall;
    private System.Windows.Forms.TextBox      _txtInstallPath;
    private System.Windows.Forms.Button       _btnBrowse;
    private System.Windows.Forms.Label        lblWebRoot;
    private System.Windows.Forms.TextBox      _txtWebRoot;
    private System.Windows.Forms.Label        lblPort;
    private System.Windows.Forms.TextBox      _txtHttpPort;
    private System.Windows.Forms.Label        lblPortNote;
    private System.Windows.Forms.Label        lblPwd;
    private System.Windows.Forms.TextBox      _txtDbPassword;
    private System.Windows.Forms.Label        lblPwdNote;
    private System.Windows.Forms.Label        _lblZipStatus;
    private System.Windows.Forms.ProgressBar  _progressBar;
    private System.Windows.Forms.RichTextBox  _logBox;
    private System.Windows.Forms.Button       _btnInstall;
    private System.Windows.Forms.Button       _btnClose;
    private PictureBox pictureBox1;
}
