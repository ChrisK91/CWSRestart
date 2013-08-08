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
        Utilities.Settings.Settings settings;

        private Settings()
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "CWSRestart.exe.config");
            settings = new Utilities.Settings.Settings(file);

            AutostartCWSProtocol = settings.GetAppSettingWithStandardValue("AutostartCWSProtocol", false);
            AutostartStatistics = settings.GetAppSettingWithStandardValue("AutostartStatistics", false);
            AutostartWatcher = settings.GetAppSettingWithStandardValue("AutostartWatcher", false);

            WatcherTimeout = settings.GetAppSettingWithStandardValue("WatcherTimeout", (uint)60);
        }

        public void LoadPreset(string filename)
        {
            LoadPreset(filename, filename);
        }

        public void LoadPreset(string filename, string name)
        {
            string message = String.Format("Would you like to load the preset from {0}?", name);

            if (System.Windows.Forms.MessageBox.Show(message, "Load preset", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Utilities.Settings.Preset p = Utilities.Settings.Preset.Load(filename);

                if (p.ProcessName != null)
                    ServerService.Helper.Settings.Instance.ServerProcessName = p.ProcessName;

                if (p.ServerLocation != null)
                    ServerService.Helper.Settings.Instance.ServerPath = p.ServerLocation;

                if (p.Port != null)
                    ServerService.Helper.Settings.Instance.Port = (int)p.Port;

                if (p.DoNotRedirectOutput != null)
                    ServerService.Helper.Settings.Instance.DoNotRedirectOutput = (bool)p.DoNotRedirectOutput;

                if (p.BypassSendQuit != null)
                    ServerService.Helper.Settings.Instance.BypassSendQuit = (bool)p.BypassSendQuit;

                if (p.Checks.ContainsKey("Internet"))
                    ServerService.Helper.Settings.Instance.CheckInternet = p.Checks["Internet"];

                if (p.Checks.ContainsKey("LAN"))
                    ServerService.Helper.Settings.Instance.CheckInternet = p.Checks["LAN"];

                if (p.Checks.ContainsKey("Loopback"))
                    ServerService.Helper.Settings.Instance.CheckInternet = p.Checks["Loopback"];

                foreach (string s in p.AdditionalProcesses)
                {
                    if (!ServerService.Helper.Settings.Instance.AdditionalProcesses.Contains(s))
                        ServerService.Helper.Settings.Instance.AdditionalProcesses.Add(s);
                }
            }
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
