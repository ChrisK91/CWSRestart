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

        public Client(string name)
        {
            this.name = name;
        }


        private void sendCommand(Commands.Command command, String content)
        {
            if (tryConnect())
            {
                StreamWriter writer = new StreamWriter(client, System.Text.Encoding.UTF8, 2048, true);

                string message = String.Format("{0} {1} {2}", Commands.Actions.GET, command, content);
                writer.WriteLine(message);
                writer.Close();
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

                return true;
            }
            catch(TimeoutException)
            {
                Console.WriteLine("Could not connect");
            }
            return false;
        }

        public bool Test()
        {
            sendCommand(Commands.Command.IDENTIFY, name);
            if (client.IsConnected)
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

        public Dictionary<string, object> GetStatistics()
        {
            sendCommand(Commands.Command.STATISTICS, "");
            Dictionary<string, object> ret = new Dictionary<string, object>();

            if (client.IsConnected)
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

        private void disconnectClient()
        {
            client.Dispose();
            client = null;
        }
    }
}