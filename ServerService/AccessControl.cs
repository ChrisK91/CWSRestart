using ServerService.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        private ObservableCollection<AccessListEntry> accessList = new ObservableCollection<AccessListEntry>();
        public ObservableCollection<AccessListEntry> AccessList
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
            if (AccessList.Count > 0)
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] connectionInformation = ipGlobalProperties.GetActiveTcpConnections();

                IEnumerator enumerator = connectionInformation.GetEnumerator();

                Logging.OnLogMessage(String.Format("Enforcing {0}", Mode), Logging.MessageType.Info);

                while (enumerator.MoveNext())
                {
                    TcpConnectionInformation info = (TcpConnectionInformation)enumerator.Current;

                    if (info.LocalEndPoint.Port == Settings.Instance.Port && info.State == TcpState.Established)
                    {
                        int ret = 0;
                        bool actionTaken = false;
                        bool playerFound = PlayerInList(info);

                        switch (Mode)
                        {
                            case AccessMode.Whitelist:

                                if(!playerFound)
                                {
                                    ret = Helper.DisconnectWrapper.CloseRemoteIP(info.RemoteEndPoint.Address.ToString());
                                    actionTaken = true;
                                }
                                
                                break;

                            case AccessMode.Blacklist:

                                if(playerFound)
                                {
                                    ret = Helper.DisconnectWrapper.CloseRemoteIP(info.RemoteEndPoint.Address.ToString());
                                    actionTaken = true;
                                }

                                break;
                        }

                        if (actionTaken && (Helper.UacHelper.IsProcessElevated == Helper.UacHelper.IsUacEnabled))
                        {
                            Logging.OnLogMessage(String.Format("{0} should now be kicked.", info.RemoteEndPoint.ToString()), Logging.MessageType.Info);
                        }
                        else if (actionTaken)
                        {
                            Logging.OnLogMessage(String.Format("{0} could not be kicked.", info.RemoteEndPoint.ToString()), Logging.MessageType.Warning);
                        }
                    }
                }
            }
        }

        public bool PlayerInList(TcpConnectionInformation info)
        {
            foreach (AccessListEntry e in accessList)
            {
                if (e.Matches(info.RemoteEndPoint.Address))
                {
                    return true;
                }
            }
            return false;
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

        public void SaveList(string filepath)
        {
            StringBuilder sb = new StringBuilder();

            foreach (AccessListEntry e in AccessList)
                sb.AppendLine(e.ToString());

            File.WriteAllText(filepath, sb.ToString());
        }

        public void RestoreList(string filepath)
        {
            using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                AccessList = new ObservableCollection<AccessListEntry>();
                StreamReader sr = new StreamReader(fs);
                
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    AccessListEntry e;
                    if (GenerateEntryFromString(line, out e))
                        if (!AccessList.Contains(e))
                            AccessList.Add(e);
                }

                sr.Close();
            }
        }

        public static bool GenerateEntryFromString(string s, out AccessListEntry target)
        {
            if (!AccessIP.TryParse(s, out target))
                if (!AccessIPRange.TryParse(s, out target))
                    return false;

            return true;
        }
    }
}
