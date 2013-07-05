using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        }

        private async void refreshExternalIp_Click(object sender, EventArgs e)
        {
            externalIPTextBox.Text = (await ServerService.Helper.GetExternalIp()).ToString();
        }

        private async void refreshLanIp_Click(object sender, EventArgs e)
        {
           lanIPTextBox.Text = (await ServerService.Helper.GetLocalIP()).ToString();
        }

        private async void singleCheckButton_Click(object sender, EventArgs e)
        {
            singleCheckButton.Enabled = false;

            log("Checking server access. This might take one or two minutes. Please be patient");

            try
            {
                if (ServerService.Validator.ProcessRunning())
                    log("The server is running");
                else
                    log("Warning: The process is not running");

                ServerService.Validator.AccessType access = await ServerService.Validator.GetAccessType(lanIPTextBox.Text, externalIPTextBox.Text);

                if (access == 0)
                {
                    log("The server can't be accessed. Please check your firewall configuration");
                }
                else
                {
                    log("The server can be accessed from: " + access.ToString());
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
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
                logTextBox.Text += text + Environment.NewLine;
            }
        }

        private void selectActionButton_Click(object sender, EventArgs e)
        {
            if (SelectServerDialog.ShowDialog() == DialogResult.OK)
                actionTextBox.Text = SelectServerDialog.FileName;
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
            //worse than stopwatch, but better on the CPU
            TimeSpan elapsed = DateTime.Now - lastRun;

            timerCountdown.Value = (elapsed.TotalMilliseconds >= timerCountdown.Maximum) ? timerCountdown.Maximum : (int)elapsed.TotalMilliseconds;

            if (elapsed.TotalMilliseconds >= scheduleIntervall)
            {
                //stop the timer so we only have one check
                Watcher.Enabled = false;
                toggleServerWatcher.Enabled = false;
                setIntervalButton.Enabled = false;
                timerCountdown.Style = ProgressBarStyle.Marquee;

                log("Time to check if the server is still running");

                bool restartRequired = false;
                bool tryquit = false;

                if (ServerService.Validator.ProcessRunning())
                {
                    log("The process is still running. Let's see if it still responds. This might take a while.");

                    try
                    {
                        ServerService.Validator.AccessType access = await ServerService.Validator.GetAccessType(lanIPTextBox.Text, externalIPTextBox.Text);
                        ServerService.Validator.AccessType ignoreFlags = 0;

                        if (ignoreLoopback.Checked)
                            ignoreFlags |= ServerService.Validator.AccessType.Loopback;

                        if (ignoreInternet.Checked)
                            ignoreFlags |= ServerService.Validator.AccessType.Internet;

                        if (ignoreLAN.Checked)
                            ignoreFlags |= ServerService.Validator.AccessType.LAN;

                        log("The access to the following connections will be ignored:");
                        log((ignoreFlags == 0) ? "none" : ignoreFlags.ToString());

                        access = access | ignoreFlags;
                        ServerService.Validator.AccessType required = ServerService.Validator.AccessType.Internet | ServerService.Validator.AccessType.LAN | ServerService.Validator.AccessType.Loopback;

                        if (access != required)
                        {
                            log("The following connections were not available:");
                            log((required ^ access).ToString());
                            restartRequired = true;
                            tryquit = true;
                        }
                        else
                        {
                            log("Everything looks great :)");
                        }
                    }
                    catch(Exception ex)
                    {
                        log(ex.Message);
                    }
                }
                else
                {
                    log("The process has gone :(");
                    restartRequired = true;
                }

                //do the magic
                if (restartRequired)
                {
                    log("A restart is required.");

                    if (tryquit)
                    {
                        log("Trying to send the q key to the server");
                        ServerService.Helper.SendQuit();
                        log("Waiting for 10 seconds to see if the server is shutting down");
                        Thread.Sleep(10000);

                        if(ServerService.Validator.ProcessRunning())
                        {
                            log("The server is still running. Let's force it to quite.");
                            ServerService.Helper.KillServer();
                            log("Waiting for 5 seconds");
                            Thread.Sleep(5000);
                        }
                    }

                    log("Starting server");
                    Process.Start(actionTextBox.Text);
                }

                lastRun = DateTime.Now;
                timerCountdown.Value = 0;

                //resume the timer
                Watcher.Enabled = true;
                toggleServerWatcher.Enabled = true;
                setIntervalButton.Enabled = true;
                timerCountdown.Style = ProgressBarStyle.Continuous;
            }
        }
    }
}
