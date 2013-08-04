using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CWSRestart.Helper
{
    internal class Settings
    {
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        private static Settings instance = new Settings();

        private Settings()
        {
            createSettingsIfNotExists();

            AutostartCWSProtocol = getAppSettingWithStandardValue("AutostartCWSProtocol", false);
            AutostartStatistics = getAppSettingWithStandardValue("AutostartStatistics", false);
            AutostartWatcher = getAppSettingWithStandardValue("AutostartWatcher", false);

            WatcherTimeout = (uint)getAppSettingWithStandardValue("WatcherTimeout", 60);
        }

        public bool AutostartCWSProtocol { get; private set; }
        public bool AutostartStatistics { get; private set; }
        public bool AutostartWatcher { get; private set; }

        private uint watchertimeout = 60;
        public uint WatcherTimeout
        {
            get
            {
                return watchertimeout;
            }
            set
            {
                if (watchertimeout != value)
                {
                    watchertimeout = value;
                    setAppSetting("WatcherTimeout", (int)value);
                }
            }
        }

        private string getAppSettingWithStandardValue(string key, string fallback)
        {
            if (getAppSetting(key) != null)
                return getAppSetting(key);

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

        private void createSettingsIfNotExists()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CWSRestart.exe.config")))
                using (Stream resource = GetType().Assembly.GetManifestResourceStream("CWSRestart.CWSRestart.exe.config"))
                {
                    using (Stream output = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), "CWSRestart.exe.config")))
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

        private void setAppSetting(string key, int fallback)
        {
            setAppSetting(key, fallback.ToString());
        }
    }
}
