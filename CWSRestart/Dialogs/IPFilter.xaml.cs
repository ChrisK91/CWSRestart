using ServerService.Access;
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
            ListBoxItem lbi = sender as ListBoxItem;

            AccessListEntry address = null;

            if (lbi.DataContext is PlayerInfo)
            {
                PlayerInfo pi = lbi.DataContext as PlayerInfo;
                address = ParseText(pi.Address.ToString());
            }
            else if (lbi.DataContext is AccessListEntry)
            {
                address = lbi.DataContext as AccessListEntry;
            }
            //AccessListEntry address = ParseText(((ListBoxItem)sender).DataContext.ToString());

            if (address != null)
            {
                if (AccessControl.Instance.AccessList.Contains(address))
                    AccessControl.Instance.AccessList.Remove(address);
                else
                    AccessControl.Instance.AccessList.Add(address);
            }
        }

        private void AddIPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            AccessListEntry ip;
            if (e.Key == Key.Enter && (ip = ParseText(AddIPTextBox.Text)) != null)
            {
                if (!AccessControl.Instance.AccessList.Contains(ip))
                    AccessControl.Instance.AccessList.Add(ip);

                AddIPTextBox.Text = String.Empty;
                e.Handled = true;
            }
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private static AccessListEntry ParseText(string text)
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
            AccessControl.Instance.SaveList(listLocation);
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(listLocation))
                AccessControl.Instance.RestoreList(listLocation);
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                PlayerInfo add = ActivePlayersList.SelectedItem as PlayerInfo;

                if(add != null)
                    DisconnectWrapper.CloseRemoteIP(add.Address.ToString());
            }
        }
    }
}
