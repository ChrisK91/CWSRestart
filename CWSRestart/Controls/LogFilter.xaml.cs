using CWSRestart.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CWSRestart.Controls
{
    public partial class LogFilter : UserControl, INotifyPropertyChanged
    {
        #region messages
        LimitedObservableCollection<LogMessage> messages;
        public LimitedObservableCollection<LogMessage> Messages
        {
            get
            {
                return messages;
            }
        }

        ICollectionView log;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region filter
        private bool hideServer;
        private bool hideGeneral;
        private bool hideInfo;
        private bool hideError;
        private bool hideWarning;

        public bool filterServer(object o)
        {
            LogMessage m = o as LogMessage;
            if ((m.MessageType == MessageType.Server) && (hideServer == true))
                return false;
            return true;
        }

        public bool filterInfo(object o)
        {
            LogMessage m = o as LogMessage;
            if ((m.MessageType == MessageType.Info) && (hideInfo == true))
                return false;
            return true;
        }

        public bool filterGeneral(object o)
        {
            LogMessage m = o as LogMessage;
            if ((m.MessageType == MessageType.General) && (hideGeneral == true))
                return false;
            return true;
        }

        public bool filterError(object o)
        {
            LogMessage m = o as LogMessage;
            if ((m.MessageType == MessageType.Error) && (hideError == true))
                return false;
            return true;
        }

        public bool filterWarning(object o)
        {
            LogMessage m = o as LogMessage;
            if ((m.MessageType == MessageType.Warning) && (hideWarning == true))
                return false;
            return true;
        }

        public bool HideServer
        {
            get
            {
                return hideServer;
            }
            set
            {
                hideServer = value;

                notifyPropertyChanged();
                log.Refresh();
            }
        }

        public bool HideGeneral
        {
            get
            {
                return hideGeneral;
            }
            set
            {
                hideGeneral = value;

                notifyPropertyChanged();
                log.Refresh();
            }
        }

        public bool HideInfo
        {
            get
            {
                return hideInfo;
            }
            set
            {
                hideInfo = value;
                notifyPropertyChanged();

                log.Refresh();
            }
        }

        public bool HideError
        {
            get
            {
                return hideError;
            }
            set
            {
                hideError = value;
                notifyPropertyChanged();

                log.Refresh();
            }
        }

        public bool HideWarning
        {
            get
            {
                return hideWarning;
            }
            set
            {
                hideWarning = value;
                notifyPropertyChanged();
                log.Refresh();
            }
        }

        #region logFilter

        private bool logFilter(object o)
        {
            LogMessage m = o as LogMessage;

            switch (m.MessageType)
            {
                case MessageType.Warning:
                    if (hideWarning)
                        return false;
                    break;

                case MessageType.Server:
                    if (hideServer)
                        return false;
                    break;

                case MessageType.Info:
                    if (hideInfo)
                        return false;
                    break;

                case MessageType.General:
                    if (hideGeneral)
                        return false;
                    break;

                case MessageType.Error:
                    if (hideError)
                        return false;
                    break;
            }

            return true;
        }

        #endregion


        #endregion

        public LogFilter()
        {
            messages = new LimitedObservableCollection<LogMessage>();
            messages.MaxCapacity = 500;
            log = CollectionViewSource.GetDefaultView(Messages);
            log.Filter = logFilter;
            log.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Descending));

            InitializeComponent();
#if DEBUG
            messages.Add(new LogMessage("This is a general Message", MessageType.General));
            messages.Add(new LogMessage("This is a server Message", MessageType.Server));
            messages.Add(new LogMessage("This is an info", MessageType.Info));
            messages.Add(new LogMessage("This is an error", MessageType.Error));
            messages.Add(new LogMessage("This is a warning", MessageType.Warning));
            messages.Add(new LogMessage("I'm from the past", new DateTime(1990,1,1)));
            messages.Add(new LogMessage("I'm from the future", new DateTime(3000,1,1)));
#endif
        }

        public class LogMessage
        {
            public DateTime Timestamp { get; private set; }
            public LogFilter.MessageType MessageType { get; private set; }
            public string Message { get; private set; }

            public LogMessage(string Message)
            {
                initalize(DateTime.Now, LogFilter.MessageType.General, Message);
            }

            public LogMessage(string Message, DateTime Timestamp)
            {
                initalize(Timestamp, LogFilter.MessageType.General, Message);
            }

            public LogMessage(string Message, LogFilter.MessageType MessageType)
            {
                initalize(DateTime.Now, MessageType, Message);
            }

            public LogMessage(string Message, DateTime Timestamp, LogFilter.MessageType MessageType)
            {
                initalize(Timestamp, MessageType, Message);
            }

            private void initalize(DateTime timestamp, LogFilter.MessageType messageType, string message)
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

        private void LogView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                StringBuilder b = new StringBuilder();
                foreach(LogMessage m in LogView.SelectedItems)
                {
                    b.AppendFormat("{0:HH:mm:ss}", m.Timestamp);
                    b.Append(" ");
                    b.Append(m.MessageType.ToString());
                    b.Append(": ");
                    b.Append(m.Message);
                    b.Append(Environment.NewLine);
                }

                Clipboard.SetText(b.ToString());
            }
        }
    }
}
