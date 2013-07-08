using System;
using System.IO;
using System.Net;

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

    /// <summary>
    /// This class contains all settings for the Server Service
    /// </summary>
    public abstract class Settings
    {
        /// <summary>
        /// A service that can be used to retrieve the current (global) IP
        /// </summary>
        public static Uri IPService = new Uri("http://bot.whatismyipaddress.com/");

        /// <summary>
        /// The Loopback IP
        /// </summary>
        public static IPAddress Loopback = IPAddress.Loopback;

        /// <summary>
        /// The port on which the server is listening
        /// </summary>
        public static int Port = 12345;

        /// <summary>
        /// The local network IP
        /// </summary>
        public static IPAddress LAN;

        /// <summary>
        /// The global IP
        /// </summary>
        public static IPAddress Internet;

        /// <summary>
        /// The server process name
        /// </summary>
        public static string ServerProcessName = "Server";

        /// <summary>
        /// The timeout to wait for the server to quit, before we try killing it
        /// </summary>
        public static int Timeout = 10000;


        /// <summary>
        /// The location of the server exectuable
        /// </summary>
        public static string ServerPath = "";

        /// <summary>
        /// Describes some access locations
        /// </summary>
        [Flags]
        public enum AccessType
        {
            /// <summary>
            /// Access on the loopback network (120.0.0.1/localhost)
            /// </summary>
            Loopback = 0x1,
            /// <summary>
            /// Access in the LAN (eg. 192.168.*.*)
            /// </summary>
            LAN = 0x2,
            /// <summary>
            /// Access from the world wide web
            /// </summary>
            Internet = 0x4
        }


        /// <summary>
        /// Check if all settings are set
        /// </summary>
        /// <returns>True if everything is fine</returns>
        public static bool Validate()
        {
            if(File.Exists(ServerPath) && (LAN != null) && (Internet != null))
                return true;

            return false;
        }
    }
}
