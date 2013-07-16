using Nancy;
using System;
using System.Collections.Generic;
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
                return View["stats"];
            };
        }
    }
}
