using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    /// <summary>
    /// Contains information about the importance of log messages
    /// </summary>
    [Flags]
    public enum MessageType
    {
        /// <summary>
        /// The message is purely informative
        /// </summary>
        INFO = 0x01,
        /// <summary>
        /// The message contains details about an error
        /// </summary>
        ERROR = 0x02,
        /// <summary>
        /// The message contains a warning
        /// </summary>
        WARNING = 0x04,
        /// <summary>
        /// The message contains debug information
        /// </summary>
        DEBUG = 0x08
    }

    /// <summary>
    /// Provides presets for logging levels
    /// </summary>
    public static class Verbosity
    {
        /// <summary>
        /// Every message will be logged
        /// </summary>
        public const MessageType Verbose = MessageType.DEBUG | MessageType.ERROR | MessageType.INFO | MessageType.WARNING;

        /// <summary>
        /// Everything, except debug information will be logged
        /// </summary>
        public const MessageType Detailed = MessageType.INFO | MessageType.ERROR | MessageType.WARNING;

        /// <summary>
        /// Only warnings and errors are logged
        /// </summary>
        public const MessageType Minimal = MessageType.ERROR | MessageType.WARNING;

        /// <summary>
        /// Only errors are logged
        /// </summary>
        public const MessageType Quiet = MessageType.ERROR;

        /// <summary>
        /// Nothing should be logged
        /// </summary>
        public const MessageType None = 0x0;
    }
}
