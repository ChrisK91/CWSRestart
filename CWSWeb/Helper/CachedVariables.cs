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

                    if (rawData.ContainsKey("LOGFILE"))
                        Stats.UpdateFromCSV(rawData["LOGFILE"].ToString());

                    if (rawData.ContainsKey("ENABLED"))
                        Stats.Enabled = Boolean.Parse(rawData["ENABLED"].ToString());

                }

                statsLastUpdated = DateTime.Now;
                refreshJSON();
            }
        }

        private static DateTime statsLastUpdated;
        private static int[] memoryUsage;

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

            public void UpdateFromCSV(string path)
            {
                if (File.Exists(path))
                {
                    string data = ReadEndTokens(path, Settings.Instance.LinesToRead, Encoding.UTF8, Environment.NewLine);

                    string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    bool dropRestart;

                    if (1 < lines.Length && lines.Length < Settings.Instance.LinesToRead)
                    {
                        Keys = new DateTime[lines.Length - 2];
                        ActivePlayers = new int[lines.Length - 2];
                        memoryUsage = new int[lines.Length - 2];
                        dropRestart = false;
                    }
                    else
                    {
                        Keys = new DateTime[lines.Length - 1];
                        ActivePlayers = new int[lines.Length - 1];
                        memoryUsage = new int[lines.Length - 1];
                        dropRestart = true;
                    }

                    List<DateTime> tmpRestarts = new List<DateTime>();

                    int index = 0;
                    int prevRestarts = 0;

                    foreach (string line in lines)
                    {
                        string[] value = line.Split(new string[] { ";" }, StringSplitOptions.None);
                        if (value.Length == 7 && value[0] != "Timestamp")
                        {
                            DateTime dt = DateTime.ParseExact(value[0], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            int current = Int32.Parse(value[2]);
                            int memory = Int32.Parse(value[4]);
                            int currentRestarts = Int32.Parse(value[6]);


                            if (dt.Year > 1)
                            {
                                Keys[index] = dt;
                                ActivePlayers[index] = current;
                                memoryUsage[index] = memory;

                                if (currentRestarts > prevRestarts)
                                {
                                    if (!dropRestart)
                                    {
                                        tmpRestarts.Add(dt);
                                    }
                                    else
                                    {
                                        dropRestart = false;
                                    }
                                    prevRestarts = currentRestarts;
                                }

                                index++;
                            }
                        }
                    }

                    Restarts = tmpRestarts.ToArray();
                }
            }

            public static string ReadEndTokens(string path, Int64 numberOfTokens, Encoding encoding, string tokenSeparator)
            {

                try
                {
                    int sizeOfChar = encoding.GetByteCount("\n");
                    byte[] buffer = encoding.GetBytes(tokenSeparator);


                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
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
                catch (IOException)
                {
                    //File in use
                    return "";
                }
            }
        }
    }
}
