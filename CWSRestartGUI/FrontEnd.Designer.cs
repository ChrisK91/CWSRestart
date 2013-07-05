namespace CWSRestartGUI
{
    partial class FrontEnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.configGroupBox = new System.Windows.Forms.GroupBox();
            this.ignoreLoopback = new System.Windows.Forms.CheckBox();
            this.ignoreLAN = new System.Windows.Forms.CheckBox();
            this.ignoreInternet = new System.Windows.Forms.CheckBox();
            this.actionTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectActionButton = new System.Windows.Forms.Button();
            this.refreshLanIP = new System.Windows.Forms.Button();
            this.refreshExternalIp = new System.Windows.Forms.Button();
            this.lanIPTextBox = new System.Windows.Forms.TextBox();
            this.externalIPTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.SelectServerDialog = new System.Windows.Forms.OpenFileDialog();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.singleCheckButton = new System.Windows.Forms.Button();
            this.toggleServerWatcher = new System.Windows.Forms.Button();
            this.Watcher = new System.Windows.Forms.Timer(this.components);
            this.timerCountdown = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.intervalTextBox = new System.Windows.Forms.TextBox();
            this.setIntervalButton = new System.Windows.Forms.Button();
            this.configGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // configGroupBox
            // 
            this.configGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configGroupBox.Controls.Add(this.ignoreLoopback);
            this.configGroupBox.Controls.Add(this.ignoreLAN);
            this.configGroupBox.Controls.Add(this.ignoreInternet);
            this.configGroupBox.Controls.Add(this.actionTextBox);
            this.configGroupBox.Controls.Add(this.label3);
            this.configGroupBox.Controls.Add(this.label2);
            this.configGroupBox.Controls.Add(this.selectActionButton);
            this.configGroupBox.Controls.Add(this.refreshLanIP);
            this.configGroupBox.Controls.Add(this.refreshExternalIp);
            this.configGroupBox.Controls.Add(this.lanIPTextBox);
            this.configGroupBox.Controls.Add(this.externalIPTextBox);
            this.configGroupBox.Controls.Add(this.label1);
            this.configGroupBox.Controls.Add(this.ipLabel);
            this.configGroupBox.Location = new System.Drawing.Point(12, 12);
            this.configGroupBox.Name = "configGroupBox";
            this.configGroupBox.Size = new System.Drawing.Size(560, 115);
            this.configGroupBox.TabIndex = 0;
            this.configGroupBox.TabStop = false;
            this.configGroupBox.Text = "Configuration";
            // 
            // ignoreLoopback
            // 
            this.ignoreLoopback.AutoSize = true;
            this.ignoreLoopback.Location = new System.Drawing.Point(424, 92);
            this.ignoreLoopback.Name = "ignoreLoopback";
            this.ignoreLoopback.Size = new System.Drawing.Size(129, 17);
            this.ignoreLoopback.TabIndex = 9;
            this.ignoreLoopback.Text = "Loopback (Localhost)";
            this.ignoreLoopback.UseVisualStyleBackColor = true;
            // 
            // ignoreLAN
            // 
            this.ignoreLAN.AutoSize = true;
            this.ignoreLAN.Location = new System.Drawing.Point(316, 92);
            this.ignoreLAN.Name = "ignoreLAN";
            this.ignoreLAN.Size = new System.Drawing.Size(95, 17);
            this.ignoreLAN.TabIndex = 8;
            this.ignoreLAN.Text = "Local Network";
            this.ignoreLAN.UseVisualStyleBackColor = true;
            // 
            // ignoreInternet
            // 
            this.ignoreInternet.AutoSize = true;
            this.ignoreInternet.Location = new System.Drawing.Point(241, 92);
            this.ignoreInternet.Name = "ignoreInternet";
            this.ignoreInternet.Size = new System.Drawing.Size(62, 17);
            this.ignoreInternet.TabIndex = 7;
            this.ignoreInternet.Text = "Internet";
            this.ignoreInternet.UseVisualStyleBackColor = true;
            // 
            // actionTextBox
            // 
            this.actionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionTextBox.Location = new System.Drawing.Point(241, 65);
            this.actionTextBox.Name = "actionTextBox";
            this.actionTextBox.ReadOnly = true;
            this.actionTextBox.Size = new System.Drawing.Size(232, 20);
            this.actionTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ignore timeouts from (Watcher):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "When the server is not responding, restart with:";
            // 
            // selectActionButton
            // 
            this.selectActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectActionButton.Location = new System.Drawing.Point(479, 63);
            this.selectActionButton.Name = "selectActionButton";
            this.selectActionButton.Size = new System.Drawing.Size(75, 23);
            this.selectActionButton.TabIndex = 6;
            this.selectActionButton.Text = "Browse...";
            this.selectActionButton.UseVisualStyleBackColor = true;
            this.selectActionButton.Click += new System.EventHandler(this.selectActionButton_Click);
            // 
            // refreshLanIP
            // 
            this.refreshLanIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshLanIP.Location = new System.Drawing.Point(479, 37);
            this.refreshLanIP.Name = "refreshLanIP";
            this.refreshLanIP.Size = new System.Drawing.Size(75, 23);
            this.refreshLanIP.TabIndex = 4;
            this.refreshLanIP.Text = "Refresh";
            this.refreshLanIP.UseVisualStyleBackColor = true;
            this.refreshLanIP.Click += new System.EventHandler(this.refreshLanIp_Click);
            // 
            // refreshExternalIp
            // 
            this.refreshExternalIp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshExternalIp.Location = new System.Drawing.Point(479, 11);
            this.refreshExternalIp.Name = "refreshExternalIp";
            this.refreshExternalIp.Size = new System.Drawing.Size(75, 23);
            this.refreshExternalIp.TabIndex = 2;
            this.refreshExternalIp.Text = "Refresh";
            this.refreshExternalIp.UseVisualStyleBackColor = true;
            this.refreshExternalIp.Click += new System.EventHandler(this.refreshExternalIp_Click);
            // 
            // lanIPTextBox
            // 
            this.lanIPTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lanIPTextBox.Location = new System.Drawing.Point(70, 39);
            this.lanIPTextBox.Name = "lanIPTextBox";
            this.lanIPTextBox.Size = new System.Drawing.Size(403, 20);
            this.lanIPTextBox.TabIndex = 3;
            // 
            // externalIPTextBox
            // 
            this.externalIPTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.externalIPTextBox.Location = new System.Drawing.Point(70, 13);
            this.externalIPTextBox.Name = "externalIPTextBox";
            this.externalIPTextBox.Size = new System.Drawing.Size(403, 20);
            this.externalIPTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "LAN IP";
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(6, 16);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(58, 13);
            this.ipLabel.TabIndex = 0;
            this.ipLabel.Text = "External IP";
            // 
            // SelectServerDialog
            // 
            this.SelectServerDialog.DefaultExt = "exe";
            this.SelectServerDialog.FileName = "openFileDialog1";
            this.SelectServerDialog.Filter = "Executables|*.exe|Batch Files|*.bat|All files|*.*";
            this.SelectServerDialog.Title = "Select the file you want to run, after the server is down";
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.Location = new System.Drawing.Point(142, 133);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(430, 216);
            this.logTextBox.TabIndex = 1000;
            // 
            // singleCheckButton
            // 
            this.singleCheckButton.Location = new System.Drawing.Point(12, 133);
            this.singleCheckButton.Name = "singleCheckButton";
            this.singleCheckButton.Size = new System.Drawing.Size(124, 23);
            this.singleCheckButton.TabIndex = 10;
            this.singleCheckButton.Text = "Validate configuration";
            this.singleCheckButton.UseVisualStyleBackColor = true;
            this.singleCheckButton.Click += new System.EventHandler(this.singleCheckButton_Click);
            // 
            // toggleServerWatcher
            // 
            this.toggleServerWatcher.Location = new System.Drawing.Point(12, 162);
            this.toggleServerWatcher.Name = "toggleServerWatcher";
            this.toggleServerWatcher.Size = new System.Drawing.Size(124, 23);
            this.toggleServerWatcher.TabIndex = 11;
            this.toggleServerWatcher.Text = "Start Watcher";
            this.toggleServerWatcher.UseVisualStyleBackColor = true;
            this.toggleServerWatcher.Click += new System.EventHandler(this.toggleServerWatcher_Click);
            // 
            // Watcher
            // 
            this.Watcher.Interval = 1000;
            this.Watcher.Tick += new System.EventHandler(this.Watcher_Tick);
            // 
            // timerCountdown
            // 
            this.timerCountdown.Location = new System.Drawing.Point(12, 191);
            this.timerCountdown.Maximum = 60000;
            this.timerCountdown.Name = "timerCountdown";
            this.timerCountdown.Size = new System.Drawing.Size(124, 23);
            this.timerCountdown.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Timer interval in seconds";
            // 
            // intervalTextBox
            // 
            this.intervalTextBox.Location = new System.Drawing.Point(12, 235);
            this.intervalTextBox.Name = "intervalTextBox";
            this.intervalTextBox.Size = new System.Drawing.Size(81, 20);
            this.intervalTextBox.TabIndex = 13;
            this.intervalTextBox.Text = "60";
            // 
            // setIntervalButton
            // 
            this.setIntervalButton.Location = new System.Drawing.Point(99, 233);
            this.setIntervalButton.Name = "setIntervalButton";
            this.setIntervalButton.Size = new System.Drawing.Size(37, 23);
            this.setIntervalButton.TabIndex = 14;
            this.setIntervalButton.Text = "OK";
            this.setIntervalButton.UseVisualStyleBackColor = true;
            this.setIntervalButton.Click += new System.EventHandler(this.setIntervalButton_Click);
            // 
            // FrontEnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.setIntervalButton);
            this.Controls.Add(this.intervalTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.timerCountdown);
            this.Controls.Add(this.toggleServerWatcher);
            this.Controls.Add(this.singleCheckButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.configGroupBox);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrontEnd";
            this.Text = "CWS Restart";
            this.configGroupBox.ResumeLayout(false);
            this.configGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox configGroupBox;
        private System.Windows.Forms.Button refreshExternalIp;
        private System.Windows.Forms.TextBox externalIPTextBox;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Button refreshLanIP;
        private System.Windows.Forms.TextBox lanIPTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog SelectServerDialog;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Button singleCheckButton;
        private System.Windows.Forms.Button toggleServerWatcher;
        private System.Windows.Forms.TextBox actionTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button selectActionButton;
        private System.Windows.Forms.CheckBox ignoreLoopback;
        private System.Windows.Forms.CheckBox ignoreLAN;
        private System.Windows.Forms.CheckBox ignoreInternet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer Watcher;
        private System.Windows.Forms.ProgressBar timerCountdown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox intervalTextBox;
        private System.Windows.Forms.Button setIntervalButton;
    }
}