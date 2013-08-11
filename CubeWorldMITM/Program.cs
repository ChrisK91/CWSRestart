using CubeWorldMITM.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        private static bool autosaveEnabled = false;
        private static EventWaitHandle wait;

        private static List<MITMMessageHandler> establishedConnections = new List<MITMMessageHandler>();

        private static Dictionary<string, List<string>> KnownNames = new Dictionary<string, List<string>>();
        private static Dictionary<string, MITMMessageHandler> ConnectedPlayers = new Dictionary<string, MITMMessageHandler>();

        private static uint port = 12345;
        private static uint serverPort = 12346;

        private static IPAddress mitmIP = IPAddress.Any;
        private static IPAddress cubeWorldIP = IPAddress.Loopback;

        private static System.Timers.Timer autosave = new System.Timers.Timer();

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
            Console.WriteLine("You can start this program with CubeWorldMITM [port1] [port2] [internal ip] [server ip]");
            Console.WriteLine("\t [port1] - the port on which the MITM server should listen");
            Console.WriteLine("\t \t - default 12345");
            Console.WriteLine();
            Console.WriteLine("\t [port2] - the port on which the cubeworld server is listening");
            Console.WriteLine("\t \t - default 12346");
            Console.WriteLine();
            Console.WriteLine("\t [internal ip] - the ip on wich the MITM should listen");
            Console.WriteLine("\t \t - defaults to any IP");
            Console.WriteLine();
            Console.WriteLine("\t [server ip] - the ip on wich the CubeWorld Server is listening");
            Console.WriteLine("\t \t - defaults to loopback");
            Console.WriteLine();
            Console.WriteLine();

            parseSettings(args);

            if (Helper.Settings.Instance.StartServer == true)
            {
                if (File.Exists(Helper.Settings.Instance.ServerLocation))
                    Helper.Launcher.LaunchServerConfigured(Helper.Settings.Instance.ServerLocation);
                else
                    Console.WriteLine("The specified file does not exist: {0}", Helper.Settings.Instance.ServerLocation);

                Console.WriteLine();
                Console.WriteLine();
            }

            mitm = new TcpListener(mitmIP, (int)port);

            Thread listenerThread = new Thread(new ThreadStart(ConnectionLoop));
            listenerThread.Start();

            messageLoop();

            shouldExit = true;

            Dictionary<string, MITMMessageHandler> tmp = new Dictionary<string, MITMMessageHandler>(ConnectedPlayers);
            foreach (KeyValuePair<string, MITMMessageHandler> h in tmp)
                if (h.Value.Connected)
                    h.Value.Disconnect();

            listenerThread.Abort();
        }

        private static void parseSettings(string[] args)
        {
            if (args.Count() >= 1)
            {
                if (UInt32.TryParse(args[0], out port) && port > 0)
                {
                    Console.WriteLine("MITM port: {0}", port);
                }
                else
                {
                    port = 12345;
                }
            }

            if (args.Count() >= 2)
            {
                if (UInt32.TryParse(args[1], out serverPort) && serverPort > 0)
                {
                    Console.WriteLine("CubeWorld server port: {0}", serverPort);
                }
                else
                {
                    serverPort = 12346;
                }
            }

            if (args.Count() >= 3)
            {
                if (IPAddress.TryParse(args[2], out mitmIP))
                    Console.WriteLine("MITM IP: {0}", mitmIP.ToString());
            }

            if (args.Count() >= 4)
            {
                if (IPAddress.TryParse(args[3], out cubeWorldIP))
                    Console.WriteLine("CubeWorld Server IP: {0}", cubeWorldIP.ToString());
            }
        }

        private static void messageLoop()
        {
            while (shouldExit != true)
            {
                Console.WriteLine("Enter one of the following options");
                Console.WriteLine("c - to receive a list of every currently connected player");
                Console.WriteLine("n - to receive a list of every name that we know and the corresponding IP");
                Console.WriteLine("a - about");
                Console.WriteLine("s - to save a list of every known player");
                Console.WriteLine("t - to enable/disable autosaving of playernames");
                Console.WriteLine("x - to configure CWSRestart via CWSProtocol");
                Console.WriteLine("quit - to quit");

                string action = Console.ReadLine();
                Console.Clear();

                switch (action.ToLower())
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

                            foreach (string s in h.Value)
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

                    case "s":
                        centerText("--------------------------");
                        centerText("Saving players");
                        centerText("--------------------------");

                        savePlayers();

                        break;

                    case "t":
                        if (autosaveEnabled)
                        {
                            autosaveEnabled = false;
                            autosave.Enabled = false;
                            Console.WriteLine("Autosaving is now disabled.");
                        }
                        else
                        {
                            autosave.Elapsed += autosave_Elapsed;

                            double timeout;
                            Console.WriteLine("Enter the desired interval in seconds:");
                            if (Double.TryParse(Console.ReadLine(), out timeout) && timeout > 0)
                            {
                                timeout = timeout * 1000;
                                autosave.Interval = timeout;
                                autosave.AutoReset = true;
                                autosave.Enabled = true;
                                autosaveEnabled = true;

                                Console.WriteLine("Autosaving is now enabled.");
                            }
                            else
                            {
                                Console.WriteLine("The timeout is not valid.");
                            }
                        }
                        break;

                    case "x":

                        Dictionary<string, bool> checks = new Dictionary<string, bool>();
                        checks.Add(Utilities.Settings.Preset.InternetAccess, false);
                        checks.Add(Utilities.Settings.Preset.LANAccess, true);
                        checks.Add(Utilities.Settings.Preset.LoopbackAccess, true);

                        Utilities.Settings.Preset p = new Utilities.Settings.Preset("CWSProtocol", Helper.Launcher.ServerLocation, Helper.Launcher.ServerName, (int?)serverPort, null, null, null, checks);

                        string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        Guid name = Guid.NewGuid();

                        folder = Path.Combine(folder, "CubeWorldMITM");

                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        string filename = Path.Combine(folder, name.ToString() + ".xml");

                        p.Save(filename);

                        CWSProtocol.Client c = new CWSProtocol.Client("CubeWorldMITM");
                        if (!c.SendPreset(filename, true))
                        {
                            File.Delete(filename);
                            Console.WriteLine("Could not send preset.");
                        }

                        break;

                    case "quit":
                        shouldExit = true;
                        break;
                }

                Console.WriteLine();
            }
        }

        private static void savePlayers()
        {
            string filename = String.Format("{0}.{1}", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "txt");
            string folder = Path.Combine(Environment.CurrentDirectory.ToString(), "output");
            string output = Path.Combine(folder, filename);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!File.Exists(output))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(output, FileMode.Create, FileAccess.Write, FileShare.None)))
                    {
                        sw.WriteLine("Known players:");

                        Dictionary<string, List<string>> tmp = new Dictionary<string, List<string>>(KnownNames);

                        foreach (KeyValuePair<string, List<string>> k in tmp)
                        {
                            sw.Write("{0}: ", k.Key);
                            foreach (string s in k.Value)
                            {
                                sw.Write("{0}, ", s);
                            }
                            sw.Write(Environment.NewLine);
                        }
                    }
                    Console.WriteLine("Known players saved to {0}.", output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured: {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("The file {0} already exists.", output);
            }
        }

        static void autosave_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            autosave.Enabled = false;

            savePlayers();

            if (!shouldExit && autosaveEnabled)
                autosave.Enabled = true;
        }

        private static void ConnectionLoop()
        {
            try
            {
                while (!shouldExit)
                {
                    wait = new EventWaitHandle(false, EventResetMode.ManualReset);
                    mitm.Start();
                    mitm.BeginAcceptTcpClient(OnConnect, null);

                    wait.WaitOne();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Could not create MITM server on port 12345.");
                Console.WriteLine("You can run \"netstat -a -b -n -o\" to check if something is already running on port 12345");
                shouldExit = true;
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        private static void OnConnect(IAsyncResult ar)
        {
            TcpClient client = null;
            try
            {
                client = mitm.EndAcceptTcpClient(ar);
                if (Helper.Settings.Instance.PlayerLimit < 0 || ConnectedPlayers.Count <= Helper.Settings.Instance.PlayerLimit)
                {
                    wait.Set();

                    TcpClient toServer = new TcpClient();
                    toServer.Connect(cubeWorldIP, (int)serverPort);

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
                        else if (!KnownNames.ContainsKey(h.IP))
                        {
                            List<string> tmp = new List<string>();
                            tmp.Add(h.Name);
                            KnownNames.Add(h.IP, tmp);
                        }

                        if (!levelAllowed(h.Level))
                        {
                            h.Disconnect();
                            Console.WriteLine("{0} was kicked, because the level {1} is not allowed.", h.Name, h.Level);
                        }
                    });

                    if (ConnectedPlayers.ContainsKey(handler.IP))
                    {
                        ConnectedPlayers[handler.IP].Disconnect();
                        ConnectedPlayers.Remove(handler.IP);
                    }
                    ConnectedPlayers.Add(handler.IP, handler);

                    establishedConnections.Add(handler);
                }
                else
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                    Console.WriteLine("An error occured while connecting to your server. Is the CubeWorld server running?");
                else
                    Console.WriteLine("An error occured: {0}", ex.Message);

                if(client != null && client.Connected)
                    client.Close();
            }
        }

        private static bool levelAllowed(uint p)
        {
            if (Helper.Settings.Instance.MinLevel < Helper.Settings.Instance.MaxLevel)
            {
                return (p >= Helper.Settings.Instance.MinLevel) && (p <= Helper.Settings.Instance.MaxLevel);
            }
            return true;
        }

        private static void centerText(String text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Console.WriteLine(text);
        }
    }
}
