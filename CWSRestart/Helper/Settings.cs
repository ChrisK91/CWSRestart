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
        Utilities.Settings settings;

        private Settings()
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "CWSRestart.exe.config");
            settings = new Utilities.Settings(file);

            AutostartCWSProtocol = settings.GetAppSettingWithStandardValue("AutostartCWSProtocol", false);
            AutostartStatistics = settings.GetAppSettingWithStandardValue("AutostartStatistics", false);
            AutostartWatcher = settings.GetAppSettingWithStandardValue("AutostartWatcher", false);

            WatcherTimeout = settings.GetAppSettingWithStandardValue("WatcherTimeout", (uint)60);
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
                    settings.SetAppSetting("WatcherTimeout", value);
                }
            }
        }
    }
}
