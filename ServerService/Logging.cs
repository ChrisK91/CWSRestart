using System;

namespace ServerService
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

    public class LogMessageEventArgs : EventArgs
    {
        public string message { get; private set; }
        public MessageType type { get; private set; }

        public LogMessageEventArgs(string message, MessageType type)
        {
            this.message = message;
            this.type = type;
        }
    }

    /// <summary>
    /// A helper class for logging and communicating
    /// </summary>
    public abstract class Logging
    {
        /// <summary>
        /// Is used to relay logging messages to the hosting application
        /// </summary>
        public static event EventHandler<LogMessageEventArgs> LogMessage;

        /// <summary>
        /// Will send the log message
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="type">the type</param>
        internal static void OnLogMessage(string message, MessageType type)
        {
            if (LogMessage != null)
                LogMessage(null, new LogMessageEventArgs(message, type));
        }
    }
}
