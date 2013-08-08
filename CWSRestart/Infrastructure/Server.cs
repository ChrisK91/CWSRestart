using CWSProtocol;
using ServerService;
using ServerService.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWSRestart.Infrastructure
{
    public class Server : INotifyPropertyChanged
    {
        private volatile bool _shouldStop = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public ServerService.Statistics Statistics;

        public static Server Instance
        {
            get
            {
                return instance;
            }
        }

        private static Server instance = new Server();

        private Server()
        {
        }

        public Action ClearLog;
        public Func<List<Controls.LogFilter.LogMessage>> GetLog;

        NamedPipeServerStream serverStream;
        volatile EventWaitHandle wait;

        private void doServerWork()
        {
            IsRunning = true;

            Helper.Logging.OnLogMessage("Starting CWSRestartServer for process communication", ServerService.Logging.MessageType.Info);

            PipeSecurity ps = new PipeSecurity();

            ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, System.Security.AccessControl.AccessControlType.Allow));
            serverStream = new NamedPipeServerStream(CWSProtocol.Configuration.SERVERNAME, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, ps);


            while (!_shouldStop)
            {
                wait = new EventWaitHandle(false, EventResetMode.ManualReset);

                serverStream.BeginWaitForConnection(ar =>
                    {
                        try
                        {
                            serverStream.EndWaitForConnection(ar);
                            if (serverStream.IsConnected)
                            {
                                //Helper.Logging.OnLogMessage("Module connected", ServerService.Logging.MessageType.Info);

                                StreamReader sr = new StreamReader(serverStream, System.Text.Encoding.UTF8, true, 2048, true);
                                string message = sr.ReadLine();
                                
                                if (message != null)
                                {
                                    string[] messages = message.Split(new string[] { " " }, 3, StringSplitOptions.None);

                                    if (messages.Count() == 3 || messages.Count() == 2)
                                    {
                                        if (messages.Count() == 2 || messages.Count() == 3)
                                        {
                                            Commands.Actions a = (Commands.Actions)Enum.Parse(typeof(Commands.Actions), messages[0]);
                                            Commands.Command c = (Commands.Command)Enum.Parse(typeof(Commands.Command), messages[1]);

                                            message = (messages.Count() == 3) ? messages[2] : "";

                                            switch (a)
                                            {
                                                case Commands.Actions.GET:

                                                    switch (c)
                                                    {
                                                        case Commands.Command.IDENTIFY:
                                                            Helper.Logging.OnLogMessage(String.Format("{0} has said hello", message), ServerService.Logging.MessageType.Info);
                                                            sendReply(Commands.Command.ACK, "", serverStream);
                                                            break;

                                                        case Commands.Command.STATISTICS:
                                                            //Helper.Logging.OnLogMessage("Statistics were requested by an external module", ServerService.Logging.MessageType.Info);

                                                            sendReply(Commands.Command.STATISTICS, String.Format("ALIVE {0}", ServerService.Validator.Instance.IsRunning()), serverStream);

                                                            if (Statistics != null && Statistics.Enabled)
                                                            {
                                                                sendReply(Commands.Command.STATISTICS, String.Format("TOTAL {0}", Statistics.Players.Count), serverStream);
                                                                sendReply(Commands.Command.STATISTICS, String.Format("CURRENT {0}", Statistics.ConnectedPlayers.Count), serverStream);
                                                                sendReply(Commands.Command.STATISTICS, String.Format("RUNTIME {0:00}:{1:00}:{2:00}", Statistics.Runtime.TotalHours, Statistics.Runtime.Minutes, Statistics.Runtime.Seconds), serverStream);
                                                                sendReply(Commands.Command.STATISTICS, String.Format("ENABLED {0}", Statistics.Enabled), serverStream);

                                                                if(Statistics.StatisticsDB != null)
                                                                    sendReply(Commands.Command.STATISTICS, String.Format("STATISTICSFILE {0}", Statistics.StatisticsDB.Filename), serverStream);
                                                            }

                                                            sendReply(Commands.Command.ENDSTATISTICS, "", serverStream);

                                                            break;

                                                        case Commands.Command.START:
                                                            ServerService.Helper.General.StartServer();
                                                            break;

                                                        case Commands.Command.STOP:
                                                            ServerService.Helper.General.SendQuit();
                                                            break;

                                                        case Commands.Command.RESTART:
                                                            ServerService.Helper.General.RestartServer();
                                                            break;

                                                        case Commands.Command.KILL:
                                                            ServerService.Helper.General.KillServer();
                                                            break;

                                                        case Commands.Command.WATCHER:
                                                            sendReply(Commands.Command.WATCHER, String.Format("ENABLED {0}", Helper.Watcher.Instance.IsRunning), serverStream);
                                                            sendReply(Commands.Command.WATCHER, String.Format("BLOCKED {0}", Helper.Watcher.Instance.IsBlocked), serverStream);
                                                            sendReply(Commands.Command.WATCHER, String.Format("TIMEOUT {0}", Helper.Watcher.Instance.IntervallSeconds.ToString()), serverStream);
                                                            sendReply(Commands.Command.WATCHER, String.Format("CHECKINTERNET {0}", ServerService.Helper.Settings.Instance.CheckInternet), serverStream);
                                                            sendReply(Commands.Command.WATCHER, String.Format("CHECKLAN {0}", ServerService.Helper.Settings.Instance.CheckLAN), serverStream);
                                                            sendReply(Commands.Command.WATCHER, String.Format("CHECKLOOPBACK {0}", ServerService.Helper.Settings.Instance.CheckLoopback), serverStream);
                                                            break;

                                                        case Commands.Command.LOG:
                                                            if (GetLog != null)
                                                            {
                                                                List<Controls.LogFilter.LogMessage> logEntries = GetLog();

                                                                try
                                                                {
                                                                    StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);

                                                                    foreach (Controls.LogFilter.LogMessage m in logEntries)
                                                                    {
                                                                        if (m != null)
                                                                        {
                                                                            StringBuilder b = new StringBuilder();
                                                                            b.AppendFormat("{0:HH:mm:ss}", m.Timestamp);
                                                                            b.Append(" ");
                                                                            b.Append(m.MessageType.ToString());
                                                                            b.Append(": ");
                                                                            b.Append(m.Message);
                                                                            b.Append(Environment.NewLine);

                                                                            writer.WriteLine(b.ToString());
                                                                        }
                                                                    }

                                                                    writer.Close();
                                                                }
                                                                catch (IOException ex)
                                                                {
                                                                    if (Debugger.IsAttached)
                                                                    {
                                                                        Debugger.Break();
                                                                        Debugger.Log(1, "server", ex.Message);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                sendReply(Commands.Command.LOG, "", serverStream);
                                                            }
                                                            break;

                                                        case Commands.Command.CONNECTED:

                                                            List<IPAddress> connected;

                                                            if (Statistics.Enabled && (connected = new List<IPAddress>(Statistics.ConnectedPlayers)).Count > 0)
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);

                                                                foreach (IPAddress ip in connected)
                                                                    writer.WriteLine(ip.ToString());

                                                                writer.Close();
                                                            }
                                                            else
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);
                                                                writer.WriteLine("");
                                                                writer.Close();
                                                            }

                                                            break;

                                                        case Commands.Command.ACCESSLIST:

                                                            List<AccessListEntry> entries = new List<AccessListEntry>(AccessControl.Instance.AccessList);

                                                            if (Statistics.Enabled && entries.Count > 0)
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);

                                                                foreach (AccessListEntry e in entries)
                                                                    writer.WriteLine(e.ToString());

                                                                writer.Close();
                                                            }
                                                            else
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);
                                                                writer.WriteLine("");
                                                                writer.Close();
                                                            }

                                                            break;

                                                        case Commands.Command.ACCESSMODE:
                                                            sendReply(Commands.Command.ACCESSMODE, AccessControl.Instance.Mode.ToString(), serverStream);
                                                            break;
                                                    }

                                                    break;
                                                case Commands.Actions.POST:
                                                    switch (c)
                                                    {
                                                        case Commands.Command.LOG:
                                                            if (String.Compare(message, "clear", true) == 0)
                                                            {
                                                                if (ClearLog != null)
                                                                    ClearLog();
                                                            }
                                                            break;

                                                        case Commands.Command.WATCHER:
                                                            {
                                                                if (String.Compare(message, "start", true) == 0 && !Helper.Watcher.Instance.IsRunning)
                                                                    Helper.Watcher.Instance.Start();
                                                                else if (String.Compare(message, "stop", true) == 0 && Helper.Watcher.Instance.IsRunning && !Helper.Watcher.Instance.IsBlocked)
                                                                    Helper.Watcher.Instance.Stop();
                                                                else
                                                                {
                                                                    string[] parts = message.Split(new string[] { " " }, 2, StringSplitOptions.None);

                                                                    if (parts.Length == 2)
                                                                    {
                                                                        switch (parts[0])
                                                                        {
                                                                            case "TIMEOUT":
                                                                                UInt32 seconds;
                                                                                if (UInt32.TryParse(parts[1], out seconds))
                                                                                    Helper.Watcher.Instance.IntervallSeconds = seconds;
                                                                                break;
                                                                            case "ACCESS":
                                                                                parts = parts[1].Split(' ');
                                                                                if (parts.Length == 6)
                                                                                {
                                                                                    for (int i = 0; i < parts.Length; i = i + 2)
                                                                                    {
                                                                                        bool check;
                                                                                        if (Boolean.TryParse(parts[i + 1], out check))
                                                                                        {
                                                                                            switch (parts[i])
                                                                                            {
                                                                                                case "CHECKINTERNET":
                                                                                                    ServerService.Helper.Settings.Instance.CheckInternet = check;
                                                                                                    break;

                                                                                                case "CHECKLAN":
                                                                                                    ServerService.Helper.Settings.Instance.CheckLAN = check;
                                                                                                    break;

                                                                                                case "CHECKLOOPBACK":
                                                                                                    ServerService.Helper.Settings.Instance.CheckLoopback = check;
                                                                                                    break;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;

                                                        case Commands.Command.KICK:
                                                            IPAddress ip;
                                                            if (IPAddress.TryParse(message, out ip))
                                                            {
                                                                ServerService.Helper.DisconnectWrapper.CloseRemoteIP(ip.ToString());
                                                            }
                                                            break;

                                                        case Commands.Command.ACCESSLIST:
                                                            string line;
                                                            List<AccessListEntry> entries = new List<AccessListEntry>();
                                                            AccessListEntry tmp;

                                                            if (AccessControl.GenerateEntryFromString(message, out tmp))
                                                                entries.Add(tmp);

                                                            while ((line = sr.ReadLine()) != null)
                                                            {
                                                                string[] parts = line.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);

                                                                if (parts.Length == 3 && parts[0] == Commands.Actions.POST.ToString() && parts[1] == Commands.Command.ACCESSLIST.ToString() && parts[2] != String.Empty)
                                                                {
                                                                    if (AccessControl.GenerateEntryFromString(parts[2], out tmp))
                                                                        entries.Add(tmp);
                                                                }
                                                            }

                                                            AccessControl.Instance.AccessList = new System.Collections.ObjectModel.ObservableCollection<AccessListEntry>(entries);
                                                            break;

                                                        case Commands.Command.ACCESSMODE:
                                                            ServerService.AccessControl.AccessMode mode;

                                                            if (System.Enum.TryParse<AccessControl.AccessMode>(message, out mode))
                                                                AccessControl.Instance.Mode = mode;

                                                            break;

                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }

                                sr.Close();
                            }
                            serverStream.Disconnect();
                            wait.Set();
                        }
                        catch (ObjectDisposedException)
                        {
                            Helper.Logging.OnLogMessage("CWSRestartServer has been stopped", ServerService.Logging.MessageType.Info);
                            wait.Set();
                        }
                    }, null);

                wait.WaitOne();
            }

            serverStream.Close();
            IsRunning = false;
        }

        private void sendReply(Commands.Command command, String content, NamedPipeServerStream server)
        {
            try
            {
                StreamWriter writer = new StreamWriter(server, System.Text.Encoding.UTF8, 2048, true);
                string message = String.Format("{0} {1} {2}", Commands.Actions.POST, command, content);
                writer.WriteLine(message);
                writer.Close();
            }
            catch (IOException ex)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Log(0, "Error", ex.Message);
                    Debugger.Break();
                }
                return;
            }
        }

        private void clientConnected(IAsyncResult ar)
        {
            using (NamedPipeServerStream serverStream = new NamedPipeServerStream("CWSRestartServer", PipeDirection.In, 4, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                serverStream.EndWaitForConnection(ar);
            }
        }

        private Thread mainServer;

        private bool isRunning = false;
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    notifyPropertyChanged();
                    notifyPropertyChanged("ButtonText");
                }
            }
        }

        public string ButtonText
        {
            get
            {
                return (isRunning) ? "Stop CWSProtocol" : "Start CWSProtocol";
            }
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleServer()
        {
            if (mainServer == null || !mainServer.IsAlive)
            {
                _shouldStop = false;
                ThreadStart start = new ThreadStart(doServerWork);
                mainServer = new Thread(start);
                mainServer.Start();
            }
            else
            {
                if (wait != null)
                    wait.Set();

                _shouldStop = true;
            }
        }
    }
}
