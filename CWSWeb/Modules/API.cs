using CWSWeb.Helper;
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
                    return View["stats", CachedVariables.StatsJSON].WithHeader("Cache-Control", "no-cache"); ;
                };

            Get["/stats/playeractivity"] = parameters =>
                {
                    return View["statsData", new DataContainer(CachedVariables.KeysJSON, CachedVariables.ActiveplayersJSON)].WithHeader("Cache-Control", "no-cache");
                };
        }
    }
}
