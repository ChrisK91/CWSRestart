using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    class Settings
    {
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                    instance = new Settings();

                return instance;
            }
        }

        public volatile CWSProtocol.Client Client;
        public int LinesToRead;
        private Utilities.Settings.Settings settings;

        private Settings()
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "CWSWeb.exe.config");
            settings = new Utilities.Settings.Settings(file);

            LinesToRead = settings.GetAppSettingWithStandardValue("LinesToRead", 201);
        }
    }
}
