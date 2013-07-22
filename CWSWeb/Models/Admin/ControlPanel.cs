using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Models.Admin
{
    public class ControlPanel
    {
        public List<string> LogMessages { get; private set; }
        
        public bool ServerAlive {
            get {
                return Helper.CachedVariables.Stats != null ? Helper.CachedVariables.Stats.IsAlive : false;
            }
        }

        public bool WatcherEnabled { get; private set; }
        public bool WatcherBusy { get; private set; }
        public uint WatcherTimeout { get; private set; }

        public ControlPanel(List<string> LogMessages, bool WatcherEnabled, bool WatcherBusy, uint WatcherTimeout)
        {
            this.LogMessages = LogMessages;
            this.WatcherBusy = WatcherBusy;
            this.WatcherTimeout = WatcherTimeout;
            this.WatcherEnabled = WatcherEnabled;
        }
    }
}
