using CWSProtocol;
using ServerService;
using ServerService.Access;
using ServerService.Access.Entries;
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
    public sealed class Server : INotifyPropertyChanged, IDisposable
    {
        private volatile bool _shouldStop = false;
        public event PropertyChangedEventHandler PropertyChanged;

        private ServerService.Statistics statistics;
        public ServerService.Statistics Statistics
        {
            get
            {
                return statistics;
            }
            set
            {
                if (statistics != value)
                    statistics = value;
            }
        }

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

        private Action clearLog;
        public Action ClearLog
        {
            get
            {
                return clearLog;
            }
            set
            {
                if (clearLog != value)
                    clearLog = value;
            }
        }

        private Func<List<Helper.LogMessage>> getLog;
        public Func<List<Helper.LogMessage>> GetLog
        {
            get
            {
                return getLog;
            }
            set
            {
                if (getLog != value)
                    getLog = value;
            }
        }

        NamedPipeServerStream serverStream;
        volatile EventWaitHandle wait;

        private void doServerWork()
        {
            IsRunning = true;

            Helper.Logging.OnLogMessage("Starting CWSRestartServer for process communication", ServerService.MessageType.Info);

            PipeSecurity ps = new PipeSecurity();

            ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, System.Security.AccessControl.AccessControlType.Allow));
            serverStream = new NamedPipeServerStream(CWSProtocol.Settings.SERVERNAME, PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, ps);


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
                                //Helper.Logging.OnLogMessage("Module connected", ServerService.MessageType.Info);

                                StreamReader sr = new StreamReader(serverStream, System.Text.Encoding.UTF8, true, 2048, true);
                                string message = sr.ReadLine();

                                if (message != null)
                                {
                                    string[] messages = message.Split(new string[] { " " }, 3, StringSplitOptions.None);

                                    if (messages.Count() == 3 || messages.Count() == 2)
                                    {
                                        if (messages.Count() == 2 || messages.Count() == 3)
                                        {
                                            CWSProtocol.Commands.Action a = (CWSProtocol.Commands.Action)Enum.Parse(typeof(CWSProtocol.Commands.Action), messages[0]);
                                            CWSProtocol.Commands.Command c = (CWSProtocol.Commands.Command)Enum.Parse(typeof(CWSProtocol.Commands.Command), messages[1]);

                                            message = (messages.Count() == 3) ? messages[2] : "";

                                            switch (a)
                                            {
                                                case CWSProtocol.Commands.Action.GET:

                                                    switch (c)
                                                    {
                                                        case CWSProtocol.Commands.Command.IDENTIFY:
                                                            Helper.Logging.OnLogMessage(String.Format("{0} said hello", message), ServerService.MessageType.Info);
                                                            sendReply(CWSProtocol.Commands.Command.ACK, "", serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.STATISTICS:
                                                            //Helper.Logging.OnLogMessage("Statistics were requested by an external module", ServerService.MessageType.Info);

                                                            sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("ALIVE {0}", ServerService.Validator.IsRunning()), serverStream);

                                                            if (Statistics != null && Statistics.Enabled)
                                                            {
                                                                sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("TOTAL {0}", Statistics.Players.Count), serverStream);
                                                                sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("CURRENT {0}", Statistics.ConnectedPlayers.Count), serverStream);
                                                                sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("RUNTIME {0:00}:{1:00}:{2:00}", Statistics.Runtime.TotalHours, Statistics.Runtime.Minutes, Statistics.Runtime.Seconds), serverStream);
                                                                sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("ENABLED {0}", Statistics.Enabled), serverStream);

                                                                if (Statistics.StatisticsDB != null)
                                                                    sendReply(CWSProtocol.Commands.Command.STATISTICS, String.Format("STATISTICSFILE {0}", Statistics.StatisticsDB.DatabaseFile), serverStream);
                                                            }

                                                            sendReply(CWSProtocol.Commands.Command.ENDSTATISTICS, "", serverStream);

                                                            break;

                                                        case CWSProtocol.Commands.Command.START:
                                                            ServerService.Helper.General.StartServer();
                                                            break;

                                                        case CWSProtocol.Commands.Command.STOP:
                                                            ServerService.Helper.General.SendQuit();
                                                            break;

                                                        case CWSProtocol.Commands.Command.RESTART:
                                                            ServerService.Helper.General.RestartServer();
                                                            break;

                                                        case CWSProtocol.Commands.Command.KILL:
                                                            ServerService.Helper.General.KillServer();
                                                            break;

                                                        case CWSProtocol.Commands.Command.WATCHER:
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("ENABLED {0}", Helper.Watcher.Instance.IsRunning), serverStream);
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("BLOCKED {0}", Helper.Watcher.Instance.IsBlocked), serverStream);
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("TIMEOUT {0}", Helper.Watcher.Instance.IntervallSeconds.ToString()), serverStream);
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("CHECKINTERNET {0}", ServerService.Helper.Settings.Instance.CheckInternet), serverStream);
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("CHECKLAN {0}", ServerService.Helper.Settings.Instance.CheckLAN), serverStream);
                                                            sendReply(CWSProtocol.Commands.Command.WATCHER, String.Format("CHECKLOOPBACK {0}", ServerService.Helper.Settings.Instance.CheckLoopback), serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.LOG:
                                                            if (GetLog != null)
                                                            {
                                                                List<Helper.LogMessage> logEntries = GetLog();

                                                                try
                                                                {
                                                                    StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);

                                                                    foreach (Helper.LogMessage m in logEntries)
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
                                                                sendReply(CWSProtocol.Commands.Command.LOG, "", serverStream);
                                                            }
                                                            break;

                                                        case CWSProtocol.Commands.Command.CONNECTED:

                                                            List<PlayerInfo> connected;

                                                            if (Statistics.Enabled && (connected = new List<PlayerInfo>(Statistics.ConnectedPlayers)).Count > 0)
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);

                                                                foreach (PlayerInfo ip in connected)
                                                                    writer.WriteLine(ip.Address.ToString());

                                                                writer.Close();
                                                            }
                                                            else
                                                            {
                                                                StreamWriter writer = new StreamWriter(serverStream, System.Text.Encoding.UTF8, 2048, true);
                                                                writer.WriteLine("");
                                                                writer.Close();
                                                            }

                                                            break;

                                                        case CWSProtocol.Commands.Command.ACCESSLIST:

                                                            List<AccessListEntry> entries = new List<AccessListEntry>(AccessControl.Instance.AccessList);

                                                            if ((Statistics.Enabled || ServerService.Helper.Settings.Instance.ExternalAccessControl) && entries.Count > 0)
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

                                                        case CWSProtocol.Commands.Command.ACCESSMODE:
                                                            sendReply(CWSProtocol.Commands.Command.ACCESSMODE, AccessControl.Instance.Mode.ToString(), serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.PLAYERSDATABASE:
                                                            Helper.Settings.Instance.SetUpPlayersdatabase();
                                                            sendReply(CWSProtocol.Commands.Command.PLAYERSDATABASE, Helper.Settings.Instance.KnownPlayersLocation, serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.PLAYERIDENTIFICATION:
                                                            if (Helper.Settings.Instance.PlayeridentificationEnabled)
                                                                sendReply(CWSProtocol.Commands.Command.PLAYERIDENTIFICATION, "ENABLED", serverStream);
                                                            else
                                                                sendReply(CWSProtocol.Commands.Command.PLAYERIDENTIFICATION, "DISABLED", serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.PREMIUMSLOTS:
                                                            if (Helper.Settings.Instance.PremiumslotsEnabled)
                                                                sendReply(CWSProtocol.Commands.Command.PREMIUMSLOTS, "ENABLED", serverStream);
                                                            else
                                                                sendReply(CWSProtocol.Commands.Command.PREMIUMSLOTS, "DISABLED", serverStream);
                                                            break;

                                                        case CWSProtocol.Commands.Command.PREMIUMDATABASE:
                                                            Helper.Settings.Instance.SetUpPremiumdatabase();
                                                            sendReply(CWSProtocol.Commands.Command.PREMIUMDATABASE, Helper.Settings.Instance.PremiumLocation, serverStream);
                                                            break;
                                                    }

                                                    break;
                                                case CWSProtocol.Commands.Action.POST:
                                                    switch (c)
                                                    {
                                                        case CWSProtocol.Commands.Command.LOG:
                                                            if (String.Compare(message, "clear", true) == 0)
                                                            {
                                                                if (ClearLog != null)
                                                                    ClearLog();
                                                            }
                                                            break;

                                                        case CWSProtocol.Commands.Command.WATCHER:
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

                                                        case CWSProtocol.Commands.Command.KICK:
                                                            IPAddress ip;
                                                            if (IPAddress.TryParse(message, out ip))
                                                            {
                                                                ServerService.Helper.DisconnectWrapper.CloseRemoteIP(ip.ToString());
                                                            }
                                                            break;

                                                        case CWSProtocol.Commands.Command.ACCESSLIST:
                                                            string line;
                                                            List<AccessListEntry> entries = new List<AccessListEntry>();
                                                            AccessListEntry tmp;

                                                            if (AccessControl.GenerateEntryFromString(message, out tmp))
                                                                entries.Add(tmp);

                                                            while ((line = sr.ReadLine()) != null)
                                                            {
                                                                string[] parts = line.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);

                                                                if (parts.Length == 3 && parts[0] == CWSProtocol.Commands.Action.POST.ToString() && parts[1] == CWSProtocol.Commands.Command.ACCESSLIST.ToString() && !String.IsNullOrEmpty(parts[2]))
                                                                {
                                                                    if (AccessControl.GenerateEntryFromString(parts[2], out tmp))
                                                                        entries.Add(tmp);
                                                                }
                                                            }

                                                            AccessControl.Instance.SetAccessList(new System.Collections.ObjectModel.ObservableCollection<AccessListEntry>(entries));
                                                            break;

                                                        case CWSProtocol.Commands.Command.ACCESSMODE:
                                                            AccessMode mode;

                                                            if (System.Enum.TryParse<AccessMode>(message, out mode))
                                                                AccessControl.Instance.Mode = mode;

                                                            break;

                                                        case CWSProtocol.Commands.Command.PRESET:
                                                            string[] content = message.Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);

                                                            if (content.Length == 2 && (content[0] == "DELETE" || content[0] == "PERSISTENT") && File.Exists(content[1]))
                                                            {
                                                                Helper.Settings.Instance.LoadPreset(content[1]);

                                                                if (content[0] == "DELETE")
                                                                    File.Delete(content[1]);
                                                            }

                                                            break;

                                                        case CWSProtocol.Commands.Command.PLAYERIDENTIFICATION:
                                                            if (message.ToLowerInvariant() == "enable")
                                                                Helper.Settings.Instance.PlayeridentificationEnabled = true;
                                                            else
                                                                Helper.Settings.Instance.PlayeridentificationEnabled = false;

                                                            Helper.Logging.OnLogMessage("Now ready to identify players...", ServerService.MessageType.Info);

                                                            break;

                                                        case CWSProtocol.Commands.Command.PREMIUMSLOTS:
                                                            if (message.ToLowerInvariant() == "enable")
                                                                Helper.Settings.Instance.PremiumslotsEnabled = true;
                                                            else
                                                                Helper.Settings.Instance.PremiumslotsEnabled = false;

                                                            Helper.Logging.OnLogMessage(Helper.Settings.Instance.PremiumslotsEnabled ? "Premium slots are enabled" : "Premium slots are disabled", ServerService.MessageType.Info);
                                                            break;

                                                        case CWSProtocol.Commands.Command.EXTERNALACCESSCONTROL:
                                                            if (message.ToLowerInvariant() == "enable")
                                                                ServerService.Helper.Settings.Instance.ExternalAccessControl = true;
                                                            else
                                                                ServerService.Helper.Settings.Instance.ExternalAccessControl = false;

                                                            Helper.Logging.OnLogMessage(ServerService.Helper.Settings.Instance.ExternalAccessControl ? "Access is controlled by an external program." : "Access to your server is controlled by CWSRestart.", ServerService.MessageType.Info);
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
                            Helper.Logging.OnLogMessage("CWSRestartServer has been stopped", ServerService.MessageType.Info);
                            wait.Set();
                        }
                    }, null);

                wait.WaitOne();
            }

            serverStream.Close();
            IsRunning = false;
        }

        private void sendReply(CWSProtocol.Commands.Command command, String content, NamedPipeServerStream server)
        {
            try
            {
                StreamWriter writer = new StreamWriter(server, System.Text.Encoding.UTF8, 2048, true);
                string message = String.Format("{0} {1} {2}", CWSProtocol.Commands.Action.POST, command, content);
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

        public void Dispose()
        {
            if (Statistics != null)
                Statistics.Dispose();

            if (serverStream != null)
                serverStream.Dispose();

            if (wait != null)
                wait.Dispose();
        }
    }
}
