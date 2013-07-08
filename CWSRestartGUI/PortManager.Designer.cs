namespace CWSRestartGUI
{
    partial class PortManager
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
            this.refreshButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.togglePortButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(12, 83);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(155, 23);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh status";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(12, 9);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(496, 71);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Select an option";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // togglePortButton
            // 
            this.togglePortButton.Enabled = false;
            this.togglePortButton.Location = new System.Drawing.Point(353, 83);
            this.togglePortButton.Name = "togglePortButton";
            this.togglePortButton.Size = new System.Drawing.Size(155, 23);
            this.togglePortButton.TabIndex = 1;
            this.togglePortButton.Text = "Open port";
            this.togglePortButton.UseVisualStyleBackColor = true;
            this.togglePortButton.Click += new System.EventHandler(this.togglePortButton_Click);
            // 
            // PortManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 118);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.togglePortButton);
            this.Controls.Add(this.refreshButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PortManager";
            this.Text = "UPnP Wizzard";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button togglePortButton;
    }
}