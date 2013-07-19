using CWSWeb.Helper;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Modules
{
    public class Root : NancyModule
    {
        public Root() : base()
        {
            Get["/"] = parameters =>
            {
                return View["index", CachedVariables.Stats];
            };
        }
    }
}
