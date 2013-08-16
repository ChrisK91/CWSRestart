using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace CWSRestart.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectAdditionalProcesses.xaml
    /// </summary>
    public partial class SelectAdditionalProcesses : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> currentProcesses;
        public List<string> CurrentProcesses
        {
            get
            {
                return currentProcesses;
            }
            private set
            {
                if (currentProcesses != value)
                {
                    currentProcesses = value;
                    notifyPropertyChanged();
                }
            }
        }
    

        public SelectAdditionalProcesses()
        {
            InitializeComponent();

            CurrentProcesses = new List<string>();
            Process[] active = Process.GetProcesses();

            foreach (Process p in active)
                CurrentProcesses.Add(p.ProcessName);

            CurrentProcesses.Sort();
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string processName = ((ListBoxItem)sender).Content.ToString();

            if (ServerService.Helper.Settings.Instance.AdditionalProcesses.Contains(processName))
                ServerService.Helper.Settings.Instance.AdditionalProcesses.Remove(processName);
            else
                ServerService.Helper.Settings.Instance.AdditionalProcesses.Add(processName);
        }

        private void ProcessNameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(ProcessNameTextbox.Text) && e.Key == Key.Enter)
            {
                if (!ServerService.Helper.Settings.Instance.AdditionalProcesses.Contains(ProcessNameTextbox.Text))
                    ServerService.Helper.Settings.Instance.AdditionalProcesses.Add(ProcessNameTextbox.Text);
                e.Handled = true;
            }
        }
    }
}
