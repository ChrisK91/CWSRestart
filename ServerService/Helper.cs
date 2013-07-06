using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Timers;

namespace ServerService
{
    public class Helper
    {
        private static System.Timers.Timer timeout;
        public static bool Working
        {
            get
            {
                return (timeout != null) ? timeout.Enabled : false;
            }
        }

        private static System.Timers.Timer output;
        static Process server;

        public async static Task<IPAddress> GetExternalIp()
        {
            WebRequest request = WebRequest.Create(Settings.IPService);
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

        public static void SendQuit()
        {
            if(server == null)
                server = Process.GetProcessesByName(Settings.ServerProcessName)[0];

            try
            {
                server.StandardInput.WriteLine("q");

                if (output != null)
                    output.Stop();
            }
            catch (Exception ex)
            {
                Logging.OnLogMessage("Unable to send keypress...", Logging.MessageType.Warning);
            }
        }

        public static void KillServer()
        {
            Process server = Process.GetProcessesByName(Settings.ServerProcessName)[0];
            server.Kill();
        }

        public static void RestartServer()
        {
            if (Validator.IsRunning())
            {
                Logging.OnLogMessage("Sending the q-Key", Logging.MessageType.Info);
                SendQuit();
                Logging.OnLogMessage(String.Format("Waiting {0} seconds to see if the server is able to quit", Settings.Timeout / 1000), Logging.MessageType.Info);
                timeout = new System.Timers.Timer(Settings.Timeout);
                timeout.AutoReset = false;
                timeout.Elapsed += timeout_Elapsed;
                timeout.Start();
            }
        }

        static void timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Validator.IsRunning())
            {
                Logging.OnLogMessage("The server is still running. Now we force it to quit.", Logging.MessageType.Info);
                KillServer();
            }
            else
            {
                Logging.OnLogMessage("The server has quit.", Logging.MessageType.Info);
            }

            StartServer();
        }

        public static void StartServer()
        {
            Logging.OnLogMessage("Starting the server", Logging.MessageType.Info);
            ProcessStartInfo pStart = new ProcessStartInfo(Settings.ServerPath);
            pStart.UseShellExecute = false;
            pStart.RedirectStandardInput = true;
            pStart.RedirectStandardOutput = true;

            output = new System.Timers.Timer(500);
            output.Elapsed += output_Elapsed;
            output.Start();

            server = Process.Start(pStart);
        }

        static void output_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (server != null)
            {
                StreamReader tmp = server.StandardOutput;

                string line;
                while((line = tmp.ReadLine()) != null)
                    if(line.Trim() != "")
                        Logging.OnLogMessage(line, Logging.MessageType.Server);
            }
        }
    }
}
