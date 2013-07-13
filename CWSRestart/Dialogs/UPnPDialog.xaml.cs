using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for UPnPDialog.xaml
    /// </summary>
    public partial class UPnPDialog : Window, INotifyPropertyChanged
    {
        #region variables
        public event PropertyChangedEventHandler PropertyChanged;

        private bool TCPopen = false;
        private bool UDPopen = false;

        private string ip = "";

        private string status = "Please refresh to begin";
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;
                    notifyPropertyChanged();
                }
            }
        }

        private string buttonText = ":)";
        public string ButtonText
        {
            get
            {
                return buttonText;
            }
            set
            {
                if (buttonText != value)
                {
                    buttonText = value;
                    notifyPropertyChanged();
                }
            }
        }

        private bool toggleEnabled = false;
        public bool ToggleEnabled
        {
            get
            {
                return toggleEnabled;
            }
            set
            {
                if (toggleEnabled != value)
                {
                    toggleEnabled = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        public UPnPDialog(string ip)
        {
            InitializeComponent();
            this.ip = ip;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

            checkOpen();

            if (TCPopen && UDPopen)
            {
                Status = "The ports are open";
                ButtonText = "Close both ports";
                ToggleEnabled = true;
            }
            else if (TCPopen)
            {
                Status = "Only the TCP port is open";
                ButtonText = "Open TCP port";
                ToggleEnabled = true;
            }
            else if (UDPopen)
            {
                Status = "Only the UDP port is open";
                ButtonText = "Open UDP port";
                ToggleEnabled = true;
            }
            else
            {
                Status = "The ports are closed";
                ButtonText = "Open ports";
                ToggleEnabled = true;
            }
        }

            
        private bool checkOpen()
        {
            UDPopen = false;
            TCPopen = false;

            NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
            NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

            if (mappings == null)
            {
                Status = "UPnP is disabled in your router. Try enabling it and hit refresh.";
            }
            else
            {
                foreach (NATUPNPLib.IStaticPortMapping mapping in mappings)
                {
                    if (mapping.InternalClient == ip && mapping.InternalPort == 12345)
                    {
                        switch (mapping.Protocol.ToUpper())
                        {
                            case "UDP":
                                UDPopen = true;
                                break;
                            case "TCP":
                                TCPopen = true;
                                break;
                        }
                    }
                }
            }

            return (TCPopen && UDPopen);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(TCPopen && UDPopen))
            {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

                if (mappings == null)
                {
                    Status = "UPnP is disabled in your router. Try enabling it and hit refresh.";
                }
                else
                {
                    try
                    {
                        mappings.Add(ServerService.Settings.Instance.Port, "UDP", ServerService.Settings.Instance.Port, ip, true, "CubeWorld UDP");
                        mappings.Add(ServerService.Settings.Instance.Port, "TCP", ServerService.Settings.Instance.Port, ip, true, "CubeWorld TCP");

                        RefreshButton_Click(null, null);
                    }
                    catch
                    {
                        Status = "Could not open ports. Is UPnP enabled?";
                    }
                }
            }
            else
            {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

                if (mappings == null)
                {
                    Status = "UPnP is disabled in your router. Try enabling it and hit refresh.";
                }
                else
                {
                    try
                    {
                        if (UDPopen)
                            mappings.Remove(ServerService.Settings.Instance.Port, "UDP");

                        if (TCPopen)
                            mappings.Remove(ServerService.Settings.Instance.Port, "TCP");

                        RefreshButton_Click(null, null);
                    }
                    catch
                    {
                        Status = "Could not close ports. Is UPnP enabled?";
                    }
                }
            }
        }
    }
}
