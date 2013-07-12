using ServerService;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class FrontEnd : Window
    {
        public FrontEnd()
        {
            InitializeComponent();
        }

        private async void RefreshExternalButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Settings.Instance.Internet = await ServerService.Helper.GetExternalIp();
        }

        private async void RefreshLanButton_Click(object sender, RoutedEventArgs e)
        {
            ServerService.Settings.Instance.LAN = await ServerService.Helper.GetLocalIP();
        }

        private void SelectServerButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog selectServer = new Microsoft.Win32.OpenFileDialog();

            selectServer.Filter = "Executables|*.exe|Batch Files|*.bat|All Files|*.*";

            Nullable<bool> result = selectServer.ShowDialog();

            if (result == true)
            {
                ServerService.Settings.Instance.ServerPath = selectServer.FileName;
            }
        }
    }
}
