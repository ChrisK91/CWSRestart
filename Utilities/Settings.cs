using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Net;

namespace Utilities
{
    /// <summary>
    /// A settings class, that reads and writes from/to *.config files
    /// </summary>
    public class Settings
    {
        private string file;

        /// <summary>
        /// Initializes the Settings-class with the given file
        /// </summary>
        /// <param name="ConfigurationFile">The config file. An empty file will be created, if the specified file doesn't exist.</param>
        public Settings(string ConfigurationFile)
        {
            file = ConfigurationFile;

            if (!File.Exists(file))
            {
                using (Stream resource = GetType().Assembly.GetManifestResourceStream("Utilities.Embedded.empty.config"))
                {
                    using (Stream output = File.OpenWrite(file))
                    {
                        resource.CopyTo(output);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the given value to the configuration file
        /// </summary>
        /// <param name="Key">The key of the configuration parameter</param>
        /// <param name="Value">The value</param>
        public void SetAppSetting(string Key, object Value)
        {
            ExeConfigurationFileMap configurationMap = new ExeConfigurationFileMap();
            configurationMap.ExeConfigFilename = file;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configurationMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[Key] != null)
                config.AppSettings.Settings.Remove(Key);

            config.AppSettings.Settings.Add(Key, Value.ToString());
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Reads the value for the given key from the settings.
        /// </summary>
        /// <param name="key">The setting to read</param>
        /// <returns>The value. Null if the setting was not found.</returns>
        public string GetAppSettingValue(string key)
        {
            ExeConfigurationFileMap configurationMap = new ExeConfigurationFileMap();
            configurationMap.ExeConfigFilename = file;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configurationMap, ConfigurationUserLevel.None);

            return config.AppSettings.Settings[key] != null ? config.AppSettings.Settings[key].Value : null;
        }

        /// <summary>
        /// Returns the given setting from the configuration. If the setting is not set, a new one with the fallback value is created.
        /// </summary>
        /// <param name="key">The key to read</param>
        /// <param name="fallback">The default value</param>
        /// <returns>The configuration value if present, otherwise fallback</returns>
        public int GetAppSettingWithStandardValue(string key, int fallback)
        {
            return readOrCreateValue<int>(key, fallback, int.TryParse);
        }

        public bool GetAppSettingWithStandardValue(string key, bool fallback)
        {
            return readOrCreateValue<bool>(key, fallback, Boolean.TryParse);
        }

        public string GetAppSettingWithStandardValue(string key, string fallback)
        {
            if (GetAppSettingValue(key) == null)
            {
                SetAppSetting(key, fallback);
                return fallback;
            }

            return GetAppSettingValue(key);
        }

        public IPAddress GetAppSettingWithStandardValue(string key, IPAddress fallback)
        {
            return readOrCreateValue<IPAddress>(key, fallback, IPAddress.TryParse);
        }

        public uint GetAppSettingWithStandardValue(string key, uint fallback)
        {
            return readOrCreateValue<uint>(key, fallback, uint.TryParse);
        }

        private T readOrCreateValue<T>(string key, T value, TryParseHandler<T> handler)
        {
            if (!(GetAppSettingValue(key) != null && handler(GetAppSettingValue(key), out value)))
            {
                SetAppSetting(key, value);
            }

            return value;
        }

        private delegate bool TryParseHandler<T>(string value, out T result);
        private bool tryParse<T>(string value, out T result, TryParseHandler<T> handler)
        {
            return handler(value, out result);
        }
    }
}
