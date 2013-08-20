using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    /// <summary>
    /// An interface for loggers
    /// </summary>
    interface ILogger
    {
        /// <summary>
        /// Defines the level that should be logged
        /// </summary>
        MessageType Level { get; set; }

        /// <summary>
        /// Adds the given message to the log
        /// </summary>
        /// <param name="message">The message to add</param>
        void AddMessage(string message);

        /// <summary>
        /// Adds the given message to the log, if Level includes the message type
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        void AddMessage(MessageType type, string message);

        /// <summary>
        /// Adds the given message to the log with a timestamp, if Level includes the message type
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        /// <param name="timestamp">The timestamp when the message occured</param>
        void AddMessage(MessageType type, string message, DateTime timestamp);

        /// <summary>
        /// Adds the given exception to the log
        /// </summary>
        /// <param name="ex">The exception to log</param>
        void AddMessage(Exception ex);

        /// <summary>
        /// Adds the given exception to the log with a timestamp
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        void AddMessage(Exception ex, DateTime timestamp);

        /// <summary>
        /// Adds the given exception to the log, with the calling member name, a timestamp and the type of the message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        /// <param name="member">The calling members name</param>
        /// <param name="type">The message type</param>
        void AddMessage(Exception ex, DateTime timestamp, MessageType type);
    }
}
