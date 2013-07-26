using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSProtocol
{
    public static class Configuration
    {
        public const string SERVERNAME = "CWSRestart";
    }

    public static class Commands
    {
        public enum Actions
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
            ACCESSLIST
        }
    }
}
