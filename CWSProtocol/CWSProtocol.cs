using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSProtocol
{
    /// <summary>
    /// Configuration parameters for CWSProtocol
    /// </summary>
    public static class Settings
    {
        public const string SERVERNAME = "CWSRestart";
    }
    
    namespace Commands
    {
        /// <summary>
        /// Command actions
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Retreives an information from CWSRestart
            /// </summary>
            GET,
            /// <summary>
            /// Sends content to CWSRestart
            /// </summary>
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
            PLAYERIDENTIFICATION,
            PREMIUMSLOTS,
            PREMIUMDATABASE,
            EXTERNALACCESSCONTROL
        }
    }
}
