using ServerService.Access;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;



namespace CWSProtocol
{
    public sealed class Client : IDisposable
    {
        private NamedPipeClientStream client;
        private string name;
        public bool CanConnect { get; private set; }

        public Client(string name)
        {
            this.name = name;
            CanConnect = false;
        }

        private bool sendCommand(Commands.Command command)
        {
            return sendCommand(command, "");
        }

        private bool sendCommand(Commands.Command command, String content)
        {
            return sendCommand(command, content, Commands.Action.GET);
        }

        private bool sendCommand(Commands.Command command, String content, Commands.Action action)
        {
            try
            {
                if (tryConnect())
                {
                    StreamWriter writer = new StreamWriter(client, System.Text.Encoding.UTF8, 2048, true);

                    string message = String.Format("{0} {1} {2}", action, command, content);
                    writer.WriteLine(message);
                    writer.Close();
                    return true;
                }
                return false;
            }
            catch (IOException)
            {
                disconnectClient();
                return false;
            }
        }

        private Tuple<Commands.Command, string> readResponse()
        {
            if (tryConnect())
            {
                StreamReader reader = new StreamReader(client, System.Text.Encoding.UTF8, true, 2048, true);

                string response = reader.ReadLine();

                if (response == null)
                    return null;

                string[] messages = response.Split(new string[] { " " }, 3, StringSplitOptions.None);

                if (messages.Count() == 2 || messages.Count() == 3)
                {
                    Commands.Action a = (Commands.Action)Enum.Parse(typeof(Commands.Action), messages[0]);
                    Commands.Command c = (Commands.Command)Enum.Parse(typeof(Commands.Command), messages[1]);

                    string message = (messages.Count() == 3) ? messages[2] : "";

                    switch (a)
                    {
                        case Commands.Action.POST:
                            reader.Close();
                            return new Tuple<Commands.Command, string>(c, message);
                        default:
                            reader.Close();
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    reader.Close();
                    throw new NotImplementedException();
                }
            }
            return null;
        }

        private bool tryConnect()
        {
            try
            {
                if (client == null)
                    client = new NamedPipeClientStream(".", Settings.SERVERNAME, PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation);

                if (!client.IsConnected)
                {
                    client.Connect(10);
                }

                CanConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                if (ex is IOException)
                    Console.WriteLine("Could not connect");
                else if (ex is TimeoutException)
                    Console.WriteLine("Connection to CWSRestart timed out");
                else
                {
                    Console.WriteLine(ex.Message);

                    if (Debugger.IsAttached)
                        Debugger.Break();
                }
            }
            return false;
        }

        public bool Test()
        {
            if (sendCommand(Commands.Command.IDENTIFY, name))
            {
                Tuple<Commands.Command, string> answer = readResponse();

                if (answer == null)
                {
                    disconnectClient();
                    return false;
                }

                if (answer.Item1 == Commands.Command.ACK)
                {
                    disconnectClient();
                    return true;
                }
            }

            disconnectClient();
            return false;
        }

        public void SendStart()
        {
            if (sendCommand(Commands.Command.START))
                disconnectClient();
        }

        public void SendStop()
        {
            if (sendCommand(Commands.Command.STOP))
                disconnectClient();
        }

        public void SendRestart()
        {
            if (sendCommand(Commands.Command.RESTART))
                disconnectClient();
        }

        public void SendKill()
        {
            if (sendCommand(Commands.Command.KILL))
                disconnectClient();
        }

        public void SetWatcherTimeout(int seconds)
        {
            if (sendCommand(Commands.Command.WATCHER, String.Format("TIMEOUT {0}", seconds), Commands.Action.POST))
                disconnectClient();
        }

        public void SetWatcherCheckAccess(bool CheckInternet, bool CheckLAN, bool CheckLoopback)
        {
            if (sendCommand(Commands.Command.WATCHER, String.Format("ACCESS CHECKINTERNET {0} CHECKLAN {1} CHECKLOOPBACK {2}", CheckInternet, CheckLAN, CheckLoopback), Commands.Action.POST))
                disconnectClient();
        }

        public Dictionary<string, object> GetStatistics()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            if (sendCommand(Commands.Command.STATISTICS))
            {
                Tuple<Commands.Command, string> answer;

                answer = readResponse();

                if (answer != null)
                {
                    while (answer != null && answer.Item1 != Commands.Command.ENDSTATISTICS)
                    {

                        if (answer.Item1 == Commands.Command.STATISTICS)
                        {
                            string[] contents = answer.Item2.Split(new string[] { " " }, 2, StringSplitOptions.None);

                            if (contents.Length == 2)
                            {
                                ret.Add(contents[0], contents[1]);
                            }
                        }
                        answer = readResponse();
                    }
                }
            }

            disconnectClient();
            return ret;
        }

