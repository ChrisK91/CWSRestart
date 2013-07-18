using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Pages
{
    public class API : NancyModule
    {
        public API() : base("/API")
        {
            Get["/stats"] = parameters =>
            {
                return View["stats", CachedVariables.StatsJSON];
            };
        }
    }

    public class Statistics
    {
        public bool IsAlive;
        public Players PlayerStats = new Players();


        public class Players
        {
            public int Total;
            public int Current;
        }
    }

    public static class CachedVariables
    {
        private static DateTime statsLastUpdated;
        private static Statistics stats;
        public static Statistics Stats
        {
            get
            {
                if (statsLastUpdated == null || DateTime.Now.Subtract(statsLastUpdated) >= new TimeSpan(0, 0, 5))
                {
                    stats = new Statistics();

                    if (Helper.Settings.Client != null)
                    {
                        Dictionary<string, object> rawData = Helper.Settings.Client.GetStatistics();

                        if (rawData.ContainsKey("ALIVE"))
                            stats.IsAlive = Boolean.Parse(rawData["ALIVE"].ToString());

                        if (rawData.ContainsKey("CURRENT"))
                            stats.PlayerStats.Current = Int32.Parse(rawData["CURRENT"].ToString());

                        if (rawData.ContainsKey("TOTAL"))
                            stats.PlayerStats.Total = Int32.Parse(rawData["TOTAL"].ToString());
                    }

                    statsLastUpdated = DateTime.Now;
                }

                return stats;
            }
        }


        private static string statsJSON;
        public static string StatsJSON
        {
            get
            {
                if (statsLastUpdated == null || DateTime.Now.Subtract(statsLastUpdated) >= new TimeSpan(0, 0, 5))
                {
                    Nancy.Json.JavaScriptSerializer js = new Nancy.Json.JavaScriptSerializer();
                    statsJSON = js.Serialize(Stats);
                }

                return statsJSON;
            }
        }
    }
}
