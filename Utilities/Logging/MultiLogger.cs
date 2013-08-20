using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    /// <summary>
    /// Provides support for multiple loggers.
    /// </summary>
    public class MultiLogger : ILogger
    {
        /// <summary>
        /// An internal list of the current loggers
        /// </summary>
        private IList<ILogger> loggers;

        public IReadOnlyList<ILogger> Loggers
        {
            get
            {
                return new List<ILogger>(loggers); 
            }
        }

        /// <summary>
        /// Level is not supported by the MultiLogger
        /// </summary>
        public MessageType Level
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public MultiLogger()
        {
            loggers = new List<ILogger>();
        }

        /// <summary>
        /// Adds a new logger to the underlying logs
        /// </summary>
        /// <param name="logger">The logger that should be added</param>
        public void Add(ILogger logger)
        {
            loggers.Add(logger);
        }

        /// <summary>
        /// Removes an existing logger from the underlying logs
        /// </summary>
        /// <param name="logger">The logger that should be removed</param>
        public void Remove(ILogger logger)
        {
            loggers.Remove(logger);
        }

        /// <summary>
        /// Adds the given message to the underlying logs
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessage(string message)
        {
            AddMessage(MessageType.INFO, message);
        }

        /// <summary>
        /// Adds the given message to the underlying logs
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        public void AddMessage(MessageType type, string message)
        {
            AddMessage(type, message, DateTime.Now);
        }

        /// <summary>
        /// Adds the given message to the underlying logs with a timestamp
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="message">The content of the message</param>
        /// <param name="timestamp">The timestamp when the message occured</param>
        public void AddMessage(MessageType type, string message, DateTime timestamp)
        {
            foreach (ILogger l in loggers)
                l.AddMessage(type, message, timestamp);
        }

        /// <summary>
        /// Adds the given exception to the underlying logs
        /// </summary>
        /// <param name="ex">The exception to log</param>
        public void AddMessage(Exception ex)
        {
            AddMessage(ex, DateTime.Now);
        }

        /// <summary>
        /// Adds the given exception to the underlying logs with a timestamp
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        public void AddMessage(Exception ex, DateTime timestamp)
        {
            AddMessage(ex, timestamp, MessageType.INFO);
        }

        /// <summary>
        /// Adds the given exception to the underlying logs, with the calling member name, a timestamp and the type of the message
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="timestamp">The timestamp</param>
        /// <param name="member">The calling members name</param>
        /// <param name="type">The message type</param>
        public void AddMessage(Exception ex, DateTime timestamp, MessageType type)
        {
            foreach (ILogger l in loggers)
                l.AddMessage(ex, timestamp, type);
        }
    }
}
