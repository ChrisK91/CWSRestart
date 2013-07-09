using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

namespace ServerService
{
    /// <summary>
    /// This class contains all settings for the Server Service
    /// </summary>
    public sealed class Settings : INotifyPropertyChanged
    {
        private static readonly Settings instance = new Settings();

        private Settings() { }

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private Uri ipservice = new Uri("http://bot.whatismyipaddress.com/");

        /// <summary>
        /// A service that can be used to retrieve the current (global) IP
        /// </summary>
        public Uri IPService
        {
            get
            {
                return ipservice;
            }
            set
            {
                ipservice = value;
                notifyPropertyChanged();
            }
        }

        /// <summary>
        /// The Loopback IP
        /// </summary>
        private IPAddress loopback = IPAddress.Loopback;
        public IPAddress Loopback
        {
            get
            {
                return loopback;
            }
            set
            {
                loopback = value;
                notifyPropertyChanged();
                notifyPropertyChanged("LoopbackAddress");
            }
        }

        public string LoopbackAddress
        {
            get
            {
                return Loopback.ToString();
            }
            set
            {
                IPAddress tmp;
                if(IPAddress.TryParse(value, out tmp))
                    Loopback = tmp;

                notifyPropertyChanged();
            }
        }

        /// <summary>
        /// The port on which the server is listening
        /// </summary>
        public int Port = 12345;

        /// <summary>
        /// The local network IP
        /// </summary>
        public IPAddress LAN;

        /// <summary>
        /// The global IP
        /// </summary>
        public IPAddress Internet;


        private string serverProcessName = "Server";
        /// <summary>
        /// The server process name
        /// </summary>
        public string ServerProcessName
        {
            get
            {
                return serverProcessName;
            }
            set
            {
                serverProcessName = value;
                notifyPropertyChanged();
            }
        }

        /// <summary>
        /// The timeout to wait for the server to quit, before we try killing it
        /// </summary>
        public int Timeout = 10000;


        /// <summary>
        /// The location of the server exectuable
        /// </summary>
        public string ServerPath = "";

        /// <summary>
        /// Describes some access locations
        /// </summary>
        [Flags]
        public enum AccessType
        {
            /// <summary>
            /// Access on the loopback network (120.0.0.1/localhost)
            /// </summary>
            Loopback = 0x1,
            /// <summary>
            /// Access in the LAN (eg. 192.168.*.*)
            /// </summary>
            LAN = 0x2,
            /// <summary>
            /// Access from the world wide web
            /// </summary>
            Internet = 0x4
        }


        /// <summary>
        /// Check if all settings are set
        /// </summary>
        /// <returns>True if everything is fine</returns>
        public bool Validate()
        {
            if (File.Exists(ServerPath) && (LAN != null) && (Internet != null))
                return true;

            return false;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
