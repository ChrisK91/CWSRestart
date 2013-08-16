using CWSRestart.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSRestart.Helper
{
    public class LogMessage
    {
        public DateTime Timestamp { get; private set; }
        public MessageType MessageType { get; private set; }
        public string Message { get; private set; }

        public LogMessage(string Message)
        {
            initalize(DateTime.Now, MessageType.General, Message);
        }

        public LogMessage(string Message, DateTime Timestamp)
        {
            initalize(Timestamp, MessageType.General, Message);
        }

        public LogMessage(string Message, MessageType MessageType)
        {
            initalize(DateTime.Now, MessageType, Message);
        }

        public LogMessage(string Message, DateTime Timestamp, MessageType MessageType)
        {
            initalize(Timestamp, MessageType, Message);
        }

        private void initalize(DateTime timestamp, MessageType messageType, string message)
        {
            Timestamp = timestamp;
            MessageType = messageType;
            Message = message;
        }
    }

    public enum MessageType
    {
        Server,
        Info,
        Warning,
        Error,
        General
    }
}