        public Dictionary<string, object> GetWatcherStatus()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            if (sendCommand(Commands.Command.WATCHER))
            {
                for (int i = 0; i < 6; i++)
                {
                    Tuple<Commands.Command, string> answer = readResponse();

                    if (answer != null && answer.Item1 == Commands.Command.WATCHER)
                    {
                        string[] contents = answer.Item2.Split(new string[] { " " }, 2, StringSplitOptions.None);

                        if (contents.Length == 2)
                        {
                            ret.Add(contents[0], contents[1]);
                        }
                    }
                    else
                    {
                        disconnectClient();
                        return null;
                    }
                }
            }

            disconnectClient();
            return ret;
        }

        public IList<string> GetLogMessages()
        {
            List<string> ret = new List<string>();

            if (sendCommand(Commands.Command.LOG))
            {
                string line;
                StreamReader reader = new StreamReader(client, System.Text.Encoding.UTF8, true, 2048, true);

                while ((line = reader.ReadLine()) != null)
                    ret.Add(line);
            }

            disconnectClient();
            return ret;
        }

        public void StartWatcher()
        {
            sendCommand(Commands.Command.WATCHER, "START", Commands.Action.POST);
            disconnectClient();
        }

        public void StopWatcher()
        {
            sendCommand(Commands.Command.WATCHER, "STOP", Commands.Action.POST);
            disconnectClient();
        }

        public void ClearLogMessage()
        {
            sendCommand(Commands.Command.LOG, "CLEAR", Commands.Action.POST);
            disconnectClient();
        }

        public IList<string> GetConnectedPlayers()
        {
            List<string> ret = new List<string>();

            if (sendCommand(Commands.Command.CONNECTED))
            {
                string line;
                StreamReader reader = new StreamReader(client, System.Text.Encoding.UTF8, true, 2048, true);

                while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                    ret.Add(line);
            }

            disconnectClient();
            return ret;
        }

        public void KickPlayer(string ip)
        {
            sendCommand(Commands.Command.KICK, ip, Commands.Action.POST);
            disconnectClient();
        }

        public IList<string> GetAccessListEntries()
        {
            List<string> ret = new List<string>();

            if (sendCommand(Commands.Command.ACCESSLIST))
            {
                string line;
                StreamReader reader = new StreamReader(client, System.Text.Encoding.UTF8, true, 2048, true);

                while (String.IsNullOrEmpty(line = reader.ReadLine()))
                    ret.Add(line);
            }

            disconnectClient();
            return ret;
        }

        private void disconnectClient()
        {
            if (client != null)
            {
                client.Dispose();
            }
            client = null;
        }

        public AccessMode GetAccessMode()
        {
            AccessMode ret = AccessMode.Blacklist;
            if (sendCommand(Commands.Command.ACCESSMODE))
            {
                Tuple<Commands.Command, string> answer = readResponse();

                if (answer.Item1 == Commands.Command.ACCESSMODE)
                    ret = (AccessMode)Enum.Parse(typeof(AccessMode), answer.Item2);
            }

            disconnectClient();
            return ret;
        }

        public void SetAccess(IList<string> accessList, AccessMode mode)
        {
            if (accessList != null)
            {
                if (accessList.Count == 0)
                {
                    sendCommand(Commands.Command.ACCESSLIST, "", Commands.Action.POST);
                }
                else if (tryConnect())
                {
                    StreamWriter writer = new StreamWriter(client, System.Text.Encoding.UTF8, 2048, true);
                    foreach (string s in accessList)
                    {
                        string message = String.Format("{0} {1} {2}", Commands.Action.POST.ToString(), Commands.Command.ACCESSLIST.ToString(), s);
                        writer.WriteLine(message);
                    }
                    writer.Close();
                }

                disconnectClient();
                sendCommand(Commands.Command.ACCESSMODE, mode.ToString(), Commands.Action.POST);
                disconnectClient();
            }
        }

        public bool SendPreset(string file, bool deleteFile)
        {
            if (tryConnect())
            {
                sendCommand(Commands.Command.PRESET, String.Format("{0} {1}", deleteFile ? "DELETE" : "PERSISTENT", file), Commands.Action.POST);
                disconnectClient();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetPlayersDatabase()
        {
            if (sendCommand(Commands.Command.PLAYERSDATABASE))
            {
                Tuple<Commands.Command, string> answer = readResponse();
                disconnectClient();
                return answer == null ? null : answer.Item2;
            }
            return null;
        }

        public bool SetPlayerIdentification(bool enabled)
        {
            if (sendCommand(Commands.Command.PLAYERIDENTIFICATION, enabled ? "ENABLE" : "DISABLE", Commands.Action.POST))
            {
                disconnectClient();
                return true;
            }
            return false;
        }

        public bool GetPlayerIdentification()
        {
            if (sendCommand(Commands.Command.PLAYERIDENTIFICATION))
            {
                Tuple<Commands.Command, string> answer = readResponse();
                disconnectClient();

                return answer != null && answer.Item2.ToLowerInvariant() == "enabled" ? true : false;
            }

            return false;
        }

        public void Dispose()
        {
            if (client != null)
                client.Dispose();
        }
    }
}