using ServerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CWSRestart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FrontEnd : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Statistics stats;
        public Statistics Stats
        {
            get
            {
                return stats;
            }
            private set
            {
                if (stats != value)
                {
                    stats = value;
                    notifyPropertyChanged();
                }
            }
        }

        public FrontEnd()
        {
            InitializeComponent();
            ServerService.Logging.LogMessage += Logging_LogMessage;
            Helper.Logging.LogMessage += Logging_LogMessage;

            Stats = new Statistics(1000, false);
            Infrastructure.Server.Instance.Statistics = Stats;

            Infrastructure.Server.Instance.GetLog = () =>
            {
                return LogControl.Messages.ToList();
            };

            Infrastructure.Server.Instance.ClearLog = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    LogControl.ClearLog();
                });
            };

            if (File.Exists(Dialogs.IPFilter.ListLocation))
                AccessControl.Instance.RestoreList(Dialogs.IPFilter.ListLocation);

            if (File.Exists(ServerService.Helper.Settings.Instance.AdditionalProcessesLocation))
                ServerService.Helper.Settings.Instance.RestoreAdditionalProcceses(ServerService.Helper.Settings.Instance.AdditionalProcessesLocation);

            if (!ServerService.Helper.UacHelper.IsProcessElevated && ServerService.Helper.UacHelper.IsUacEnabled)
            {
                Helper.Logging.OnLogMessage("The application is not running as administrator. Features like banning might not work.", Logging.MessageType.Warning);
            }
            else
            {
                Helper.Logging.OnLogMessage("The application is running as administrator. Keep in mind, that everything that is launched from here, will run as administrator as well.", Logging.MessageType.Info);
            }
#if DEBUG
            ToggleInterProcessCommunication_Click(null, null);
#endif

            if (Helper.Settings.Instance.AutostartCWSProtocol)
            {
                ToggleInterProcessCommunication_Click(null, null);
            }

            if (Helper.Settings.Instance.AutostartStatistics)
            {
                ToggleStatisticsButton_Click(null, null);
            }

            if (Helper.Settings.Instance.AutostartWatcher)
            {
                if (ServerService.Helper.Settings.Instance.Validates)
                    ToggleWatcher_Click(null, null);
                else
                    Helper.Logging.OnLogMessage("Not all settings are set. Watcher not started.", Logging.MessageType.Warning);
            }
        }

        void Logging_LogMessage(object sender, Logging.LogMessageEventArgs e)
        {
            CWSRestart.Controls.LogFilter.MessageType t = Controls.LogFilter.MessageType.General;

            switch (e.type)
            {
                case Logging.MessageType.Error:
                    t = Controls.LogFilter.MessageType.Error;
                    break;

                case Logging.MessageType.Info:
                    t = Controls.LogFilter.MessageType.Info;
                    break;

                case Logging.MessageType.Server:
                    t = Controls.LogFilter.MessageType.Server;
                    break;

                case Logging.MessageType.Warning:
                    t = Controls.LogFilter.MessageType.Warning;
                    break;
            }

            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<Controls.LogFilter.LogMessage>((m) => LogControl.Messages.Add(m)), new Controls.LogFilter.LogMessage(e.message, t));
            }
            catch (NullReferenceException)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        private void log(string message)
        {
            LogControl.Messages.Add(new Controls.LogFilter.LogMessage(message, Controls.LogFilter.MessageType.Info));
        }

        private async void RefreshExternalButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Helper.Settings.Instance.Internet = await ServerService.Helper.General.GetExternalIp();
        }

        private async void RefreshLanButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Helper.Settings.Instance.LAN = await ServerService.Helper.General.GetLocalIP();
        }

        private void SelectServerButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog selectServer = new Microsoft.Win32.OpenFileDialog();

            selectServer.Filter = "Executables|*.exe|Batch Files|*.bat|All Files|*.*";

            Nullable<bool> result = selectServer.ShowDialog();

            if (result == true)
            {
                ServerService.Helper.Settings.Instance.ServerPath = selectServer.FileName;
            }
        }

        private async void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            await ServerService.Validator.Instance.Validates();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (await Helper.Updater.UpdateAvailable())
            {
                log("A new version is available. Please visit the website to update");
            }
            else
            {
                log("The version your are using is up to date :)");
            }
        }

        private void IntervallTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IntervallTextbox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                e.Handled = true;
            }
        }

        private void ToggleWatcher_Click(object sender, RoutedEventArgs e)
        {
            Helper.Watcher.Instance.Toggle();
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Helper.General.StartServer();
        }

        private void StopServerButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Helper.General.SendQuit();
        }

        private void RestartServerButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Helper.General.RestartServer();
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Stats.Enabled)
            {
                Stats.Stop();
            }
            else
            {
                Stats.Start();
            }
        }

        private void StatsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog logFolderDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (logFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stats.LogFolder = logFolderDialog.SelectedPath;
            }
        }

        private void UPnPButton_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.UPnPDialog dlg = new Dialogs.UPnPDialog(ServerService.Helper.Settings.Instance.LAN.ToString());
            dlg.ShowDialog();
        }

        private void AdditionalProcesses_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.SelectAdditionalProcesses dlg = new Dialogs.SelectAdditionalProcesses();
            dlg.ShowDialog();
        }

        private void IpFilterButton_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.IPFilter dlg = new Dialogs.IPFilter(ref stats);
            dlg.Show();
        }

        private void ToggleInterProcessCommunication_Click(object sender, RoutedEventArgs e)
        {
            Infrastructure.Server.Instance.ToggleServer();
        }

        private void FrontEndControl_Closing(object sender, CancelEventArgs e)
        {
            if (Infrastructure.Server.Instance.IsRunning)
                Infrastructure.Server.Instance.ToggleServer();

            AccessControl.Instance.SaveList(Dialogs.IPFilter.ListLocation);
            ServerService.Helper.Settings.Instance.SaveAdditionalProcesses(ServerService.Helper.Settings.Instance.AdditionalProcessesLocation);
        }
    }
}
