using ServerService.Helper;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CWSRestart.Dialogs
{
    /// <summary>
    /// Interaction logic for IPFilter.xaml
    /// </summary>
    public partial class IPFilter : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static string listLocation = Path.Combine(Directory.GetCurrentDirectory(), "iplist.txt");
        public static string ListLocation
        {
            get
            {
                return listLocation;
            }
        }

        private ServerService.Statistics statistics;
        public ServerService.Statistics Statistics
        {
            get
            {
                return statistics;
            }
            private set
            {
                if (statistics != value)
                {
                    statistics = value;
                    notifyPropertyChanged();
                }
            }
        }

        public IPFilter(ref ServerService.Statistics Stats)
        {
            statistics = Stats;

            InitializeComponent();
        }

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AccessListEntry address = ParseText(((ListBoxItem)sender).DataContext.ToString());

            if (address != null)
            {
                if (ServerService.AccessControl.Instance.AccessList.Contains(address))
                    ServerService.AccessControl.Instance.AccessList.Remove(address);
                else
                    ServerService.AccessControl.Instance.AccessList.Add(address);
            }
        }

        private void AddIPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            AccessListEntry ip;
            if (e.Key == Key.Enter && (ip = ParseText(AddIPTextBox.Text)) != null)
            {
                if (!ServerService.AccessControl.Instance.AccessList.Contains(ip))
                    ServerService.AccessControl.Instance.AccessList.Add(ip);

                AddIPTextBox.Text = String.Empty;
                e.Handled = true;
            }
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private AccessListEntry ParseText(string text)
        {
            AccessListEntry e = null;
            if (!AccessIP.TryParse(text, out e))
            {
                if (!AccessIPRange.TryParse(text, out e))
                {
                    return null;
                }
            }
            return e;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.AccessControl.Instance.SaveList(listLocation);
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(listLocation))
                ServerService.AccessControl.Instance.RestoreList(listLocation);
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                IPAddress add = ActivePlayersList.SelectedItem as IPAddress;

                if(add != null)
                    DisconnectWrapper.CloseRemoteIP(add.ToString());
            }
        }
    }
}
