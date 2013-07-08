using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWSRestartGUI
{
    public partial class PortManager : Form
    {
        private string ip;

        private bool TCPopen = false;
        private bool UDPopen = false;

        private bool open = false;

        public PortManager(string ip)
        {
            InitializeComponent();
            this.ip = ip;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            checkOpen();

            if (TCPopen && UDPopen)
            {
                statusLabel.Text = "The ports are open";
                togglePortButton.Text = "Close both ports";
                togglePortButton.Enabled = true;

                open = false;
            }
            else if (TCPopen)
            {
                statusLabel.Text = "Only the TCP port is open";
                togglePortButton.Text = "Close TCP port";
                togglePortButton.Enabled = true;

                open = false;
            }
            else if (UDPopen)
            {
                statusLabel.Text = "Only the UDP port is open";
                togglePortButton.Text = "Close UDP port";
                togglePortButton.Enabled = true;

                open = false;
            }
            else
            {
                statusLabel.Text = "The ports are closed";
                togglePortButton.Text = "Open ports";
                togglePortButton.Enabled = true;

                open = true;
            }
        }

        private void checkOpen()
        {
            UDPopen = false;
            TCPopen = false;

            NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
            NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

            if (mappings == null)
            {
                statusLabel.Text = "UPnP is disabled in your router. Try enabling it and hit refresh.";
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
        }

        private void togglePortButton_Click(object sender, EventArgs e)
        {
            if (open)
            {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

                if (mappings == null)
                {
                    statusLabel.Text = "UPnP is disabled in your router. Try enabling it and hit refresh.";
                }
                else
                {
                    try
                    {
                        mappings.Add(ServerService.Settings.Port, "UDP", ServerService.Settings.Port, ip, true, "CubeWorld UDP");
                        mappings.Add(ServerService.Settings.Port, "TCP", ServerService.Settings.Port, ip, true, "CubeWorld TCP");

                        refreshButton_Click(null, null);
                    }
                    catch
                    {
                        statusLabel.Text = "Could not open ports. Is UPnP enabled?";
                    }
                }
            }
            else
            {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;

                if (mappings == null)
                {
                    statusLabel.Text = "UPnP is disabled in your router. Try enabling it and hit refresh.";
                }
                else
                {
                    try
                    {
                        if (UDPopen)
                            mappings.Remove(ServerService.Settings.Port, "UDP");

                        if (TCPopen)
                            mappings.Remove(ServerService.Settings.Port, "TCP");

                        refreshButton_Click(null, null);
                    }
                    catch
                    {
                        statusLabel.Text = "Could not close ports. Is UPnP enabled?";
                    }
                }
            }
        }
    }
}
