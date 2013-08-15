using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSProtocol
{
    public static class Settings
    {
        public const string SERVERNAME = "CWSRestart";
    }

    namespace Commands
    {
        public enum Action
        {
            GET,
            POST
        }

        public enum Command
        {
            STATISTICS,
            ACK,
            IDENTIFY,
            ENDSTATISTICS,
            START,
            STOP,
            RESTART,
            KILL,
            WATCHER,
            LOG,
            CONNECTED,
            KICK,
            ACCESSMODE,
            ACCESSLIST,
            PRESET,
            PLAYERSDATABASE,
            PLAYERIDENTIFICATION
        }
    }
}
