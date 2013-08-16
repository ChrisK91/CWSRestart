using ServerService.Helper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ServerService
{
    /// <summary>
    /// Describes what is wrong with the server
    /// </summary>
    [Flags]
    public enum ServerErrors
    {
        /// <summary>
        /// The process is dead
        /// </summary>
        ProcessDead = 0x1,
        /// <summary>
        /// At least one connection failed
        /// </summary>
        Connection = 0x2,
    }

    public sealed class Validator : INotifyPropertyChanged
    {
        private static readonly Validator instance = new Validator();
        public event PropertyChangedEventHandler PropertyChanged;

        private Validator() { }

        public static Validator Instance
        {
            get
            {
                return instance;
            }
        }

        private bool working = false;
        /// <summary>
        /// Indicates, if the Validator is currently working
        /// </summary>
        public bool Working
        {
            get
            {
                return working;
            }
            private set
            {
                if (working != value)
                {
                    working = value;
                    notifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Check if the process is running
        /// </summary>
        /// <returns>true if the process is running, otherwise false</returns>
        public static bool IsRunning()
        {
            return Process.GetProcessesByName(Settings.Instance.ServerProcessName).Length != 0;
        }

        /// <summary>
        /// Retrieve, how the server can be accessed
        /// </summary>
        /// <returns>All valid access networks through which the server can be reached</returns>
        public async Task<AccessType> GetAccessType()
        {
            AccessType access = 0;

            if (await IsAccessible(AccessType.Loopback))
                access |= AccessType.Loopback;

            if (await IsAccessible(AccessType.Internet))
                access |= AccessType.Internet;

            if (await IsAccessible(AccessType.LAN))
                access |= AccessType.LAN;

            return access;
        }

        /// <summary>
        /// Determines if the server is accessible through the selected networks
        /// </summary>
        /// <param name="access">The access scheme</param>
        /// <returns>True if the server was accessible through all the specified access networks</returns>
        public async Task<bool> IsAccessible(AccessType access)
        {
            bool accessFailed = false;

            if (access.HasFlag(AccessType.Internet))
            {
                Logging.OnLogMessage("Checking access from the internet", MessageType.Info);

                if (await ServerIsListening(Settings.Instance.Internet, Settings.Instance.Port))
                    Logging.OnLogMessage("Access is possible through the internet", MessageType.Info);
                else
                    accessFailed = true;
            }

            if (access.HasFlag(AccessType.LAN))
            {
                Logging.OnLogMessage("Checking access from the local network", MessageType.Info);

                if (await ServerIsListening(Settings.Instance.LAN, Settings.Instance.Port))
                    Logging.OnLogMessage("Access is possible through the local network", MessageType.Info);
                else
                    accessFailed = true;
            }

            if (access.HasFlag(AccessType.Loopback))
            {
                Logging.OnLogMessage("Checking access via loopback", MessageType.Info);

                if (await ServerIsListening(Settings.Instance.Loopback, Settings.Instance.Port))
                    Logging.OnLogMessage("Access is possible through loopback", MessageType.Info);
                else
                    accessFailed = true;
            }

            return !accessFailed;
        }

        /// <summary>
        /// Determines if the server is listening on the specified address:port
        /// </summary>
        /// <param name="address">the ip</param>
        /// <param name="port">the port</param>
        /// <returns>true if the server is responding</returns>
        private async Task<bool> ServerIsListening(IPAddress address, int port)
        {
            Working = true;
            Logging.OnLogMessage(String.Format("Checking access on {0}:{1}. This might take a minute or two", address.ToString(), port.ToString()), MessageType.Info);
            
            using(TcpClient connection = new TcpClient(AddressFamily.InterNetwork))
            {
                try
                {
                    await connection.ConnectAsync(address, port);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.ConnectionRefused || ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Working = false;
                        Logging.OnLogMessage(String.Format("No connection possible on {0}:{1}", address.ToString(), port.ToString()), MessageType.Error);
                        return false;
                    }
                    else
                    {
                        Working = false;
                        Logging.OnLogMessage(String.Format("An error occured: {0}", ex.Message), MessageType.Error);
                        return false;
                    }
                }
            }
            Working = false;
            return true;
        }

        /// <summary>
        /// Checks if the server is still running
        /// </summary>
        /// <param name="access">The desired access scheme</param>
        /// <returns>true if everything is fine</returns>
        public async Task<ServerErrors> Validates(AccessType access)
        {
            ServerErrors serverHealth = 0;

            if (IsRunning())
            {
                Logging.OnLogMessage("The process is running", MessageType.Info);
            }
            else
            {
                Logging.OnLogMessage("The process is dead", MessageType.Warning);
                serverHealth |= ServerErrors.ProcessDead;
            }

            if (await IsAccessible(access))
            {
                Logging.OnLogMessage("The server is responding fine", MessageType.Info);
            }
            else
            {
                Logging.OnLogMessage("The server is not responding as it should", MessageType.Warning);
                serverHealth |= ServerErrors.Connection;
            }

            return serverHealth;
        }

        /// <summary>
        /// Checks if all settings are set
        /// </summary>
        /// <returns></returns>
        public Task<ServerErrors> Validates()
        {
            return Validates(Settings.Instance.IgnoreAccess);
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
