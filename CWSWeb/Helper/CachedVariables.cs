using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{

    public static class CachedVariables
    {
        internal static void UpdateCachedVariables()
        {
            if (refreshRequired())
            {
                Stats = new Statistics();

                if (Helper.Settings.Instance.Client != null)
                {
                    Dictionary<string, object> rawData = Helper.Settings.Instance.Client.GetStatistics();

                    if (rawData.ContainsKey("ALIVE"))
                        Stats.IsAlive = Boolean.Parse(rawData["ALIVE"].ToString());

                    if (rawData.ContainsKey("CURRENT"))
                        Stats.PlayerStats.Current = Int32.Parse(rawData["CURRENT"].ToString());

                    if (rawData.ContainsKey("TOTAL"))
                        Stats.PlayerStats.Total = Int32.Parse(rawData["TOTAL"].ToString());

                    if (rawData.ContainsKey("RUNTIME"))
                        Stats.FormatedRuntime = rawData["RUNTIME"].ToString();

                    if (rawData.ContainsKey("STATISTICSFILE"))
                        Stats.RefreshStatisticsFromDB(rawData["STATISTICSFILE"].ToString());

                    if (rawData.ContainsKey("ENABLED"))
                        Stats.Enabled = Boolean.Parse(rawData["ENABLED"].ToString());

                    PlayeridentificationEnabled = Helper.Settings.Instance.Client.GetPlayerIdentification();

                    PremiumslotsEnabled = Helper.Settings.Instance.Client.GetPremiumslots();

                    if (PlayeridentificationEnabled)
                        KnownPlayersLocation = Helper.Settings.Instance.Client.GetPlayersDatabase();

                    if (PremiumslotsEnabled)
                        PremiumplayersLocation = Helper.Settings.Instance.Client.GetPremiumDatabase();
                }

                statsLastUpdated = DateTime.Now;
                refreshJSON();
            }
        }

        private static DateTime statsLastUpdated;
        private static long[] memoryUsage;
        private static string knownPlayersLocation;
        private static string premiumPlayersLocation;

        public static bool PlayeridentificationEnabled { get; private set; }
        public static bool PremiumslotsEnabled { get; private set; }

        public static ServerService.Database.KnownPlayers KnownPlayers { get; private set; }
        public static ServerService.Database.PremiumPlayers PremiumPlayers { get; private set; }

        public static string KnownPlayersLocation
        {
            get
            {
                return knownPlayersLocation;
            }
            set
            {
                if (knownPlayersLocation != value)
                {
                    if (File.Exists(value))
                    {
                        knownPlayersLocation = value;
                        KnownPlayers = new ServerService.Database.KnownPlayers(value);
                    }
                }
            }
        }

        public static string PremiumplayersLocation
        {
            get
            {
                return premiumPlayersLocation;
            }
            set
            {
                if (premiumPlayersLocation != value)
                {
                    if (File.Exists(value))
                    {
                        premiumPlayersLocation = value;
                        PremiumPlayers = new ServerService.Database.PremiumPlayers(value);
                    }
                }
            }
        }

        public static Statistics Stats { get; private set; }
        public static string RestartsJSON { get; private set; }
        public static string StatsJSON { get; private set; }
        public static string KeysJSON { get; private set; }
        public static string ActiveplayersJSON { get; private set; }
        public static string MemoryUsageJSON { get; private set; }

        private static bool refreshRequired()
        {
            return statsLastUpdated == null || ((DateTime.Now - statsLastUpdated) >= new TimeSpan(0, 0, 5));
        }

        private static void performUpdate()
        {
            if (refreshRequired())
            {
                refreshJSON();
            }
        }

        private static void refreshJSON()
        {
            Nancy.Json.JavaScriptSerializer js = new Nancy.Json.JavaScriptSerializer();
            StatsJSON = js.Serialize(Stats);
            KeysJSON = js.Serialize(Stats.Keys);
            ActiveplayersJSON = js.Serialize(Stats.ActivePlayers);
            MemoryUsageJSON = js.Serialize(memoryUsage);
            RestartsJSON = js.Serialize(Stats.Restarts);
        }

        public class Statistics
        {
            public bool IsAlive;
            public Players PlayerStats = new Players();
            public string FormatedRuntime = "";
            public bool Enabled = false;

            public DateTime[] Keys;
            public int[] ActivePlayers;
            public DateTime[] Restarts;


            public class Players
            {
                public int Total;
                public int Current;
            }

            public async void RefreshStatisticsFromDB(string path)
            {
                if (File.Exists(path))
                {
                    ServerService.Database.Statistics stats = new ServerService.Database.Statistics(path);
                    List<ServerService.Database.StatisticsEntry> entries = await stats.GetStatisticEntriesAsync(Settings.Instance.LinesToRead);

                    bool dropRestart = entries.Count > Settings.Instance.LinesToRead;

                    Keys = new DateTime[entries.Count];
                    ActivePlayers = new int[entries.Count];
                    memoryUsage = new long[entries.Count];

                    List<DateTime> tmpRestarts = new List<DateTime>();

                    int prevRestarts = 0;

                    for (int i = 0; i < entries.Count; i++)
                    {
                        Keys[i] = entries[i].TimeStamp;
                        ActivePlayers[i] = entries[i].CurrentPlayers;
                        memoryUsage[i] = entries[i].CurrentMemory;

                        if (entries[i].Restarts > prevRestarts)
                        {
                            if (dropRestart)
                            {
                                dropRestart = false;
                            }
                            else
                            {
                                tmpRestarts.Add(entries[i].TimeStamp);
                            }
                            prevRestarts = entries[i].Restarts;
                        }
                    }

                    Restarts = tmpRestarts.ToArray();
                }
            }
        }
    }
}
