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

                    if (rawData.ContainsKey("RUNTIME"))
                        stats.FormatedRuntime = rawData["RUNTIME"].ToString();

                    if (rawData.ContainsKey("LOGFILE"))
                        stats.UpdateFromCSV(rawData["LOGFILE"].ToString());

                }

                statsLastUpdated = DateTime.Now;
                refreshJSON();
            }
        }

        private static DateTime statsLastUpdated;
        private static Statistics stats;
        public static Statistics Stats
        {
            get
            {
                return stats;
            }
        }


        private static string statsJSON;
        public static string StatsJSON
        {
            get
            {
                return statsJSON;
            }
        }

        private static string keysJSON;
        public static string KeysJSON
        {
            get
            {
                return keysJSON;
            }
        }

        private static string activeplayersJSON;
        public static string ActiveplayersJSON
        {
            get
            {
                return activeplayersJSON;
            }
        }

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
            statsJSON = js.Serialize(Stats);
            keysJSON = js.Serialize(Stats.Keys);
            activeplayersJSON = js.Serialize(Stats.ActivePlayers);
        }

        public class Statistics
        {
            public bool IsAlive;
            public Players PlayerStats = new Players();
            public string FormatedRuntime = "";

            public DateTime[] Keys;
            public int[] ActivePlayers;


            public class Players
            {
                public int Total;
                public int Current;
            }

            public void UpdateFromCSV(string path)
            {
                if (File.Exists(path))
                {
                    const int lineLimit = 151;
                    string data = ReadEndTokens(path, lineLimit, Encoding.UTF8, Environment.NewLine);

                    string[] lines = data.Split(new string[]{ Environment.NewLine }, StringSplitOptions.None);

                    if (1 < lines.Length && lines.Length < lineLimit)
                    {
                        Keys = new DateTime[lines.Length - 2];
                        ActivePlayers = new int[lines.Length - 2];
                    }
                    else
                    {
                        Keys = new DateTime[lines.Length - 1];
                        ActivePlayers = new int[lines.Length - 1];
                    }

                    int index = 0;

                    foreach (string line in lines)
                    {
                        string[] value = line.Split(new string[]{";"}, StringSplitOptions.None);
                        if (value.Length == 7 && value[0] != "Timestamp")
                        {
                            DateTime dt = DateTime.ParseExact(value[0], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            int current = Int32.Parse(value[2]);

                            if (dt.Year > 1)
                            {
                                Keys[index] = dt;
                                ActivePlayers[index] = current;

                                index++;
                            }
                        }
                    }
                }
            }

            public static string ReadEndTokens(string path, Int64 numberOfTokens, Encoding encoding, string tokenSeparator)
            {

                int sizeOfChar = encoding.GetByteCount("\n");
                byte[] buffer = encoding.GetBytes(tokenSeparator);


                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Int64 tokenCount = 0;
                    Int64 endPosition = fs.Length / sizeOfChar;

                    for (Int64 position = sizeOfChar; position < endPosition; position += sizeOfChar)
                    {
                        fs.Seek(-position, SeekOrigin.End);
                        fs.Read(buffer, 0, buffer.Length);

                        if (encoding.GetString(buffer) == tokenSeparator)
                        {
                            tokenCount++;
                            if (tokenCount == numberOfTokens)
                            {
                                byte[] returnBuffer = new byte[fs.Length - fs.Position];
                                fs.Read(returnBuffer, 0, returnBuffer.Length);
                                return encoding.GetString(returnBuffer);
                            }
                        }
                    }

                    // handle case where number of tokens in file is less than numberOfTokens
                    fs.Seek(0, SeekOrigin.Begin);
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    return encoding.GetString(buffer);
                }
            }
        }
    }
}
