// MainForm.Designer.cs
// ─────────────────────────────────────────────────────────────────────────────
// Auto-maintained by the Visual Studio WinForms Designer.
// Edit control layout visually — do not hand-edit this file.
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

partial class MainForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        statusPanel = new Panel();
        lblPhpIcon = new Label();
        _lblPhpDot = new Label();
        _lblPhpStatus = new Label();
        _btnPhpStart = new Button();
        _btnPhpStop = new Button();
        lblDbIcon = new Label();
        _lblDbDot = new Label();
        _lblDbStatus = new Label();
        _btnDbStart = new Button();
        _btnDbStop = new Button();
        pictureBox1 = new PictureBox();
        _btnStartAll = new Button();
        _btnStopAll = new Button();
        _btnOpenWebRoot = new Button();
        _btnSettings = new Button();
        _btnOpenBrowser = new Button();
        separator = new Panel();
        lblLog = new Label();
        _logBox = new RichTextBox();
        _btnClearLog = new Button();
        _btnHide = new Button();
        statusPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // statusPanel
        // 
        statusPanel.BackColor = Color.White;
        statusPanel.BorderStyle = BorderStyle.FixedSingle;
        statusPanel.Controls.Add(lblPhpIcon);
        statusPanel.Controls.Add(_lblPhpDot);
        statusPanel.Controls.Add(_lblPhpStatus);
        statusPanel.Controls.Add(_btnPhpStart);
        statusPanel.Controls.Add(_btnPhpStop);
        statusPanel.Controls.Add(lblDbIcon);
        statusPanel.Controls.Add(_lblDbDot);
        statusPanel.Controls.Add(_lblDbStatus);
        statusPanel.Controls.Add(_btnDbStart);
        statusPanel.Controls.Add(_btnDbStop);
        statusPanel.Location = new Point(12, 12);
        statusPanel.Name = "statusPanel";
        statusPanel.Size = new Size(660, 90);
        statusPanel.TabIndex = 0;
        // 
        // lblPhpIcon
        // 
        lblPhpIcon.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPhpIcon.Location = new Point(8, 10);
        lblPhpIcon.Name = "lblPhpIcon";
        lblPhpIcon.Size = new Size(40, 22);
        lblPhpIcon.TabIndex = 0;
        lblPhpIcon.Text = "PHP";
        lblPhpIcon.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _lblPhpDot
        // 
        _lblPhpDot.Font = new Font("Segoe UI", 12F);
        _lblPhpDot.ForeColor = Color.Gray;
        _lblPhpDot.Location = new Point(65, 8);
        _lblPhpDot.Name = "_lblPhpDot";
        _lblPhpDot.Size = new Size(20, 22);
        _lblPhpDot.TabIndex = 1;
        _lblPhpDot.Text = "●";
        _lblPhpDot.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _lblPhpStatus
        // 
        _lblPhpStatus.Font = new Font("Segoe UI", 9F);
        _lblPhpStatus.Location = new Point(88, 10);
        _lblPhpStatus.Name = "_lblPhpStatus";
        _lblPhpStatus.Size = new Size(370, 22);
        _lblPhpStatus.TabIndex = 2;
        _lblPhpStatus.Text = "Stopped";
        _lblPhpStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _btnPhpStart
        // 
        _btnPhpStart.Font = new Font("Segoe UI", 8.5F);
        _btnPhpStart.Location = new Point(464, 9);
        _btnPhpStart.Name = "_btnPhpStart";
        _btnPhpStart.Size = new Size(82, 24);
        _btnPhpStart.TabIndex = 0;
        _btnPhpStart.Text = "Start";
        _btnPhpStart.Click += BtnPhpStart_Click;
        // 
        // _btnPhpStop
        // 
        _btnPhpStop.Enabled = false;
        _btnPhpStop.Font = new Font("Segoe UI", 8.5F);
        _btnPhpStop.Location = new Point(554, 9);
        _btnPhpStop.Name = "_btnPhpStop";
        _btnPhpStop.Size = new Size(82, 24);
        _btnPhpStop.TabIndex = 1;
        _btnPhpStop.Text = "Stop";
        _btnPhpStop.Click += BtnPhpStop_Click;
        // 
        // lblDbIcon
        // 
        lblDbIcon.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDbIcon.Location = new Point(8, 50);
        lblDbIcon.Name = "lblDbIcon";
        lblDbIcon.Size = new Size(50, 22);
        lblDbIcon.TabIndex = 3;
        lblDbIcon.Text = "MariaDB";
        lblDbIcon.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _lblDbDot
        // 
        _lblDbDot.Font = new Font("Segoe UI", 12F);
        _lblDbDot.ForeColor = Color.Gray;
        _lblDbDot.Location = new Point(65, 50);
        _lblDbDot.Name = "_lblDbDot";
        _lblDbDot.Size = new Size(20, 22);
        _lblDbDot.TabIndex = 4;
        _lblDbDot.Text = "●";
        _lblDbDot.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _lblDbStatus
        // 
        _lblDbStatus.Font = new Font("Segoe UI", 9F);
        _lblDbStatus.Location = new Point(88, 50);
        _lblDbStatus.Name = "_lblDbStatus";
        _lblDbStatus.Size = new Size(370, 22);
        _lblDbStatus.TabIndex = 5;
        _lblDbStatus.Text = "Stopped";
        _lblDbStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _btnDbStart
        // 
        _btnDbStart.Font = new Font("Segoe UI", 8.5F);
        _btnDbStart.Location = new Point(464, 49);
        _btnDbStart.Name = "_btnDbStart";
        _btnDbStart.Size = new Size(82, 24);
        _btnDbStart.TabIndex = 2;
        _btnDbStart.Text = "Start";
        _btnDbStart.Click += BtnDbStart_Click;
        // 
        // _btnDbStop
        // 
        _btnDbStop.Enabled = false;
        _btnDbStop.Font = new Font("Segoe UI", 8.5F);
        _btnDbStop.Location = new Point(554, 49);
        _btnDbStop.Name = "_btnDbStop";
        _btnDbStop.Size = new Size(82, 24);
        _btnDbStop.TabIndex = 3;
        _btnDbStop.Text = "Stop";
        _btnDbStop.Click += BtnDbStop_Click;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(278, 520);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(98, 68);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 6;
        pictureBox1.TabStop = false;
        // 
        // _btnStartAll
        // 
        _btnStartAll.Font = new Font("Segoe UI", 9F);
        _btnStartAll.Location = new Point(12, 116);
        _btnStartAll.Name = "_btnStartAll";
        _btnStartAll.Size = new Size(88, 28);
        _btnStartAll.TabIndex = 1;
        _btnStartAll.Text = "Start All";
        _btnStartAll.Click += BtnStartAll_Click;
        // 
        // _btnStopAll
        // 
        _btnStopAll.Font = new Font("Segoe UI", 9F);
        _btnStopAll.Location = new Point(108, 116);
        _btnStopAll.Name = "_btnStopAll";
        _btnStopAll.Size = new Size(88, 28);
        _btnStopAll.TabIndex = 2;
        _btnStopAll.Text = "Stop All";
        _btnStopAll.Click += BtnStopAll_Click;
        // 
        // _btnOpenWebRoot
        // 
        _btnOpenWebRoot.Font = new Font("Segoe UI", 9F);
        _btnOpenWebRoot.Location = new Point(210, 116);
        _btnOpenWebRoot.Name = "_btnOpenWebRoot";
        _btnOpenWebRoot.Size = new Size(110, 28);
        _btnOpenWebRoot.TabIndex = 3;
        _btnOpenWebRoot.Text = "Open WebRoot";
        _btnOpenWebRoot.Click += BtnOpenWebRoot_Click;
        // 
        // _btnSettings
        // 
        _btnSettings.Font = new Font("Segoe UI", 9F);
        _btnSettings.Location = new Point(334, 116);
        _btnSettings.Name = "_btnSettings";
        _btnSettings.Size = new Size(88, 28);
        _btnSettings.TabIndex = 4;
        _btnSettings.Text = "Settings";
        _btnSettings.Click += BtnSettings_Click;
        // 
        // _btnOpenBrowser
        // 
        _btnOpenBrowser.Font = new Font("Segoe UI", 9F);
        _btnOpenBrowser.Location = new Point(436, 116);
        _btnOpenBrowser.Name = "_btnOpenBrowser";
        _btnOpenBrowser.Size = new Size(110, 28);
        _btnOpenBrowser.TabIndex = 5;
        _btnOpenBrowser.Text = "Open Browser";
        _btnOpenBrowser.Click += BtnOpenBrowser_Click;
        // 
        // separator
        // 
        separator.BackColor = Color.Silver;
        separator.Location = new Point(12, 154);
        separator.Name = "separator";
        separator.Size = new Size(660, 1);
        separator.TabIndex = 6;
        // 
        // lblLog
        // 
        lblLog.AutoSize = true;
        lblLog.Font = new Font("Segoe UI", 8F);
        lblLog.ForeColor = Color.Gray;
        lblLog.Location = new Point(12, 162);
        lblLog.Name = "lblLog";
        lblLog.Size = new Size(86, 13);
        lblLog.TabIndex = 7;
        lblLog.Text = "Service Output:";
        // 
        // _logBox
        // 
        _logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _logBox.BackColor = Color.FromArgb(25, 25, 25);
        _logBox.Font = new Font("Consolas", 8.5F);
        _logBox.ForeColor = Color.LightGray;
        _logBox.Location = new Point(12, 180);
        _logBox.Name = "_logBox";
        _logBox.ReadOnly = true;
        _logBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        _logBox.Size = new Size(660, 325);
        _logBox.TabIndex = 7;
        _logBox.Text = "";
        _logBox.WordWrap = false;
        // 
        // _btnClearLog
        // 
        _btnClearLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _btnClearLog.Font = new Font("Segoe UI", 8F);
        _btnClearLog.Location = new Point(12, 520);
        _btnClearLog.Name = "_btnClearLog";
        _btnClearLog.Size = new Size(88, 68);
        _btnClearLog.TabIndex = 8;
        _btnClearLog.Text = "Clear Log";
        _btnClearLog.Click += BtnClearLog_Click;
        // 
        // _btnHide
        // 
        _btnHide.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnHide.Font = new Font("Segoe UI", 8F);
        _btnHide.Location = new Point(558, 520);
        _btnHide.Name = "_btnHide";
        _btnHide.Size = new Size(114, 68);
        _btnHide.TabIndex = 9;
        _btnHide.Text = "Minimise to Tray";
        _btnHide.Click += BtnHide_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 245, 245);
        ClientSize = new Size(684, 600);
        Controls.Add(pictureBox1);
        Controls.Add(statusPanel);
        Controls.Add(_btnStartAll);
        Controls.Add(_btnStopAll);
        Controls.Add(_btnOpenWebRoot);
        Controls.Add(_btnSettings);
        Controls.Add(_btnOpenBrowser);
        Controls.Add(separator);
        Controls.Add(lblLog);
        Controls.Add(_logBox);
        Controls.Add(_btnClearLog);
        Controls.Add(_btnHide);
        MinimumSize = new Size(600, 420);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WINSERV Control Panel";
        statusPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    // ── Control Fields ────────────────────────────────────────────────────────
    private System.Windows.Forms.Panel        statusPanel;
    private System.Windows.Forms.Label        lblPhpIcon;
    private System.Windows.Forms.Label        _lblPhpDot;
    private System.Windows.Forms.Label        _lblPhpStatus;
    private System.Windows.Forms.Button       _btnPhpStart;
    private System.Windows.Forms.Button       _btnPhpStop;
    private System.Windows.Forms.Label        lblDbIcon;
    private System.Windows.Forms.Label        _lblDbDot;
    private System.Windows.Forms.Label        _lblDbStatus;
    private System.Windows.Forms.Button       _btnDbStart;
    private System.Windows.Forms.Button       _btnDbStop;
    private System.Windows.Forms.Button       _btnStartAll;
    private System.Windows.Forms.Button       _btnStopAll;
    private System.Windows.Forms.Button       _btnOpenWebRoot;
    private System.Windows.Forms.Button       _btnSettings;
    private System.Windows.Forms.Button       _btnOpenBrowser;
    private System.Windows.Forms.Panel        separator;
    private System.Windows.Forms.Label        lblLog;
    private System.Windows.Forms.RichTextBox  _logBox;
    private System.Windows.Forms.Button       _btnClearLog;
    private System.Windows.Forms.Button       _btnHide;
    private PictureBox pictureBox1;
}
