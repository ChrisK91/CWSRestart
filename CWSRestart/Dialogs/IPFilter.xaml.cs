using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;

namespace CWSRestart.Dialogs
{
    /// <summary>
    /// Interaction logic for IPFilter.xaml
    /// </summary>
    public partial class IPFilter : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
            IPAddress address = ((ListBoxItem)sender).DataContext as IPAddress;

            if (ServerService.AccessControl.Instance.AccessList.Contains(address))
                ServerService.AccessControl.Instance.AccessList.Remove(address);
            else
                ServerService.AccessControl.Instance.AccessList.Add(address);
        }

        private void AddIPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            IPAddress ip;
            if (e.Key == Key.Enter && IPAddress.TryParse(AddIPTextBox.Text, out ip))
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
    }
}
