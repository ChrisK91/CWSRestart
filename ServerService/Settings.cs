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

        #region IPService
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
                if (ipservice != value)
                {
                    ipservice = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Loopback
        private IPAddress loopback = IPAddress.Loopback;

        /// <summary>
        /// The Loopback Network IP
        /// </summary>
        public IPAddress Loopback
        {
            get
            {
                return loopback;
            }
            set
            {
                if (loopback != value)
                {
                    loopback = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Port
        private int port = 12345;
        /// <summary>
        /// The port on which the server is listening
        /// </summary>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                if (port != value)
                {
                    port = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        #region LAN
        private IPAddress lan;

        /// <summary>
        /// The local network IP
        /// </summary>
        public IPAddress LAN
        {
            get
            {
                return lan;
            }
            set
            {
                if (lan != value)
                {
                    lan = value;
                    notifyPropertyChanged();
                    Revalidate();
                }
            }
        }
        #endregion

        #region Internet
        private IPAddress internet;

        /// <summary>
        /// The global IP
        /// </summary>
        public IPAddress Internet
        {
            get
            {
                return internet;
            }
            set
            {
                if (internet != value)
                {
                    internet = value;
                    notifyPropertyChanged();
                    Revalidate();
                }
            }
        }
        #endregion

        #region ServerProcessName
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
                if (serverProcessName != value)
                {
                    serverProcessName = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Timeout
        private int timeout = 10000;
        /// <summary>
        /// The timeout to wait for the server to quit, before we try killing it
        /// </summary>
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                if (timeout != value)
                {
                    timeout = value;
                    notifyPropertyChanged();
                }
            }
        }
        #endregion

        #region ServerPath
        private string serverPath = "";
        /// <summary>
        /// The location of the server exectuable
        /// </summary>
        public string ServerPath
        {
            get
            {
                return serverPath;
            }
            set
            {
                if (serverPath != value)
                {
                    serverPath = value;
                    notifyPropertyChanged();
                    Revalidate();
                }
            }
        }
        #endregion

        #region IgnoreAccessSettings

        private bool checkLoopback = false;
        private bool checkInternet = false;
        private bool checkLAN = false;
        private AccessType ignoreAccess = 0;

        public bool CheckLoopback
        {
            get
            {
                return checkLoopback;
            }
            set
            {
                if (checkLoopback != value)
                {
                    checkLoopback = value;
                    if (checkLoopback)
                        ignoreAccess |= AccessType.Loopback;
                    else
                        ignoreAccess ^= AccessType.Loopback;
                    notifyPropertyChanged();
                    notifyPropertyChanged("IgnoreAccess");
                }
            }
        }

        public bool CheckInternet
        {
            get
            {
                return checkInternet;
            }
            set
            {
                if (checkInternet)
                {
                    checkInternet = value;
                    if (checkInternet)
                        ignoreAccess |= AccessType.Internet;
                    else
                        ignoreAccess ^= AccessType.Internet;
                    notifyPropertyChanged();
                    notifyPropertyChanged("IgnoreAccess");
                }
            }
        }

        public bool CheckLAN
        {
            get
            {
                return checkLAN;
            }
            set
            {
                if (checkLAN != value)
                {
                    checkLAN = value;
                    if (checkLAN)
                        ignoreAccess |= AccessType.LAN;
                    else
                        ignoreAccess ^= AccessType.LAN;
                    notifyPropertyChanged();
                    notifyPropertyChanged("IgnoreAccess");
                }
            }
        }

        public AccessType IgnoreAccess
        {
            get
            {
                return ignoreAccess;
            }
            set
            {
                if (ignoreAccess != value)
                {
                    CheckInternet = (value.HasFlag(AccessType.Internet)) ? true : false;
                    CheckLAN = (value.HasFlag(AccessType.LAN)) ? true : false;
                    CheckLoopback = (value.HasFlag(AccessType.Loopback)) ? true : false;

                    ignoreAccess = value;
                    notifyPropertyChanged();
                }
            }
        }

        #endregion

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

        private bool validates = false;
        /// <summary>
        /// Indicates if all settings are set properly
        /// </summary>
        public bool Validates
        {
            get
            {
                return validates;
            }
            private set
            {
                if (validates != value)
                {
                    validates = value;
                    notifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Check if all settings are set
        /// </summary>
        /// <returns>True if everything is fine</returns>
        public bool Revalidate()
        {
            Validates = false;

            if (File.Exists(ServerPath) && (LAN != null) && (Internet != null))
                Validates = true;

            return Validates;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
