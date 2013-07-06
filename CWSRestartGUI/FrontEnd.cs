using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWSRestartGUI
{
    public partial class FrontEnd : Form
    {
        private int scheduleIntervall = 60000;
        private DateTime lastRun = DateTime.Now;

        public FrontEnd()
        {
            InitializeComponent();
            ServerService.Logging.LogMessage += Logging_LogMessage;
        }

        void Logging_LogMessage(string message, ServerService.Logging.MessageType type)
        {
            log(String.Format("{0}: {1}", type.ToString(), message));
        }

        private async void refreshExternalIp_Click(object sender, EventArgs e)
        {
            externalIPTextBox.Text = (await ServerService.Helper.GetExternalIp()).ToString();
            ServerService.Settings.Internet = IPAddress.Parse(externalIPTextBox.Text);
        }

        private async void refreshLanIp_Click(object sender, EventArgs e)
        {
           lanIPTextBox.Text = (await ServerService.Helper.GetLocalIP()).ToString();
           ServerService.Settings.LAN = IPAddress.Parse(lanIPTextBox.Text);
        }

        private async void singleCheckButton_Click(object sender, EventArgs e)
        {
            singleCheckButton.Enabled = false;

            await ServerService.Validator.Validates(getAccessScheme());
            
            singleCheckButton.Enabled = true;
        }

        private void log(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(log), text);
            }
            else
            {
                logTextBox.Text = String.Format("{0:HH:mm:ss} - {1}", DateTime.Now, text) + Environment.NewLine + logTextBox.Text;
            }
        }

        private void selectActionButton_Click(object sender, EventArgs e)
        {
            if (SelectServerDialog.ShowDialog() == DialogResult.OK)
            {
                actionTextBox.Text = SelectServerDialog.FileName;
                ServerService.Settings.ServerPath = SelectServerDialog.FileName;
            }
        }

        private void setIntervalButton_Click(object sender, EventArgs e)
        {
            int seconds = 60;
            Int32.TryParse(intervalTextBox.Text, out seconds);

            int intervall = seconds * 1000;
            intervalTextBox.Text = seconds.ToString();
            timerCountdown.Maximum = intervall;
            scheduleIntervall = intervall;

            if(Watcher.Enabled)
                restartTimer();
        }

        private void restartTimer()
        {
            Watcher.Stop();
            Watcher.Start();
            timerCountdown.Value = 0;
            lastRun = DateTime.Now;
        }

        private void toggleServerWatcher_Click(object sender, EventArgs e)
        {
            if (lanIPTextBox.Text == "" || externalIPTextBox.Text == "" || actionTextBox.Text == "")
            {
                log("Please enter both IPs (you can also use the refresh buttons) and select a file that should be run when the server wont respond");
    
            }
            else
            {
                toggleServerWatcher.Enabled = false;

                if (!Watcher.Enabled)
                {
                    restartTimer();
                    toggleServerWatcher.Text = "Stop Watcher";
                    toggleServerWatcher.Enabled = true;
                }
                else
                {
                    Watcher.Stop();
                    timerCountdown.Value = 0;
                    toggleServerWatcher.Text = "Start Watcher";
                    toggleServerWatcher.Enabled = true;
                }
            }
        }

        private async void Watcher_Tick(object sender, EventArgs e)
        {
            if (ServerService.Helper.Working)
            {
                if (timerCountdown.Style != ProgressBarStyle.Marquee)
                    timerCountdown.Style = ProgressBarStyle.Marquee;
                lastRun = DateTime.Now;
            }
            else
            {
                //worse than stopwatch, but better on the CPU
                if(timerCountdown.Style != ProgressBarStyle.Continuous)
                    timerCountdown.Style = ProgressBarStyle.Continuous;

                TimeSpan elapsed = DateTime.Now - lastRun;

                timerCountdown.Value = (elapsed.TotalMilliseconds >= timerCountdown.Maximum) ? timerCountdown.Maximum : (int)elapsed.TotalMilliseconds;

                toggleServerWatcher.Enabled = true;
                setIntervalButton.Enabled = true;


                if (elapsed.TotalMilliseconds >= scheduleIntervall)
                {
                    //stop the timer so we only have one check
                    Watcher.Enabled = false;
                    toggleServerWatcher.Enabled = false;
                    setIntervalButton.Enabled = false;
                    timerCountdown.Style = ProgressBarStyle.Marquee;

                    log("Time to check if the server is still running");

                    ServerService.Validator.ServerErrors access = await ServerService.Validator.Validates(getAccessScheme());

                    //do the magic
                    if (access != 0)
                    {
                        log("A restart is required.");

                        if (!access.HasFlag(ServerService.Validator.ServerErrors.ProcessDead))
                        {
                            ServerService.Helper.RestartServer();
                        }
                        else
                        {
                            ServerService.Helper.StartServer();
                        }
                    }

                    lastRun = DateTime.Now;
                    timerCountdown.Value = 0;


                    //resume the timer
                    Watcher.Enabled = true;
                }
            }
        }

        private ServerService.Settings.AccessType getAccessScheme()
        {
            ServerService.Settings.AccessType scheme = 0;

            if (checkInternetCheckBox.Checked)
                scheme |= ServerService.Settings.AccessType.Internet;

            if (checkLANCheckBox.Checked)
                scheme |= ServerService.Settings.AccessType.LAN;

            if (CheckLoopbackCheckBox.Checked)
                scheme |= ServerService.Settings.AccessType.Loopback;

            return scheme;
        }
    }
}
