using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ServerService.Helper
{
    /// <summary>
    /// This class contains all settings for the Server Service
    /// </summary>
    public sealed class Settings : INotifyPropertyChanged
    {
        private static readonly Settings instance = new Settings();
        private Utilities.Settings.Settings settings;

        private Settings() {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "ServerService.dll.config");
            settings = new Utilities.Settings.Settings(file);

            IPService = new Uri(settings.GetAppSettingWithStandardValue("IPService", "http://bot.whatismyipaddress.com/"));
            Loopback = settings.GetAppSettingWithStandardValue("Loopback", IPAddress.Loopback);
            Port = settings.GetAppSettingWithStandardValue("Port", 12345);

            IPAddress tmp;

            if (settings.GetAppSettingValue("LAN") != null && IPAddress.TryParse(settings.GetAppSettingValue("LAN"), out tmp))
                LAN = tmp;

            if (settings.GetAppSettingValue("Internet") != null && IPAddress.TryParse(settings.GetAppSettingValue("Internet"), out tmp))
                Internet = tmp;

            ServerProcessName = settings.GetAppSettingWithStandardValue("ServerProcessName", "Server");
            Timeout = settings.GetAppSettingWithStandardValue("Timeout", 10000);
            ServerPath = settings.GetAppSettingWithStandardValue("ServerPath", "");

            CheckLoopback = settings.GetAppSettingWithStandardValue("CheckLoopback", false);
            CheckLAN = settings.GetAppSettingWithStandardValue("CheckLAN", true);
            CheckInternet = settings.GetAppSettingWithStandardValue("CheckInternet", true);

            DoNotRedirectOutput = settings.GetAppSettingWithStandardValue("DoNotRedirectOutput", false);
            StatisticsInterval = settings.GetAppSettingWithStandardValue("StatisticsInterval", 1000);
            SaveStatisticsEvery = settings.GetAppSettingWithStandardValue("SaveStatisticsEvery", 5);

            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SessionActive = settings.GetAppSettingWithStandardValue("SessionActiveDefault", true);
            BypassSendQuit = settings.GetAppSettingWithStandardValue("BypassSendQuit", false);
        }

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
            {
                SessionActive = true;
                Logging.OnLogMessage("The PC has been unlocked", Logging.MessageType.Info);
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                SessionActive = false;
                Logging.OnLogMessage("The PC has been locked", Logging.MessageType.Info);
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.RemoteDisconnect)
            {
                SessionActive = false;
                Logging.OnLogMessage("A remote connection has been terminated", Logging.MessageType.Info);
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.RemoteConnect)
            {
                SessionActive = true;
                Logging.OnLogMessage("A remote connection has been initiated", Logging.MessageType.Info);
            }
        }

        public bool SessionActive { get; private set; }

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private int saveStatisticsEvery;
        public int SaveStatisticsEvery
        {
            get
            {
                return saveStatisticsEvery;
            }
            set
            {
                if (value != saveStatisticsEvery)
                {
                    saveStatisticsEvery = value;
                    notifyPropertyChanged();
                    settings.SetAppSetting("SaveStatisticsEvery", value);
                }
            }
        }

        private int statisticsInterval;
        public int StatisticsInterval
        {
            get
            {
                return statisticsInterval;
            }
            set
            {
                if (value != statisticsInterval)
                {
                    statisticsInterval = value;
                    notifyPropertyChanged();
                    settings.SetAppSetting("StatisticsInterval", value);
                }
            }
        }

        #region IPService
        private Uri ipservice;
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
                    settings.SetAppSetting("IPService", value.ToString());
                }
            }
        }
        #endregion

        #region Loopback
        private IPAddress loopback;

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
                    settings.SetAppSetting("Loopback", value);
                }
            }
        }
        #endregion

        #region Port
        private int port;
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
                    settings.SetAppSetting("Port", value);
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
                    settings.SetAppSetting("LAN", value);
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
                    settings.SetAppSetting("Internet", value);
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
                    settings.SetAppSetting("ServerProcessName", value);
                }
            }
        }
        #endregion

        #region Timeout
        private int timeout;
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
                    settings.SetAppSetting("Timeout", value);
                }
            }
        }
        #endregion

        #region ServerPath
        private string serverPath;
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
                    settings.SetAppSetting("ServerPath", value);
                }
            }
        }
        #endregion

        #region IgnoreAccessSettings

        private bool checkLoopback;
        private bool checkInternet;
        private bool checkLAN;
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

                    settings.SetAppSetting("CheckLoopback", value.ToString());
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
                if (checkInternet != value)
                {
                    checkInternet = value;
                    if (checkInternet)
                        ignoreAccess |= AccessType.Internet;
                    else
                        ignoreAccess ^= AccessType.Internet;
                    notifyPropertyChanged();
                    notifyPropertyChanged("IgnoreAccess");

                    settings.SetAppSetting("CheckInternet", value.ToString());
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

                    settings.SetAppSetting("CheckLAN", value.ToString());
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

        public bool BypassSendQuit { get; set; }

        private bool doNotRedirectOutput;
        public bool DoNotRedirectOutput
        {
            get
            {
                return doNotRedirectOutput;
            }
            set
            {
                if (doNotRedirectOutput != value)
                {
                    doNotRedirectOutput = value;
                    notifyPropertyChanged();

                    settings.SetAppSetting("DoNotRedirectOutput", value.ToString());
                }
            }
        }
        #region AdditionalProcesses
        private ObservableCollection<string> additionalProcesses = new ObservableCollection<string>();
        public ObservableCollection<string> AdditionalProcesses
        {
            get
            {
                return additionalProcesses;
            }
            set
            {
                if (additionalProcesses != value)
                {
                    additionalProcesses = value;
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
