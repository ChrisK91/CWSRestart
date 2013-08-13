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

        private Utilities.Settings.Settings settings;

        private Settings() {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "CubeWorldMITM.exe.config");
            settings = new Utilities.Settings.Settings(path);

            MinLevel = settings.GetAppSettingWithStandardValue("MinLevel", -1);
            MaxLevel = settings.GetAppSettingWithStandardValue("MaxLevel", -1);
            PlayerLimit = settings.GetAppSettingWithStandardValue("PlayerLimit", -1);
            StartServer = settings.GetAppSettingWithStandardValue("StartServer", false);
            ServerLocation = settings.GetAppSettingWithStandardValue("ServerLocation", "");
            AutoIdentifyPlayers = settings.GetAppSettingWithStandardValue("AutoIdentifyPlayers", false);
        }

        public int MinLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public int PlayerLimit { get; private set; }
        public bool StartServer { get; private set; }
        public string ServerLocation { get; private set; }
        public bool AutoIdentifyPlayers { get; private set; }
    }
}
