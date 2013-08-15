using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CWSWeb.Helper
{
    internal class CacheUpdater : IDisposable
    {
        Timer updater;

        public CacheUpdater()
        {
            updater = new Timer(5000);
            updater.AutoReset = true;
            updater.Elapsed += updater_Elapsed;
            updater.Start();
        }

        void updater_Elapsed(object sender, ElapsedEventArgs e)
        {
            updater.Enabled = false;

            CachedVariables.UpdateCachedVariables();
            UserbarCreator.GenerateUserbar(CachedVariables.Stats);

            updater.Enabled = true;
        }

        public void StopUpdater()
        {
            updater.Stop();
        }

        public void StartUpdater()
        {
            updater.Start();
        }

        public void Dispose()
        {
            if (updater != null)
                updater.Dispose();
        }
    }
}
