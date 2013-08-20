using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    public class FileLogger : ILogger
    {
        private string file;

#if DEBUG
        private MessageType level = Verbosity.Verbose;
#else
        private MessageType level = Verbosity.Detailed;
#endif
        /// <summary>
        /// Gets or sets the message types, that should be written to the output log
        /// </summary>
        public MessageType Level
        {
            get
            {
                return this.level;
            }
            set
            {
                if (this.level != value)
                    this.level = value;
            }
        }

        /// <summary>
        /// Creates a new instance of a file logger. Messages are written to the output file.
        /// </summary>
        /// <param name="file"></param>
        public FileLogger(string file)
        {
            this.file = file;

            if (!File.Exists(file))
            {
                // Wrapped in using directive to dispose the created stream. We could also call .Dispose() instead.
                using (File.Create(file))
                { }
            }
        }

        /// <summary>
        /// Adds the given message to the log
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessage(string message)
        {
            AddMessage(MessageType.INFO, message);
        }

        /// <summary>
        /// Adds the given message to the log, if Level includes the message type
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        public void AddMessage(MessageType type, string message)
        {
            AddMessage(type, message, DateTime.Now);
        }

        /// <summary>
        /// Adds the given message to the log with a timestamp, if Level includes the message type
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        /// <param name="timestamp">The timestamp when the message occured</param>
        public void AddMessage(MessageType type, string message, DateTime timestamp)
        {
            if (Level.HasFlag(type))
            {
                string line = String.Format("{0:yyyy-MM-dd HH:mm:ss} - {1}:{2}", timestamp, type.ToString(), message);
                appendLine(line);
            }
        }

        /// <summary>
        /// Adds the given exception to the log
        /// </summary>
        /// <param name="ex">The exception to log</param>
        public void AddMessage(Exception ex)
        {
            AddMessage(ex, DateTime.Now);
        }

        /// <summary>
        /// Adds the given exception to the log with a timestamp
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        public void AddMessage(Exception ex, DateTime timestamp)
        {
            AddMessage(ex, DateTime.Now, MessageType.ERROR);
        }

        /// <summary>
        /// Adds the given exception to the log, with the calling member name, a timestamp and the type of the message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        /// <param name="member">The calling members name</param>
        /// <param name="type">The message type</param>
        public void AddMessage(Exception ex, DateTime timestamp, MessageType type)
        {
            if (Level.HasFlag(type))
            {
                StringBuilder sb = new StringBuilder();

                string line = String.Format("{0:yyyy-MM-dd HH:mm:ss} - {1}:{2} in {3} ({4})", timestamp, type.ToString(), ex.ToString(), ex.TargetSite.Name, ex.Message);
                sb.AppendLine(line);

                Exception tmp = ex;

                while (tmp.InnerException != null)
                {
                    tmp = tmp.InnerException;
                }

                appendLine(sb.ToString());
            }
        }

        private void appendLine(string line)
        {
            using (StreamWriter w = File.AppendText(file))
            {
                w.WriteLineAsync(line);
            }
        }
    }
}
