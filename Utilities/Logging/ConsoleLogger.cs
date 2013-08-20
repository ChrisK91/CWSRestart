using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    public class ConsoleLogger : ILogger
    {
        private Dictionary<MessageType, ConsoleColor> messageForegroundColors;
        private Dictionary<MessageType, ConsoleColor> messageBackgroundColors;

#if DEBUG
        private MessageType level = Verbosity.Verbose;
#else
        private MessageType level = Verbosity.Minimal;
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
        /// Creates a new instance of the console logger. Output is written to the console
        /// </summary>
        public ConsoleLogger()
        {
            messageForegroundColors = new Dictionary<MessageType, ConsoleColor>();
            messageBackgroundColors = new Dictionary<MessageType, ConsoleColor>();

            messageForegroundColors.Add(MessageType.INFO, ConsoleColor.Gray);
            messageForegroundColors.Add(MessageType.WARNING, ConsoleColor.Yellow);
            messageForegroundColors.Add(MessageType.ERROR, ConsoleColor.Red);
            messageForegroundColors.Add(MessageType.DEBUG, ConsoleColor.DarkBlue);

            messageBackgroundColors.Add(MessageType.DEBUG, ConsoleColor.White);
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
            }
        }

        private void appendLine(string line, MessageType type)
        {
            ConsoleColor oldForeground = Console.ForegroundColor;
            ConsoleColor oldBackground = Console.BackgroundColor;

            if (messageForegroundColors.ContainsKey(type))
                Console.ForegroundColor = messageForegroundColors[type];

            if (messageBackgroundColors.ContainsKey(type))
                Console.BackgroundColor = messageBackgroundColors[type];

            Console.WriteLine(line);

            Console.BackgroundColor = oldBackground;
            Console.ForegroundColor = oldForeground;
        }

        public static void Beep()
        {
            ConsoleColor oldForeground = Console.ForegroundColor;
            ConsoleColor oldBackground = Console.BackgroundColor;
            Console.Beep(658, 125); recolor(); Console.Beep(1320, 500); recolor(); Console.Beep(990, 250); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(1188, 250); recolor(); Console.Beep(1320, 125); recolor(); Console.Beep(1188, 125); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(990, 250); recolor(); Console.Beep(880, 500); recolor(); Console.Beep(880, 250); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(1320, 500); recolor(); Console.Beep(1188, 250); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(990, 750); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(1188, 500); recolor(); Console.Beep(1320, 500); recolor(); Console.Beep(1056, 500); recolor(); Console.Beep(880, 500); recolor(); Console.Beep(880, 500); Thread.Sleep(250); recolor(); Console.Beep(1188, 500); recolor(); Console.Beep(1408, 250); recolor(); Console.Beep(1760, 500); recolor(); Console.Beep(1584, 250); recolor(); Console.Beep(1408, 250); recolor(); Console.Beep(1320, 750); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(1320, 500); recolor(); Console.Beep(1188, 250); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(990, 500); recolor(); Console.Beep(990, 250); recolor(); Console.Beep(1056, 250); recolor(); Console.Beep(1188, 500); recolor(); Console.Beep(1320, 500); recolor(); Console.Beep(1056, 500); recolor(); Console.Beep(880, 500); recolor(); Console.Beep(880, 500); 
            Console.BackgroundColor = oldBackground;
            Console.ForegroundColor = oldForeground;
            Console.Clear();
        }

        private static void recolor()
        {
            Random r = new Random();
            Array values = Enum.GetValues(typeof(ConsoleColor));
            Random random = new Random();
            Console.BackgroundColor = (ConsoleColor)values.GetValue(random.Next(values.Length));
            Console.Clear();
        }
    }
}
