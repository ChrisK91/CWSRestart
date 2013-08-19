using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Access.Entries
{
    /// <summary>
    /// Provides access list support for single IP
    /// </summary>
    public class AccessIP : AccessListEntry
    {
        /// <summary>
        /// The underlying IP
        /// </summary>
        public IPAddress Address { get; private set; }

        /// <summary>
        /// Instanciates an access list entry for the given IP
        /// </summary>
        /// <param name="address">The IP for the entry</param>
        public AccessIP(IPAddress address)
        {
            Address = address;
        }

        private string friendlyName = null;
        private DateTime lastUpdate;
        private TimeSpan updateInterval = new TimeSpan(0, 0, 5);

        /// <summary>
        /// Contains the IP and the Playername, if identification is enabled
        /// </summary>
        public override string FriendlyName
        {
            get {
                if((friendlyName == null || lastUpdate.Add(updateInterval) <= DateTime.Now) && Helper.Settings.Instance.KnownPlayers != null)
                    updateFriendlyNameAsync();
                return friendlyName ?? Address.ToString();
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is AccessIP)
            {
                return ((AccessIP)obj).Address.Equals(Address);
            }

            return false;
        }

        private async void updateFriendlyNameAsync()
        {
            if (Helper.Settings.Instance.KnownPlayers != null)
            {
                string onlineName = await Helper.Settings.Instance.KnownPlayers.GetConnectedPlayerNameAsync(Address.ToString());

                if (onlineName != null)
                {
                    friendlyName = String.Format("{0} - {1} (online)");
                }
                else
                {
                    List<string> knownNames = await Helper.Settings.Instance.KnownPlayers.GetKnownNamesAsync(Address.ToString());

                    if (knownNames.Count > 0)
                    {
                        friendlyName = String.Format("{0} - {1}", Address.ToString(), String.Join(", ", knownNames));
                    }
                    else
                    {
                        friendlyName = Address.ToString();
                    }
                }
            }

            lastUpdate = DateTime.Now;
            NotifyPropertyChanged("FriendlyName");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Address.ToString();
        }

        public override bool Matches(System.Net.IPAddress target)
        {
            return Address.Equals(target);
        }

        /// <summary>
        /// Parses the given string to a new AccessIP
        /// </summary>
        /// <param name="source">A string representation of the entry</param>
        /// <param name="target"></param>
        /// <returns>True if parsing was succesful, otherwise false</returns>
        public static bool TryParse(string source, out AccessIP target)
        {
            IPAddress address;

            if (IPAddress.TryParse(source, out address))
            {
                target = new AccessIP(address);
                return true;
            }

            target = null;
            return false;
        }
    }
}
