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

            string presetsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "presets");
            string importedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "presets", "imported");

            AutostartCWSProtocol = settings.GetAppSettingWithStandardValue("AutostartCWSProtocol", false);
            AutostartStatistics = settings.GetAppSettingWithStandardValue("AutostartStatistics", false);
            AutostartWatcher = settings.GetAppSettingWithStandardValue("AutostartWatcher", false);
            WatcherTimeout = settings.GetAppSettingWithStandardValue("WatcherTimeout", (uint)60);
            AutoPresetLoading = settings.GetAppSettingWithStandardValue("AutoPresetLoading", false);

            if (!Directory.Exists(presetsDirectory))
                Directory.CreateDirectory(presetsDirectory);

            if (!Directory.Exists(importedDirectory))
                Directory.CreateDirectory(importedDirectory);

            else
            {
                foreach (string f in Directory.EnumerateFiles(presetsDirectory, "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetExtension(f).ToLowerInvariant() == ".xml")
                    {
                        LoadPreset(f);
                        movePreset(f);
                    }
                }
            }

            FileSystemWatcher fsw = new FileSystemWatcher(presetsDirectory, "*.xml");
            fsw.IncludeSubdirectories = false;
            fsw.Created += fsw_Created;
            fsw.Changed += fsw_Created;
            fsw.Renamed += fsw_Renamed;
            fsw.EnableRaisingEvents = true;
        }

        private void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).ToLowerInvariant() == ".xml")
            {
                LoadPreset(e.FullPath);
                movePreset(e.FullPath);
            }
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath).ToLowerInvariant() == ".xml")
            {
                LoadPreset(e.FullPath);
                movePreset(e.FullPath);
            }
        }

        private void movePreset(string filename)
        {
            string importedDirectory = Path.Combine(Directory.GetCurrentDirectory(), "presets", "imported");
            string newFile = Path.Combine(importedDirectory, Path.GetFileName(filename));

            while (File.Exists(newFile))
            {
                Guid g = Guid.NewGuid();
                newFile = String.Format("{0}.{1:N}", newFile, g);
            }

            if(File.Exists(filename))
                File.Move(filename, newFile);
        }

        public void LoadPreset(string filename)
        {
            LoadPreset(filename, filename);
        }

        public void LoadPreset(string filename, string name)
        {
            string message = String.Format("Would you like to load the preset from {0}?", name);

            if (AutoPresetLoading || System.Windows.Forms.MessageBox.Show(message, "Load preset", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2, (System.Windows.Forms.MessageBoxOptions)0x40000) == System.Windows.Forms.DialogResult.Yes)
            {
                try
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

                    if (p.Checks.ContainsKey(Utilities.Settings.Preset.InternetAccess))
                        ServerService.Helper.Settings.Instance.CheckInternet = p.Checks[Utilities.Settings.Preset.InternetAccess];

                    if (p.Checks.ContainsKey(Utilities.Settings.Preset.LANAccess))
                        ServerService.Helper.Settings.Instance.CheckInternet = p.Checks[Utilities.Settings.Preset.LANAccess];

                    if (p.Checks.ContainsKey(Utilities.Settings.Preset.LoopbackAccess))
                        ServerService.Helper.Settings.Instance.CheckInternet = p.Checks[Utilities.Settings.Preset.LoopbackAccess];

                    foreach (string s in p.AdditionalProcesses)
                    {
                        if (!ServerService.Helper.Settings.Instance.AdditionalProcesses.Contains(s))
                            ServerService.Helper.Settings.Instance.AdditionalProcesses.Add(s);
                    }

                    Logging.OnLogMessage(String.Format("Loaded preset {0}", p.Name), ServerService.Logging.MessageType.Info);
                }
                catch (Exception)
                {
                    Logging.OnLogMessage(String.Format("Could not load preset {0}", filename), ServerService.Logging.MessageType.Error);
                }
            }
        }

        public bool AutostartCWSProtocol { get; private set; }
        public bool AutostartStatistics { get; private set; }
        public bool AutostartWatcher { get; private set; }
        public bool AutoPresetLoading { get; private set; }

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
