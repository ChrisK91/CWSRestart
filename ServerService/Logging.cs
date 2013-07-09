using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    /// <summary>
    /// A helper class for logging and communicating
    /// </summary>
    public abstract class Logging
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        public enum MessageType
        {
            Info,
            Error,
            Warning,
            Server
        }

        /// <summary>
        /// The handler for an incoming log message
        /// </summary>
        /// <param name="message">The content of the log message</param>
        /// <param name="type">The type of the message</param>
        public delegate void LogMessageEventHandler(object sender, LogMessageEventArgs e);

        public class LogMessageEventArgs : EventArgs
        {
            public string message;
            public MessageType type;
        }

        /// <summary>
        /// Is used to relay logging messages to the hosting application
        /// </summary>
        public static event LogMessageEventHandler LogMessage;

        /// <summary>
        /// Will send the log message
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="type">the type</param>
        internal static void OnLogMessage(string message, MessageType type)
        {
            if (LogMessage != null)
                LogMessage(
                    null,
                    new LogMessageEventArgs
                    {
                        message = message,
                        type = type
                    });
        }
    }
}
