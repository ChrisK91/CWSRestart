using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace CWSRestartGUI
{
    public partial class FrontEnd : Form
    {
        private int scheduleIntervall = 60000;
        private DateTime lastRun = DateTime.Now;
        private ServerService.Statistics stats;

        public FrontEnd()
        {
            InitializeComponent();

            ServerService.Logging.LogMessage += Logging_LogMessage;

            stats = new ServerService.Statistics(1000, false);
            stats.StatisticsUpdated += stats_StatisticsUpdated;
        }

        void stats_StatisticsUpdated(object sender, ServerService.Statistics.StatisticsUpdatedEventArgs e)
        {
            updateStatistics(e);
        }

        private void updateStatistics(ServerService.Statistics.StatisticsUpdatedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<ServerService.Statistics.StatisticsUpdatedEventArgs>(updateStatistics), e);
            }
            else
            {
                connectedPlayersCount.Text = e.ActivePlayerCount.ToString();
                runtimeValueLabel.Text = String.Format("{0}:{1:00}:{2:00}", Math.Floor(e.Runtime.TotalHours), e.Runtime.Minutes, e.Runtime.Seconds);
                playerCountLabel.Text = String.Format("{0} players in total. RAM usage {1:0}MB (peak: {2:0}MB) - The server was restarted {3} time(s)", e.TotalUniquePlayers, (e.CurrentMemoryUsage / 1024f) / 1024f, (e.PeakMemoryUsage / 1024f) / 1024f, e.RestartCount);
            }
        }

        void Logging_LogMessage(object sender, ServerService.Logging.LogMessageEventArgs e)
        {
            ServerService.Logging.MessageType type = e.type;
            string message = e.message;

            switch (type)
            {
                case ServerService.Logging.MessageType.Error:
                    log(String.Format("{0}: {1}", type.ToString(), message), Color.Red);
                    break;
                case ServerService.Logging.MessageType.Info:
                    log(String.Format("{0}: {1}", type.ToString(), message), Color.Gray);
                    break;
                case ServerService.Logging.MessageType.Warning:
                    log(String.Format("{0}: {1}", type.ToString(), message), Color.Orange);
                    break;
                case ServerService.Logging.MessageType.Server:
                    log(String.Format("{0}: {1}", type.ToString(), message), Color.Green);
                    break;
                default:
                    log(String.Format("{0}: {1}", type.ToString(), message));
                    break;
            }
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
            if (ServerService.Settings.Validate())
            {
                singleCheckButton.Enabled = false;

                await ServerService.Validator.Validates(getAccessScheme());

                singleCheckButton.Enabled = true;
            }
            else
            {
                log("Not all settings are set. Please refresh both of your IPs and select the executable/bat that should be run when the server is dead");
            }
        }

        private void log(string text)
        {
            log(text, SystemColors.WindowText);
        }

        private void log(string text, Color foreground)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, Color>(log), text, foreground);
            }
            else
            {
                logTextBox.SelectionColor = foreground;
                logTextBox.AppendText(String.Format("{0:HH:mm:ss} - {1}{2}", DateTime.Now, text, Environment.NewLine));
                logTextBox.SelectionColor = SystemColors.WindowText;
                logTextBox.ScrollToCaret();
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

            if (Watcher.Enabled)
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
            if (!ServerService.Settings.Validate())
            {
                log("Not all settings are set. Please refresh both of your IPs and select the executable/bat that should be run when the server is dead");

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
                if (timerCountdown.Style != ProgressBarStyle.Continuous)
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
                        stats.IncreaseRestartCount();

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

        private async void checkUpdate_Click(object sender, EventArgs e)
        {
            if (await Updater.UpdateAvailable())
                log("An update is available. Please visit http://chrisk91.github.io/CWSRestart/ to update");
            else
                log("No updates found");
        }

        private void logTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void stopServerButton_Click(object sender, EventArgs e)
        {
            ServerService.Helper.SendQuit();
        }

        private void intervalTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && setIntervalButton.Enabled)
            {
                setIntervalButton_Click(sender, null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void toggleStatisticsButton_Click(object sender, EventArgs e)
        {
            toggleStatisticsButton.Enabled = false;

            if (!stats.Enabled)
            {
                
                bool success = false;

                switch(MessageBox.Show("Would you like to save logs as well?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        if(logFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            stats.LogFolder = logFolderDialog.SelectedPath;
                            success = true;
                        }
                        break;

                    case System.Windows.Forms.DialogResult.No:
                        stats.LogFolder = "";
                        success = true;
                        break;
                }

                if(success)
                {
                    stats.Start();
                    log("Statistics enabled");
                }

            }
            else
            {
                stats.Stop();
                log("Statistics disabled");
            }

            toggleStatisticsButton.Text = stats.Enabled ? "Disable statistics" : "Enable statistics";

            toggleStatisticsButton.Enabled = true;
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            ServerService.Helper.StartServer();
        }

        private void restartServerButton_Click(object sender, EventArgs e)
        {
            ServerService.Helper.RestartServer();
        }

        private void UPnPButton_Click(object sender, EventArgs e)
        {
            if (lanIPTextBox.Text != "")
            {
                PortManager pm = new PortManager(lanIPTextBox.Text);
                pm.ShowDialog();
            }
            else
            {
                log("You need to refresh your LAN IP, in order to use the UPnP wizzard.");
            }
        }
    }
}
