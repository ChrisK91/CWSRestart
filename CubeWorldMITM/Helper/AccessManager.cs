using CubeWorldMITM.Networking;
using ServerService.Access;
using ServerService.Access.Entries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utilities.Logging;

namespace CubeWorldMITM.Helper
{
    class AccessManager
    {
        private CWSProtocol.Client client;
        Dictionary<string, MITMMessageHandler> connectedPlayers;
        Dictionary<string, MITMMessageHandler> premiumPlayers;

        Timer interval;

        public bool Working { get; private set; }
        public bool Enabled
        {
            get
            {
                return interval.Enabled;
            }
        }

        public AccessManager(ref Dictionary<string, MITMMessageHandler> ConnectedPlayers, ref Dictionary<string, MITMMessageHandler> PremiumPlayers)
        {
            this.connectedPlayers = ConnectedPlayers;
            this.premiumPlayers = PremiumPlayers;

            client = new CWSProtocol.Client("CubeWorldMITM access list updater");

            interval = new Timer(15000);
            interval.AutoReset = true;
            interval.Elapsed += interval_Elapsed;
        }

        void interval_Elapsed(object sender, ElapsedEventArgs e)
        {
            interval.Enabled = false;
            Working = true;
            if (client.Test())
            {
                AccessControl.Instance.Mode = client.GetAccessMode();
                ObservableCollection<AccessListEntry> list = new ObservableCollection<AccessListEntry>();
                IList<string> entries = client.GetAccessListEntries();

                AccessListEntry entry;

                foreach (string s in entries)
                    if (!String.IsNullOrEmpty(s) && AccessControl.GenerateEntryFromString(s, out entry))
                        list.Add(entry);

                AccessControl.Instance.SetAccessList(list);

                Helper.Settings.Instance.Logger.AddMessage(MessageType.INFO, String.Format("Updated access list with {0} entries. Mode: {1}", list.Count, AccessControl.Instance.Mode.ToString()));

                Dictionary<string, MITMMessageHandler> tmp = new Dictionary<string, MITMMessageHandler>(connectedPlayers);

                foreach (var kvp in tmp)
                {
                    if (!PlayerAllowed(kvp.Key))
                        kvp.Value.Disconnect();
                }

                tmp = new Dictionary<string, MITMMessageHandler>(premiumPlayers);

                foreach (var kvp in tmp)
                {
                    if (!PlayerAllowed(kvp.Key))
                        kvp.Value.Disconnect();
                }
            }
            Working = false;
            interval.Enabled = true;
        }

        public void StartTimer()
        {
            if (!interval.Enabled && client.SetExternalAccesscontrol(true))
            {
                interval.Start();
            }
        }

        public void StopTimer()
        {
            if (interval.Enabled && client.SetExternalAccesscontrol(false))
                interval.Stop();
        }

        public bool PlayerAllowed(string ip)
        {
            switch( AccessControl.Instance.Mode)
            {
                case AccessMode.Whitelist:
                    if(AccessControl.Instance.PlayerInList(ip))
                        return true;
                    return false;

                case AccessMode.Blacklist:
                    if(AccessControl.Instance.PlayerInList(ip))
                        return false;
                    return true;

                default:
                    return true;
            }
        }
    }
}
