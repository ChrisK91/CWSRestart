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
    public class Client 
    {
        private NamedPipeClientStream client;
        private string name;
        public bool CanConnect = false;

        public Client(string name)
        {
            this.name = name;
        }

        private bool sendCommand(Commands.Command command)
        {
            return sendCommand(command, "");
        }

        private bool sendCommand(Commands.Command command, String content)
        {
            return sendCommand(command, content, Commands.Actions.GET);
        }

        private bool sendCommand(Commands.Command command, String content, Commands.Actions action)
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
                    Commands.Actions a = (Commands.Actions)Enum.Parse(typeof(Commands.Actions), messages[0]);
                    Commands.Command c = (Commands.Command)Enum.Parse(typeof(Commands.Command), messages[1]);

                    string message = (messages.Count() == 3) ? messages[2] : "";

                    switch (a)
                    {
                        case Commands.Actions.POST:
                            return new Tuple<Commands.Command, string>(c, message);
                            break;
                        default:
                            throw new NotImplementedException();
                            break;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                reader.Close();
            }
            return null;
        }

        private bool tryConnect()
        {
            try
            {
                if(client == null)
                    client = new NamedPipeClientStream(".", Configuration.SERVERNAME, PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation);

                if (!client.IsConnected)
                {
                    client.Connect(10);
                }

                CanConnect = true;
                return true;
            }
            catch(Exception ex)
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
                disconnectClient();

                if (answer == null)
                {
                    return false;
                }

                if (answer.Item1 == Commands.Command.ACK)
                {
                    return true;
                }
            }

            return false;
        }

        public void SendStart()
        {
            if(sendCommand(Commands.Command.START))
                disconnectClient();
        }

        public void SendStop()
        {
            if(sendCommand(Commands.Command.STOP))
                disconnectClient();
        }

        public void SendRestart()
        {
            if(sendCommand(Commands.Command.RESTART))
                disconnectClient();
        }

        public void SendKill()
        {
            if(sendCommand(Commands.Command.KILL))
                disconnectClient();
        }

        public void SetWatcherTimeout(UInt32 seconds)
        {
            if (sendCommand(Commands.Command.WATCHER, String.Format("TIMEOUT {0}", seconds), Commands.Actions.POST))
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

            if(sendCommand(Commands.Command.WATCHER))
            {
                for (int i = 0; i < 3; i++)
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
                        return null;
                    }
                }
            }

            disconnectClient();
            return ret;
        }

        public List<string> GetLogMessages()
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
            sendCommand(Commands.Command.WATCHER, "START", Commands.Actions.POST);
            disconnectClient();
        }

        public void StopWatcher()
        {
            sendCommand(Commands.Command.WATCHER, "STOP", Commands.Actions.POST);
            disconnectClient();
        }

        public void ClearLogMessage()
        {
            sendCommand(Commands.Command.LOG, "CLEAR", Commands.Actions.POST);
            disconnectClient();
        }

        private void disconnectClient()
        {
            if (client != null)
            {
                client.Dispose();
            }
            client = null;
        }
    }
}