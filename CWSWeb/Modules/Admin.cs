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
                var message = Session.FirstOrDefault(o => o.Key == "toggleMessage");
                
                if (message.Key != null)
                {
                    Context.ViewBag["toggleMessage"] = message.Value;
                    Session.Delete("toggleMessage");
                }

                message = Session.FirstOrDefault(o => o.Key == "requiresDelay");

                if (message.Key != null)
                {
                    Context.ViewBag["requiresDelay"] = true;
                    Session.Delete("requiresDelay");
                }

                return View["index.cshtml"];
            };

            Get["/logout"] = parameters =>
            {
                return this.LogoutAndRedirect("~/");
            };

            Get["/toggle/{action}"] = parameters =>
            {
                string action = parameters["action"].ToString();

                switch(action.ToLower())
                {
                    case "start":
                        Helper.Settings.Instance.Client.SendStart();
                        Session["toggleMessage"] = "The server will now be started. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "stop":
                        Helper.Settings.Instance.Client.SendStop();
                        Session["toggleMessage"] = "The server will now be stopped. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "restart":
                        Helper.Settings.Instance.Client.SendRestart();
                        Session["toggleMessage"] = "The server will now be restarted. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "kill":
                        Helper.Settings.Instance.Client.SendKill();
                        Session["toggleMessage"] = "The server will now be killed. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                }
                Helper.CachedVariables.UpdateCachedVariables();
                return Response.AsRedirect("/admin");
            };
        }
    }
}
