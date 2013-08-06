using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CubeWorldMITM.Helper
{
    public sealed class Settings
    {
        private static readonly Settings instance = new Settings();
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        private Settings() {
            MinLevel = getAppSettingWithStandardValue("MinLevel", -1);
            MaxLevel = getAppSettingWithStandardValue("MaxLevel", -1);
            PlayerLimit = getAppSettingWithStandardValue("PlayerLimit", -1);
            StartServer = getAppSettingWithStandardValue("StartServer", false);
            ServerLocation = getAppSettingWithStandardValue("ServerLocation", "");
        }

        public int MinLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public int PlayerLimit { get; private set; }
        public bool StartServer { get; private set; }
        public string ServerLocation { get; private set; }

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
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CubeWorldMITM.exe.config")))
                using (Stream resource = GetType().Assembly.GetManifestResourceStream("CubeWorldMITM.CubeWorldMITM.exe.config"))
                {
                    using (Stream output = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), "CubeWorldMITM.exe.config")))
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
    }
}
