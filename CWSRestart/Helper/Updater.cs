using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CWSRestart.Helper
{
    class Updater
    {
        /// <summary>
        /// Build date in Year, Month, Day
        /// </summary>
        public static DateTime BuildDate = new DateTime(2013, 7, 26);
        public static string VersionLocation = "http://chrisk91.github.io/CWSRestart/last-update";
        
        public static async Task<bool> UpdateAvailable()
        {
            WebRequest request = WebRequest.Create(VersionLocation);
            WebResponse response = await request.GetResponseAsync();

            string answer = "";

            using(Stream responseStream = response.GetResponseStream())
                using(StreamReader sr = new StreamReader(responseStream))
                    answer = await sr.ReadToEndAsync();

            answer = answer.Replace("\n", "");
            answer = answer.Replace("\r", "");
            answer = answer.Trim();


            try
            {
                DateTime lastUpdate = DateTime.ParseExact(answer, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (lastUpdate.CompareTo(BuildDate) > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
