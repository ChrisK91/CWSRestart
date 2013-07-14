using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    public sealed class AccessControl : INotifyPropertyChanged
    {
        public enum AccessMode
        {
            Whitelist,
            Blacklist
        }

        private static readonly AccessControl instance = new AccessControl();
        public event PropertyChangedEventHandler PropertyChanged;

        public static AccessControl Instance
        {
            get
            {
                return instance;
            }
        }

        private ObservableCollection<IPAddress> accessList = new ObservableCollection<IPAddress>();
        public ObservableCollection<IPAddress> AccessList
        {
            get
            {
                return accessList;
            }
            set
            {
                if (accessList != value)
                {
                    accessList = value;
                    notifyPropertyChanged();
                }
            }
        }

        public void Enforce()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connectionInformation = ipGlobalProperties.GetActiveTcpConnections();

            IEnumerator enumerator = connectionInformation.GetEnumerator();

            while (enumerator.MoveNext())
            {
                TcpConnectionInformation info = (TcpConnectionInformation)enumerator.Current;

                if (info.LocalEndPoint.Port == 12345 && info.State == TcpState.Established)
                {
                    int ret = 0;
                    bool actionTaken = false;

                    switch(Mode)
                    {
                        case AccessMode.Whitelist:

                            if (!AccessList.Contains(info.RemoteEndPoint.Address))
                            {
                                //Player not on whitelist
                                ret = Helper.DisconnectWrapper.CloseRemoteIP(info.RemoteEndPoint.Address.ToString());
                                actionTaken = true;
                            }

                            break;

                        case AccessMode.Blacklist:

                            if (AccessList.Contains(info.RemoteEndPoint.Address))
                            {
                                //Player on blacklist
                                ret = Helper.DisconnectWrapper.CloseRemoteIP(info.RemoteEndPoint.Address.ToString());
                                actionTaken = true;
                            }

                            break;
                    }

                    if (actionTaken && (Helper.UacHelper.IsProcessElevated == Helper.UacHelper.IsUacEnabled))
                    {
                        Logging.OnLogMessage(String.Format("{0} should now be kicked.", info.RemoteEndPoint.ToString()), Logging.MessageType.Info);
                    }
                    else
                    {
                        Logging.OnLogMessage(String.Format("{0} could not be kicked.", info.RemoteEndPoint.ToString()), Logging.MessageType.Warning);
                    }
                }
            }
        }

        private AccessMode mode = AccessMode.Blacklist;
        public AccessMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    notifyPropertyChanged();
                }
            }
        }

        private AccessControl()
        {
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
