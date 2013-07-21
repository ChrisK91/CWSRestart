using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace CWSWeb.Modules
{
    public class Admin : NancyModule
    {
        public Admin()
            : base("/admin")
        {
            this.RequiresAuthentication();

            Get["/"] = parameters =>
            {
                return View["index.cshtml"];
            };

            Get["/logout"] = parameters =>
            {
                return this.LogoutAndRedirect("~/");
            };
        }
    }
}
