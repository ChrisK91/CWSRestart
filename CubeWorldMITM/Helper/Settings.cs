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
    /// <summary>
    /// Provides handling for the configuration
    /// </summary>
    internal sealed class Settings
    {
        private static readonly Settings instance = new Settings();

        /// <summary>
        /// Accesses the singleton
        /// </summary>
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Manages the *.config file
        /// </summary>
        private Utilities.Settings settings;

        /// <summary>
        /// Handles errorlogging to disc and console
        /// </summary>
        public Utilities.Logging.MultiLogger Logger
        {
            get;
            private set;
        }

        private Settings()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "CubeWorldMITM.exe.config");

            Logger = new Utilities.Logging.MultiLogger();
            settings = new Utilities.Settings(path);

            Utilities.Logging.ConsoleLogger clog = new Utilities.Logging.ConsoleLogger();
            Utilities.Logging.FileLogger flog = new Utilities.Logging.FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "mitm.log"));

            MinLevel = settings.GetAppSettingWithStandardValue("MinLevel", -1);
            MaxLevel = settings.GetAppSettingWithStandardValue("MaxLevel", -1);
            MinHP = settings.GetAppSettingWithStandardValue("MinHP", -1f);
            MaxHP = settings.GetAppSettingWithStandardValue("MaxHP", -1f);
            PlayerLimit = settings.GetAppSettingWithStandardValue("PlayerLimit", -1);
            StartServer = settings.GetAppSettingWithStandardValue("StartServer", false);
            ServerLocation = settings.GetAppSettingWithStandardValue("ServerLocation", "");
            AutoIdentifyPlayers = settings.GetAppSettingWithStandardValue("AutoIdentifyPlayers", false);
            PrivateSlots = settings.GetAppSettingWithStandardValue("PrivateSlots", 0);
            clog.Level = settings.GetAppSettingWithStandardValue("ConsoleLoggingLevel", Utilities.Logging.Verbosity.Detailed);
            flog.Level = settings.GetAppSettingWithStandardValue("FileLoggingLevel", Utilities.Logging.Verbosity.Minimal);

            if (settings.GetAppSettingWithStandardValue("ConsoleLoggingEnabled", true))
                Logger.Add(clog);

            if (settings.GetAppSettingWithStandardValue("FileLoggingEnabled", true))
                Logger.Add(flog);
        }

        /// <summary>
        /// The minimum level allowed to join the server
        /// </summary>
        public int MinLevel { get; private set; }

        /// <summary>
        /// The maximum level allowed to join the server
        /// </summary>
        public int MaxLevel { get; private set; }

        /// <summary>
        /// The maxmimum number of players allowed on this server
        /// </summary>
        public int PlayerLimit { get; private set; }

        /// <summary>
        /// The number of private slots on this server
        /// </summary>
        public int PrivateSlots { get; private set; }

        /// <summary>
        /// Indicates if the server should be started, see also ServerLocation
        /// </summary>
        public bool StartServer { get; private set; }

        /// <summary>
        /// The location of the server file
        /// </summary>
        public string ServerLocation { get; private set; }

        /// <summary>
        /// Indicates if identification of players should be started automatically
        /// </summary>
        public bool AutoIdentifyPlayers { get; private set; }

        /// <summary>
        /// The minimum HP required to join
        /// </summary>
        public float MinHP { get; private set; }

        /// <summary>
        /// The maximum HP allowed to join
        /// </summary>
        public float MaxHP { get; private set; }
    }
}
