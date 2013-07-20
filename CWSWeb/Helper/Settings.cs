using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    class Settings
    {
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                    instance = new Settings();

                return instance;
            }
        }

        public int LinesToRead;

        private Settings()
        {
            LinesToRead = getAppSettingWithStandardValue("LinesToRead", 201);
        }

        public CWSProtocol.Client Client;

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
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CWSWeb.exe.config")))
                using (Stream resource = GetType().Assembly.GetManifestResourceStream("CWSWeb.CWSWeb.exe.config"))
                {
                    using (Stream output = File.OpenWrite(Path.Combine(Directory.GetCurrentDirectory(), "CWSWeb.exe.config")))
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
