using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ServerService
{
    /// <summary>
    /// This class contains all settings for the Server Service
    /// </summary>
    public sealed class Settings : INotifyPropertyChanged
    {
        private static readonly Settings instance = new Settings();

        private Settings() {
            createSettingsIfNotExists();

            IPService = new Uri(getAppSettingWithStandardValue("IPService", "http://bot.whatismyipaddress.com/"));
            Loopback = getAppSettingWithStandardValue("Loopback", IPAddress.Loopback);
            Port = getAppSettingWithStandardValue("Port", 12345);
            LAN = getAppSettingIP("LAN");
            Internet = getAppSettingIP("Internet");
            ServerProcessName = getAppSettingWithStandardValue("ServerProcessName", "Server");
            Timeout = getAppSettingWithStandardValue("Timeout", 10000);
            ServerPath = getAppSettingWithStandardValue("ServerPath", "");

            CheckLoopback = getAppSettingWithStandardValue("CheckLoopback", false);
            CheckLAN = getAppSettingWithStandardValue("CheckLAN", true);
            CheckInternet = getAppSettingWithStandardValue("CheckInternet", true);

            DoNotRedirectOutput = getAppSettingWithStandardValue("DoNotRedirectOutput", false);
            StatisticsInterval = getAppSettingWithStandardValue("StatisticsInterval", 1000);
            SaveStatisticsEvery = getAppSettingWithStandardValue("SaveStatisticsEvery", 5);

            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SessionActive = getAppSettingWithStandardValue("SessionActiveDefault", true);
            BypassSendQuit = getAppSettingWithStandardValue("BypassSendQuit", false);
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
                    setAppSetting("SaveStatisticsEvery", value);
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
                    setAppSetting("StatisticsInterval", value);
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
                    setAppSetting("IPService", value.ToString());
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
                    setAppSetting("Loopback", value);
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
                    setAppSetting("Port", value);
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
                    setAppSetting("LAN", value);
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
                    setAppSetting("Internet", value);
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
                    setAppSetting("ServerProcessName", value);
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
                    setAppSetting("Timeout", value);
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
                    setAppSetting("ServerPath", value);
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

                    setAppSetting("CheckLoopback", value.ToString());
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

                    setAppSetting("CheckInternet", value.ToString());
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

                    setAppSetting("CheckLAN", value.ToString());
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

                    setAppSetting("DoNotRedirectOutput", value.ToString());
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

        private string getAppSettingWithStandardValue(string key, string fallback)
        {
            if (getAppSetting(key) != null)
                return getAppSetting(key);

            setAppSetting(key, fallback);
            return fallback;
        }

        private IPAddress getAppSettingWithStandardValue(string key, IPAddress fallback)
        {
            IPAddress ret;
            if (getAppSetting(key) != null && IPAddress.TryParse(getAppSetting(key), out ret))
                return ret;

            setAppSetting(key, fallback);
            return fallback;
        }

        private bool getAppSettingWithStandardValue(string key, bool fallback)
        {
            bool ret;
            if (getAppSetting(key) != null && Boolean.TryParse(getAppSetting(key), out ret))
                return ret;

            setAppSetting(key, fallback.ToString());
            return fallback;
        }

        private int getAppSettingWithStandardValue(string key, int fallback)
        {
            int ret;
            if (getAppSetting(key) != null && Int32.TryParse(getAppSetting(key), out ret))
                return ret;

            setAppSetting(key, fallback);
            return fallback;
        }

        private string getAppSetting(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            return config.AppSettings.Settings[key] != null ? config.AppSettings.Settings[key].Value : null; 
        }

        private IPAddress getAppSettingIP(string key)
        {
            IPAddress ret;
            if (IPAddress.TryParse((string)getAppSetting(key), out ret))
                return ret;
            return null;
        }

        private void createSettingsIfNotExists()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ServerService.dll.config")))
                using (Stream resource = GetType().Assembly.GetManifestResourceStream("ServerService.ServerService.dll.config"))
                {
                    using (Stream output = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), "ServerService.dll.config")))
                    {
                        resource.CopyTo(output);
                    }
                }
        }

        private void setAppSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);

            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
            }

            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void setAppSetting(string key, IPAddress fallback)
        {
            setAppSetting(key, fallback.ToString());
        }


        private void setAppSetting(string key, int fallback)
        {
            setAppSetting(key, fallback.ToString());
        }

        public bool BypassSendQuit { get; private set; }
    }
}
