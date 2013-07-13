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
                stats.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrontEnd));
            this.configGroupBox = new System.Windows.Forms.GroupBox();
            this.CheckLoopbackCheckBox = new System.Windows.Forms.CheckBox();
            this.checkLANCheckBox = new System.Windows.Forms.CheckBox();
            this.checkInternetCheckBox = new System.Windows.Forms.CheckBox();
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
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.singleCheckButton = new System.Windows.Forms.Button();
            this.toggleServerWatcher = new System.Windows.Forms.Button();
            this.Watcher = new System.Windows.Forms.Timer(this.components);
            this.timerCountdown = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.intervalTextBox = new System.Windows.Forms.TextBox();
            this.setIntervalButton = new System.Windows.Forms.Button();
            this.checkUpdate = new System.Windows.Forms.Button();
            this.stopServerButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.connectedPlayersLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedPlayersCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.runtimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.runtimeValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.playerCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toggleStatisticsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.startServerButton = new System.Windows.Forms.Button();
            this.logFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.restartServerButton = new System.Windows.Forms.Button();
            this.UPnPButton = new System.Windows.Forms.Button();
            this.configGroupBox.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // configGroupBox
            // 
            this.configGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configGroupBox.Controls.Add(this.UPnPButton);
            this.configGroupBox.Controls.Add(this.CheckLoopbackCheckBox);
            this.configGroupBox.Controls.Add(this.checkLANCheckBox);
            this.configGroupBox.Controls.Add(this.checkInternetCheckBox);
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
            this.configGroupBox.Size = new System.Drawing.Size(660, 115);
            this.configGroupBox.TabIndex = 0;
            this.configGroupBox.TabStop = false;
            this.configGroupBox.Text = "Configuration";
            // 
            // CheckLoopbackCheckBox
            // 
            this.CheckLoopbackCheckBox.AutoSize = true;
            this.CheckLoopbackCheckBox.Checked = true;
            this.CheckLoopbackCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckLoopbackCheckBox.Location = new System.Drawing.Point(424, 92);
            this.CheckLoopbackCheckBox.Name = "CheckLoopbackCheckBox";
            this.CheckLoopbackCheckBox.Size = new System.Drawing.Size(129, 17);
            this.CheckLoopbackCheckBox.TabIndex = 8;
            this.CheckLoopbackCheckBox.Text = "Loopback (Localhost)";
            this.CheckLoopbackCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkLANCheckBox
            // 
            this.checkLANCheckBox.AutoSize = true;
            this.checkLANCheckBox.Checked = true;
            this.checkLANCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLANCheckBox.Location = new System.Drawing.Point(316, 92);
            this.checkLANCheckBox.Name = "checkLANCheckBox";
            this.checkLANCheckBox.Size = new System.Drawing.Size(95, 17);
            this.checkLANCheckBox.TabIndex = 7;
            this.checkLANCheckBox.Text = "Local Network";
            this.checkLANCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkInternetCheckBox
            // 
            this.checkInternetCheckBox.AutoSize = true;
            this.checkInternetCheckBox.Checked = true;
            this.checkInternetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkInternetCheckBox.Location = new System.Drawing.Point(241, 92);
            this.checkInternetCheckBox.Name = "checkInternetCheckBox";
            this.checkInternetCheckBox.Size = new System.Drawing.Size(62, 17);
            this.checkInternetCheckBox.TabIndex = 6;
            this.checkInternetCheckBox.Text = "Internet";
            this.checkInternetCheckBox.UseVisualStyleBackColor = true;
            // 
            // actionTextBox
            // 
            this.actionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionTextBox.Location = new System.Drawing.Point(241, 65);
            this.actionTextBox.Name = "actionTextBox";
            this.actionTextBox.ReadOnly = true;
            this.actionTextBox.Size = new System.Drawing.Size(332, 20);
            this.actionTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Server should be accessible from:";
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
            this.selectActionButton.Location = new System.Drawing.Point(579, 63);
            this.selectActionButton.Name = "selectActionButton";
            this.selectActionButton.Size = new System.Drawing.Size(75, 23);
            this.selectActionButton.TabIndex = 5;
            this.selectActionButton.Text = "Browse...";
            this.selectActionButton.UseVisualStyleBackColor = true;
            this.selectActionButton.Click += new System.EventHandler(this.selectActionButton_Click);
            // 
            // refreshLanIP
            // 
            this.refreshLanIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshLanIP.Location = new System.Drawing.Point(579, 37);
            this.refreshLanIP.Name = "refreshLanIP";
            this.refreshLanIP.Size = new System.Drawing.Size(75, 23);
            this.refreshLanIP.TabIndex = 3;
            this.refreshLanIP.Text = "Refresh";
            this.refreshLanIP.UseVisualStyleBackColor = true;
            this.refreshLanIP.Click += new System.EventHandler(this.refreshLanIp_Click);
            // 
            // refreshExternalIp
            // 
            this.refreshExternalIp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshExternalIp.Location = new System.Drawing.Point(579, 11);
            this.refreshExternalIp.Name = "refreshExternalIp";
            this.refreshExternalIp.Size = new System.Drawing.Size(75, 23);
            this.refreshExternalIp.TabIndex = 1;
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
            this.lanIPTextBox.ReadOnly = true;
            this.lanIPTextBox.Size = new System.Drawing.Size(503, 20);
            this.lanIPTextBox.TabIndex = 2;
            // 
            // externalIPTextBox
            // 
            this.externalIPTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.externalIPTextBox.Location = new System.Drawing.Point(70, 13);
            this.externalIPTextBox.Name = "externalIPTextBox";
            this.externalIPTextBox.ReadOnly = true;
            this.externalIPTextBox.Size = new System.Drawing.Size(503, 20);
            this.externalIPTextBox.TabIndex = 0;
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
            this.SelectServerDialog.Filter = "Executables|*.exe|Batch Files|*.bat|All files|*.*";
            this.SelectServerDialog.Title = "Select the file you want to run, after the server is down";
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.Location = new System.Drawing.Point(142, 133);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(530, 253);
            this.logTextBox.TabIndex = 1000;
            this.logTextBox.Text = "";
            this.logTextBox.WordWrap = false;
            this.logTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.logTextBox_LinkClicked);
            // 
            // singleCheckButton
            // 
            this.singleCheckButton.Location = new System.Drawing.Point(12, 133);
            this.singleCheckButton.Name = "singleCheckButton";
            this.singleCheckButton.Size = new System.Drawing.Size(124, 23);
            this.singleCheckButton.TabIndex = 2;
            this.singleCheckButton.Text = "Validate configuration";
            this.singleCheckButton.UseVisualStyleBackColor = true;
            this.singleCheckButton.Click += new System.EventHandler(this.singleCheckButton_Click);
            // 
            // toggleServerWatcher
            // 
            this.toggleServerWatcher.Location = new System.Drawing.Point(12, 162);
            this.toggleServerWatcher.Name = "toggleServerWatcher";
            this.toggleServerWatcher.Size = new System.Drawing.Size(124, 23);
            this.toggleServerWatcher.TabIndex = 3;
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
            this.intervalTextBox.TabIndex = 4;
            this.intervalTextBox.Text = "60";
            this.intervalTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.intervalTextBox_KeyDown);
            // 
            // setIntervalButton
            // 
            this.setIntervalButton.Location = new System.Drawing.Point(99, 233);
            this.setIntervalButton.Name = "setIntervalButton";
            this.setIntervalButton.Size = new System.Drawing.Size(37, 23);
            this.setIntervalButton.TabIndex = 5;
            this.setIntervalButton.Text = "OK";
            this.setIntervalButton.UseVisualStyleBackColor = true;
            this.setIntervalButton.Click += new System.EventHandler(this.setIntervalButton_Click);
            // 
            // checkUpdate
            // 
            this.checkUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkUpdate.Location = new System.Drawing.Point(12, 363);
            this.checkUpdate.Name = "checkUpdate";
            this.checkUpdate.Size = new System.Drawing.Size(124, 23);
            this.checkUpdate.TabIndex = 8;
            this.checkUpdate.Text = "Check for updates";
            this.checkUpdate.UseVisualStyleBackColor = true;
            this.checkUpdate.Click += new System.EventHandler(this.checkUpdate_Click);
            // 
            // stopServerButton
            // 
            this.stopServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopServerButton.Location = new System.Drawing.Point(12, 305);
            this.stopServerButton.Name = "stopServerButton";
            this.stopServerButton.Size = new System.Drawing.Size(124, 23);
            this.stopServerButton.TabIndex = 7;
            this.stopServerButton.Text = "Stop server";
            this.stopServerButton.UseVisualStyleBackColor = true;
            this.stopServerButton.Click += new System.EventHandler(this.stopServerButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectedPlayersLabel,
            this.connectedPlayersCount,
            this.runtimeLabel,
            this.runtimeValueLabel,
            this.playerCountLabel,
            this.toggleStatisticsButton});
            this.statusStrip1.Location = new System.Drawing.Point(0, 389);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(684, 22);
            this.statusStrip1.TabIndex = 1002;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // connectedPlayersLabel
            // 
            this.connectedPlayersLabel.AutoSize = false;
            this.connectedPlayersLabel.Name = "connectedPlayersLabel";
            this.connectedPlayersLabel.Size = new System.Drawing.Size(120, 17);
            this.connectedPlayersLabel.Text = "Connected Players:";
            this.connectedPlayersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // connectedPlayersCount
            // 
            this.connectedPlayersCount.AutoSize = false;
            this.connectedPlayersCount.Name = "connectedPlayersCount";
            this.connectedPlayersCount.Size = new System.Drawing.Size(25, 17);
            this.connectedPlayersCount.Text = "0";
            this.connectedPlayersCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // runtimeLabel
            // 
            this.runtimeLabel.AutoSize = false;
            this.runtimeLabel.Name = "runtimeLabel";
            this.runtimeLabel.Size = new System.Drawing.Size(75, 17);
            this.runtimeLabel.Text = "Runtime:";
            this.runtimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // runtimeValueLabel
            // 
            this.runtimeValueLabel.AutoSize = false;
            this.runtimeValueLabel.Name = "runtimeValueLabel";
            this.runtimeValueLabel.Size = new System.Drawing.Size(75, 17);
            // 
            // playerCountLabel
            // 
            this.playerCountLabel.Name = "playerCountLabel";
            this.playerCountLabel.Size = new System.Drawing.Size(280, 17);
            this.playerCountLabel.Spring = true;
            // 
            // toggleStatisticsButton
            // 
            this.toggleStatisticsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toggleStatisticsButton.Image = ((System.Drawing.Image)(resources.GetObject("toggleStatisticsButton.Image")));
            this.toggleStatisticsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleStatisticsButton.Name = "toggleStatisticsButton";
            this.toggleStatisticsButton.ShowDropDownArrow = false;
            this.toggleStatisticsButton.Size = new System.Drawing.Size(94, 20);
            this.toggleStatisticsButton.Text = "Enable statistics";
            this.toggleStatisticsButton.Click += new System.EventHandler(this.toggleStatisticsButton_Click);
            // 
            // startServerButton
            // 
            this.startServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startServerButton.Location = new System.Drawing.Point(12, 276);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(124, 23);
            this.startServerButton.TabIndex = 6;
            this.startServerButton.Text = "Start server";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // logFolderDialog
            // 
            this.logFolderDialog.Description = "Select, where you want to store your logfiles. These files contain the data in CS" +
    "V format, so you can analyze them later.";
            // 
            // restartServerButton
            // 
            this.restartServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.restartServerButton.Location = new System.Drawing.Point(12, 334);
            this.restartServerButton.Name = "restartServerButton";
            this.restartServerButton.Size = new System.Drawing.Size(124, 23);
            this.restartServerButton.TabIndex = 7;
            this.restartServerButton.Text = "Restart Server";
            this.restartServerButton.UseVisualStyleBackColor = true;
            this.restartServerButton.Click += new System.EventHandler(this.restartServerButton_Click);
            // 
            // UPnPButton
            // 
            this.UPnPButton.Location = new System.Drawing.Point(579, 88);
            this.UPnPButton.Name = "UPnPButton";
            this.UPnPButton.Size = new System.Drawing.Size(75, 23);
            this.UPnPButton.TabIndex = 9;
            this.UPnPButton.Text = "UPnP";
            this.UPnPButton.UseVisualStyleBackColor = true;
            this.UPnPButton.Click += new System.EventHandler(this.UPnPButton_Click);
            // 
            // FrontEnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 411);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.startServerButton);
            this.Controls.Add(this.restartServerButton);
            this.Controls.Add(this.stopServerButton);
            this.Controls.Add(this.checkUpdate);
            this.Controls.Add(this.setIntervalButton);
            this.Controls.Add(this.intervalTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.timerCountdown);
            this.Controls.Add(this.toggleServerWatcher);
            this.Controls.Add(this.singleCheckButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.configGroupBox);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "FrontEnd";
            this.Text = "CWS Restart";
            this.configGroupBox.ResumeLayout(false);
            this.configGroupBox.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.Button singleCheckButton;
        private System.Windows.Forms.Button toggleServerWatcher;
        private System.Windows.Forms.TextBox actionTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button selectActionButton;
        private System.Windows.Forms.CheckBox CheckLoopbackCheckBox;
        private System.Windows.Forms.CheckBox checkLANCheckBox;
        private System.Windows.Forms.CheckBox checkInternetCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer Watcher;
        private System.Windows.Forms.ProgressBar timerCountdown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox intervalTextBox;
        private System.Windows.Forms.Button setIntervalButton;
        private System.Windows.Forms.Button checkUpdate;
        private System.Windows.Forms.Button stopServerButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel connectedPlayersLabel;
        private System.Windows.Forms.ToolStripStatusLabel connectedPlayersCount;
        private System.Windows.Forms.ToolStripStatusLabel playerCountLabel;
        private System.Windows.Forms.ToolStripDropDownButton toggleStatisticsButton;
        private System.Windows.Forms.ToolStripStatusLabel runtimeLabel;
        private System.Windows.Forms.ToolStripStatusLabel runtimeValueLabel;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.FolderBrowserDialog logFolderDialog;
        private System.Windows.Forms.Button restartServerButton;
        private System.Windows.Forms.Button UPnPButton;
    }
}