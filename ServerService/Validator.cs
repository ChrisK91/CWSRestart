using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    public class Validator
    {
        [Flags]
        public enum AccessType
        {
            Loopback = 0x1,
            LAN = 0x2,
            Internet = 0x4
        }

        public static string ServerProcessName = "Server";

        public static bool ProcessRunning()
        {
            return Process.GetProcessesByName(Validator.ServerProcessName).Length != 0;
        }

        public static Task<AccessType> GetAccessType(IPAddress lan, IPAddress internet)
        {
            return GetAccessType(lan, internet, Helper.Loopback);
        }

        public static Task<AccessType> GetAccessType(string lan, string internet)
        {
            return GetAccessType(lan, internet, IPAddress.Loopback.ToString());
        }

        public static Task<AccessType> GetAccessType(string lan, string internet, string loopback)
        {
            IPAddress local;
            IPAddress global;
            IPAddress localhost;

            if (IPAddress.TryParse(lan, out local) && IPAddress.TryParse(internet, out global) && IPAddress.TryParse(loopback, out localhost))
                return GetAccessType(localhost, global, localhost);
            else
                throw new Exception("At least one IP Address could not be parsed");
        }

        public async static Task<AccessType> GetAccessType(IPAddress lan, IPAddress internet, IPAddress loopback)
        {
            AccessType result = 0;

            if (await ServerIsListening(lan, Helper.Port))
                result |= AccessType.LAN;

            if (await ServerIsListening(internet, Helper.Port))
                result |= AccessType.Internet;

            if (await ServerIsListening(loopback, Helper.Port))
                result |= AccessType.Loopback;

            return result;
        }

        private async static Task<bool> ServerIsListening(IPAddress address, int port)
        {
            using(TcpClient connection = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    await connection.ConnectAsync(address, port);
                }
                catch (SocketException ex)
                {
                    if(ex.SocketErrorCode == SocketError.ConnectionRefused || ex.SocketErrorCode == SocketError.TimedOut)
                        return false;
                }
            }
            return true;
        }
    }
}
