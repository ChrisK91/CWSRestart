using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace ServerService
{
    public class Helper
    {
        private static System.Timers.Timer timeout;
        public static event EventHandler ServerRestarted;

        private static BackgroundWorker output;
        internal static Process Server;

        /// <summary>
        /// Indicates if the helper is working (for instance restarting the server)
        /// </summary>
        public static bool Working
        {
            get
            {
                return (timeout != null) ? timeout.Enabled : false;
            }
        }

        /// <summary>
        /// Retrieves the external IP
        /// </summary>
        /// <returns>The external IP</returns>
        public async static Task<IPAddress> GetExternalIp()
        {
            WebRequest request = WebRequest.Create(Settings.Instance.IPService);
            WebResponse response = await request.GetResponseAsync();
            string html;

            using (Stream stream = response.GetResponseStream())
            {
                using(StreamReader sr = new StreamReader(stream))
                {
                    html = await sr.ReadToEndAsync();
                }
            }

            IPAddress externalIP;
            if (IPAddress.TryParse(html, out externalIP))
            {
                return externalIP;
            }
            else
            {
                throw new Exception("The external IP could not be retrived");
            }
        }


        /// <summary>
        /// Retrieves the LAN ip
        /// </summary>
        /// <returns>The LAN ip</returns>
        public static async Task<IPAddress> GetLocalIP()
        {
            IPHostEntry host;
            IPAddress localIP = null;
            host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                    break;
                }
            }

            return localIP;
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);   


        /// <summary>
        /// Attempts to send the "q" Key to the StandardInput of the server
        /// </summary>
        public static void SendQuit()
        {
            if (Validator.Instance.IsRunning())
            {
                if(Server == null || (Server.ProcessName != Settings.Instance.ServerProcessName))
                    Server = Process.GetProcessesByName(Settings.Instance.ServerProcessName)[0];

                try
                {
                    if (!Server.StartInfo.UseShellExecute)
                    {
                        Logging.OnLogMessage("Writing to process stream", Logging.MessageType.Info);
                        Server.StandardInput.WriteLine("q");
                    }
                    else
                    {
                        Logging.OnLogMessage("Sending key", Logging.MessageType.Info);
                        SetForegroundWindow(Server.MainWindowHandle);
                        System.Windows.Forms.SendKeys.SendWait("q{ENTER}");
                    }
                }
                catch(Exception ex)
                {
                    Logging.OnLogMessage("Unable to send keypress...", Logging.MessageType.Warning);
                    Logging.OnLogMessage(ex.Message, Logging.MessageType.Error);
                }
            }
            else
            {
                Logging.OnLogMessage("Process not found", Logging.MessageType.Error);
            }
        }

        /// <summary>
        /// Kills the
        /// </summary>
        public static void KillServer()
        {
            if (Server == null || (Server.ProcessName != Settings.Instance.ServerProcessName))
                Server = Process.GetProcessesByName(Settings.Instance.ServerProcessName)[0];

            if(Server != null)
                Server.Kill();

            if (output != null)
                output.CancelAsync();
        }

        /// <summary>
        /// Will restart the server
        /// </summary>
        public static void RestartServer()
        {
            if (Validator.Instance.IsRunning())
            {
                Logging.OnLogMessage("Sending the q-Key", Logging.MessageType.Info);
                SendQuit();
                Logging.OnLogMessage(String.Format("Waiting {0} seconds to see if the server is able to quit", Settings.Instance.Timeout / 1000), Logging.MessageType.Info);
                timeout = new System.Timers.Timer(Settings.Instance.Timeout);
                timeout.AutoReset = false;
                timeout.Elapsed += timeout_Elapsed;
                timeout.Start();

                if (ServerRestarted != null)
                    ServerRestarted(null, null);
            }
            else
            {
                Logging.OnLogMessage("Process not found", Logging.MessageType.Error);
            }
        }

        static void timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Validator.Instance.IsRunning())
            {
                Logging.OnLogMessage("The server is still running. Now we force it to quit.", Logging.MessageType.Info);
                KillServer();
            }
            else
            {
                Logging.OnLogMessage("The server has quit.", Logging.MessageType.Info);
            }

            KillAdditionalProcesses();
            StartServer();
        }

        /// <summary>
        /// Will start the server and allow us to use the StandardInput/Output
        /// </summary>
        public static void StartServer()
        {
            Logging.OnLogMessage("Starting the server", Logging.MessageType.Info);
            ProcessStartInfo pStart = new ProcessStartInfo(Settings.Instance.ServerPath);
            pStart.UseShellExecute = false;

            if (!pStart.UseShellExecute)
            {
                pStart.RedirectStandardInput = true;
                pStart.RedirectStandardOutput = true;
            }

            output = new BackgroundWorker();
            output.DoWork += output_DoWork;

            Server = Process.Start(pStart);
            output.RunWorkerAsync();
        }

        /// <summary>
        /// Will terminate all processes, that are listed in Settins.AdditionalProcesses
        /// </summary>
        private static void KillAdditionalProcesses()
        {
            foreach (string s in Settings.Instance.AdditionalProcesses)
            {
                try
                {
                    Process[] p = Process.GetProcessesByName(s);

                    foreach (Process i in p)
                    {
                        Logging.OnLogMessage(String.Format("Terminating {0}", i.ProcessName), Logging.MessageType.Info);
                        i.Kill();
                    }
                }
                catch (Exception e)
                {
                    Logging.OnLogMessage(e.Message, Logging.MessageType.Error);
                }
            }
        }

        static void output_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Server != null && !Server.StartInfo.UseShellExecute)
            {
                StreamReader tmp = Server.StandardOutput;

                string line;
                while((line = tmp.ReadLine()) != null)
                    if(line.Trim() != "")
                        Logging.OnLogMessage(line, Logging.MessageType.Server);
            }
        }
    }
}
