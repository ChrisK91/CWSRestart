using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public class PlayerInfo : INotifyPropertyChanged
    {
        public IPAddress Address { get; private set; }

        private DateTime lastUpdate;
        private TimeSpan updateInterval = new TimeSpan(0, 0, 5);

        private string friendlyName;
        public string FriendlyName
        {
            get
            {
                if ((friendlyName == null || lastUpdate.Add(updateInterval) <= DateTime.Now) && Helper.Settings.Instance.KnownPlayers != null)
                    updateFriendlyNameAsync();
                return friendlyName ?? Address.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PlayerInfo(IPAddress address)
        {
            this.Address = address;
            notifyPropertyChanged("Address");
        }

        protected void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
            notifyPropertyChanged("FriendlyName");
        }
    }
}
