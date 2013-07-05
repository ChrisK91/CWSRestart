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

namespace ServerService
{
    public class Helper
    {
        public static Uri IPService = new Uri("http://bot.whatismyipaddress.com/");
        public static IPAddress Loopback = IPAddress.Loopback;
        public static int Port = 12345;

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr hWnd);

        public async static Task<IPAddress> GetExternalIp()
        {
            WebRequest request = WebRequest.Create(IPService);
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
            Process server = Process.GetProcessesByName(Validator.ServerProcessName)[0];
            IntPtr pointer = server.Handle;
            SetForegroundWindow(pointer);
            SendKeys.SendWait("{q}");
        }

        public static void KillServer()
        {
            Process server = Process.GetProcessesByName(Validator.ServerProcessName)[0];
            server.Kill();
        }
    }
}
