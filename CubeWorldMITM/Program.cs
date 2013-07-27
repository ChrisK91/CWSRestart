using CubeWorldMITM.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CubeWorldMITM
{
    class Program
    {
        private static TcpListener mitm;
        private static volatile bool shouldExit = false;
        private static EventWaitHandle wait;

        private static List<MITMMessageHandler> establishedConnections = new List<MITMMessageHandler>();

        private static Dictionary<string, List<string>> KnownNames = new Dictionary<string, List<string>>();
        private static Dictionary<string, MITMMessageHandler> ConnectedPlayers = new Dictionary<string, MITMMessageHandler>();

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            centerText("------------------------------------------------------");
            centerText("| This program is heavily inspired and based on Coob |");
            centerText("|   Coob and CWSRestart are licensed under the GPL   |");
            centerText("------------------------------------------------------");
            Console.WriteLine();

            mitm = new TcpListener(IPAddress.Any, 12345);

            Thread listenerThread = new Thread(new ThreadStart(ConnectionLoop));
            listenerThread.Start();

            Console.WriteLine("Press any key to exit...");
            messageLoop();

            shouldExit = true;

            Dictionary<string, MITMMessageHandler> tmp = new Dictionary<string, MITMMessageHandler>(ConnectedPlayers);
            foreach(KeyValuePair<string, MITMMessageHandler> h in tmp)
                if(h.Value.Connected)
                    h.Value.Disconnect();

            listenerThread.Abort();
        }

        private static void messageLoop()
        {
            while (shouldExit != true)
            {
                Console.WriteLine("Enter one of the following options");
                Console.WriteLine("c - to receive a list of every currently connected player");
                Console.WriteLine("n - to receive a list of every name that we know and the corresponding IP");
                Console.WriteLine("a - about");
                Console.WriteLine("quit - to quit");

                string action = Console.ReadLine();
                Console.Clear();

                switch(action.ToLower())
                {
                    case "c":
                        centerText("--------------------------");
                        centerText("Connected players");
                        centerText("--------------------------");

                        Dictionary<string, MITMMessageHandler> tmpC = new Dictionary<string, MITMMessageHandler>(ConnectedPlayers);

                        foreach (KeyValuePair<string, MITMMessageHandler> h in tmpC)
                        {
                            if (h.Value.Name != null)
                                Console.WriteLine("{0}: {1} (Level: {2})", h.Key, h.Value.Name, h.Value.Level);
                            else
                                Console.WriteLine("{0}: Unknown name", h.Key);
                        }

                        break;

                    case "n":
                        centerText("--------------------------");
                        centerText("Known players");
                        centerText("--------------------------");

                        Dictionary<string, List<string>> tmpK = new Dictionary<string, List<string>>(KnownNames);

                        foreach (KeyValuePair<string, List<string>> h in tmpK)
                        {
                            Console.WriteLine("{0} has visited us with the following names:", h.Key);

                            foreach(string s in h.Value)
                                Console.Write("{0} ", s);

                            Console.WriteLine();
                        }

                        break;

                    case "a":
                        centerText("--------------------------");
                        centerText("About");
                        centerText("--------------------------");

                        centerText("CubeWorldMITM - (c) 2013 ChrisK91 licensed under the GPL3.");
                        centerText("You can view a copy of the license at http://www.gnu.org/licenses/gpl-3.0.html");
                        Console.WriteLine();
                        centerText("Additional information:");
                        centerText("CubeWorldMITM is based on code from Cube. (c) by the respective owners");
                        centerText("Project page: https://github.com/amPerl/Coob");
                        centerText("Cube is licensed under the GPL2.");
                        centerText("You can view the license at: http://www.gnu.org/licenses/gpl-2.0.html");
                        Console.WriteLine();
                        centerText("This software uses Ionic.Zlib");
                        centerText("Ionic.Zlib is licensed under the Microsoft Public License");
                        centerText("You can view the license at:");
                        centerText("https://github.com/jstedfast/Ionic.Zlib/blob/master/License.txt");
                        Console.WriteLine();

                        break;

                    case "quit":
                        shouldExit = true;
                        break;
                }

                Console.WriteLine();
            }
        }

        private static void ConnectionLoop()
        {
            while (!shouldExit)
            {
                wait = new EventWaitHandle(false, EventResetMode.ManualReset);
                mitm.Start();
                mitm.BeginAcceptTcpClient(OnConnect, null);

                wait.WaitOne();
            }
        }

        private static void OnConnect(IAsyncResult ar)
        {
            TcpClient client = mitm.EndAcceptTcpClient(ar);
            wait.Set();

            TcpClient toServer = new TcpClient(IPAddress.Loopback.ToString(), 12346);

            NetworkStream clientStream = client.GetStream();
            NetworkStream serverStream = toServer.GetStream();

            IPAddress clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

            Console.WriteLine("{1} connected to {0}", client.Client.LocalEndPoint.ToString(), clientIP.ToString());

            MITMMessageHandler handler = new MITMMessageHandler(clientStream, serverStream, clientIP.ToString());

            handler.OnClientDisconnected = new Action<MITMMessageHandler>(h =>
            {
                if (ConnectedPlayers.ContainsKey(h.IP))
                    ConnectedPlayers.Remove(h.IP);
            });

            handler.OnClientIdentified = new Action<MITMMessageHandler>(h =>
            {
                if (KnownNames.ContainsKey(h.IP) && !KnownNames[h.IP].Contains(h.Name))
                    KnownNames[h.IP].Add(h.Name);
                else if(!KnownNames.ContainsKey(h.IP))
                {
                    List<string> tmp = new List<string>();
                    tmp.Add(h.Name);
                    KnownNames.Add(h.IP, tmp);
                }
            });

            ConnectedPlayers.Add(handler.IP, handler);

            establishedConnections.Add(handler);
        }

        private static void centerText(String text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }
    }
}
