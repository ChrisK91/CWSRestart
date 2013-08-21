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
    /// <summary>
    /// A CWSProtocol client that is able to communicate with CWSRestart
    /// </summary>
    public sealed class Client : IDisposable
    {
        /// <summary>
        /// The stream to CWSRestart
        /// </summary>
        private NamedPipeClientStream client;

        /// <summary>
        /// Internal name of the client
        /// </summary>
        private string name;

        /// <summary>
        /// True if a succesful connection has ben established in the past, otherwise false
        /// </summary>
        public bool CanConnect { get; private set; }

        /// <summary>
        /// Initializes the connection to CWSRestart
        /// </summary>
        /// <param name="name"></param>
        public Client(string name)
        {
            this.name = name;
            CanConnect = false;
        }

        /// <summary>
        /// Sends the given command to CWSRestart (as GET request)
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <returns>True if the command was sent succesful, otherwise false</returns>
        private bool sendCommand(Commands.Command command)
        {
            return sendCommand(command, "");
        }

        /// <summary>
        /// Sends the given command with the given content to CWSRestart (as GET request)
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <param name="content">The content of the command</param>
        /// <returns>True if the command was sent succesful, otherwise false</returns>
        private bool sendCommand(Commands.Command command, String content)
        {
            return sendCommand(command, content, Commands.Action.GET);
        }

        /// <summary>
        /// Send the given command with the given content and method to CWSRestart
        /// </summary>
        /// <param name="command"></param>
        /// <param name="content"></param>
        /// <param name="action"></param>
        /// <returns>True if the command was sent succesful, otherwise false</returns>
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

        /// <summary>
        /// Reads a response from the stream to CWSRestart
        /// </summary>
        /// <returns>A tuple containting the Command and the content of the response. Can be null if no response was read</returns>
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

        /// <summary>
        /// Attempts to connect to CWSRestart
        /// </summary>
        /// <returns>True if a connection was made, otherwise false</returns>
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

        /// <summary>
        /// Tests the connection to CWSRestart
        /// </summary>
        /// <returns>True if succesful, otherwise false</returns>
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

        /// <summary>
        /// Sends a message to start the server
        /// </summary>
        public void SendStart()
        {
            if (sendCommand(Commands.Command.START))
                disconnectClient();
        }

        /// <summary>
        /// Sends a message to stop the server
        /// </summary>
        public void SendStop()
        {
            if (sendCommand(Commands.Command.STOP))
                disconnectClient();
        }

        /// <summary>
        /// Sends a message to restart the server
        /// </summary>
        public void SendRestart()
        {
            if (sendCommand(Commands.Command.RESTART))
                disconnectClient();
        }

        /// <summary>
        /// Sends a message to terminate the server
        /// </summary>
        public void SendKill()
        {
            if (sendCommand(Commands.Command.KILL))
                disconnectClient();
        }

        /// <summary>
        /// Sends a message to set the watcher interval to the given time
        /// </summary>
        /// <param name="seconds">The seconds between two checks</param>
        public void SetWatcherTimeout(int seconds)
        {
            if (sendCommand(Commands.Command.WATCHER, String.Format("TIMEOUT {0}", seconds), Commands.Action.POST))
                disconnectClient();
        }

        /// <summary>
        /// Sends the access scheme to CWSRestart
        /// </summary>
        /// <param name="CheckInternet">Indicates if access from the internet should be checked</param>
        /// <param name="CheckLAN">Indicates if access from the local network should be checked</param>
        /// <param name="CheckLoopback">Indicates if access from loopback should be checked</param>
        public void SetWatcherCheckAccess(bool CheckInternet, bool CheckLAN, bool CheckLoopback)
        {
            if (sendCommand(Commands.Command.WATCHER, String.Format("ACCESS CHECKINTERNET {0} CHECKLAN {1} CHECKLOOPBACK {2}", CheckInternet, CheckLAN, CheckLoopback), Commands.Action.POST))
                disconnectClient();
        }

        /// <summary>
        /// Retrieves the statistics from CWSRestart
        /// </summary>
        /// <returns>A dictionary containing a unique name and the related value.</returns>
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

        /// <summary>
        /// Retrieves the watcher configuration from CWSRestart
        /// </summary>
        /// <returns>A dictionary with configuration parameters as keys and the related values</returns>
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

        /// <summary>
        /// Retrieves a list of the current log messages from CWSRestart
        /// </summary>
        /// <returns>A List with formated log messages</returns>
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

        /// <summary>
        /// Starts the watcher
        /// </summary>
        public void StartWatcher()
        {
            sendCommand(Commands.Command.WATCHER, "START", Commands.Action.POST);
            disconnectClient();
        }

        /// <summary>
        /// Stops the watcher
        /// </summary>
        public void StopWatcher()
        {
            sendCommand(Commands.Command.WATCHER, "STOP", Commands.Action.POST);
            disconnectClient();
        }

        /// <summary>
        /// Clears the log in CWSRestart
        /// </summary>
        public void ClearLogMessage()
        {
            sendCommand(Commands.Command.LOG, "CLEAR", Commands.Action.POST);
            disconnectClient();
        }

        /// <summary>
        /// Retreives a list of connected player IPs
        /// </summary>
        /// <returns>A list with connected player ips</returns>
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

        /// <summary>
        /// Attempts to kick the specified player
        /// </summary>
        /// <param name="ip">The player to kick</param>
        public void KickPlayer(string ip)
        {
            sendCommand(Commands.Command.KICK, ip, Commands.Action.POST);
            disconnectClient();
        }

        /// <summary>
        /// Retreives the access lsit configuration
        /// </summary>
        /// <returns>A list containing the lines of the access list</returns>
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

        /// <summary>
        /// Disconnects the connection to CWSRestart
        /// </summary>
        private void disconnectClient()
        {
            if (client != null)
            {
                client.Dispose();
            }
            client = null;
        }

        /// <summary>
        /// Retreives the AccessMode of the access list
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Configures the Access Filter in CWSRestart
        /// </summary>
        /// <param name="accessList">The accesslist entries</param>
        /// <param name="mode">The accessmode (whitelist/blacklist)</param>
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

        /// <summary>
        /// Sends a preset file to CWSRestart
        /// </summary>
        /// <param name="file">The filepath to the preset file</param>
        /// <param name="deleteFile">True if the file should be deleted by CWSRestart after loading</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retreives the location of the player database
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Enables/disables the player identification in CWSRestart
        /// </summary>
        /// <param name="enabled">True if identification should be enabled, otherwise false</param>
        /// <returns></returns>
        public bool SetPlayerIdentification(bool enabled)
        {
            if (sendCommand(Commands.Command.PLAYERIDENTIFICATION, enabled ? "ENABLE" : "DISABLE", Commands.Action.POST))
            {
                disconnectClient();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retreives the status of the player identification
        /// </summary>
        /// <returns>True if identification is enabled, otherwise false</returns>
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

        /// <summary>
        /// Enables/disables premium slots in CWSRestart. This is mainly used to communicate with other modules.
        /// </summary>
        /// <param name="enabled">True if identification should be enabled, otherwise false</param>
        /// <returns></returns>
        public bool SetPremiumslots(bool enabled)
        {
            if (sendCommand(Commands.Command.PREMIUMSLOTS, enabled ? "ENABLE" : "DISABLE", Commands.Action.POST))
            {
                disconnectClient();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retreives the status of the premium slots (enabled/disabled
        /// </summary>
        /// <returns>True if identification is enabled, otherwise false</returns>
        public bool GetPremiumslots()
        {
            if (sendCommand(Commands.Command.PREMIUMSLOTS))
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