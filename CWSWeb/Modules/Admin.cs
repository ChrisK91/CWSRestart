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
            CWSProtocol.Client c = new CWSProtocol.Client("AdminModule");

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
                Models.Admin.ControlPanel m = new Models.Admin.ControlPanel(null, false, true, 0);

                if (message.Key != null)
                {
                    Context.ViewBag["requiresDelay"] = true;
                    Session.Delete("requiresDelay");
                }
                else
                {
                    List<string> logEntries = c.GetLogMessages();
                    logEntries.Reverse();

                    Dictionary<string, object> watcherSettings = c.GetWatcherStatus();

                    if (watcherSettings != null)
                        m = new Models.Admin.ControlPanel(logEntries,
                            watcherSettings.ContainsKey("ENABLED") ? Boolean.Parse(watcherSettings["ENABLED"].ToString()) : false,
                            watcherSettings.ContainsKey("BLOCKED") ? Boolean.Parse(watcherSettings["BLOCKED"].ToString()) : false,
                            watcherSettings.ContainsKey("TIMEOUT") ? UInt32.Parse(watcherSettings["TIMEOUT"].ToString()) : 0);
                }

                return View["index.cshtml", m];
            };

            Get["/logout"] = parameters =>
            {
                return this.LogoutAndRedirect("~/");
            };

            Get["/toggle/{action}"] = parameters =>
            {
                string action = parameters["action"].ToString();

                switch (action.ToLower())
                {
                    case "start":
                        c.SendStart();
                        Session["toggleMessage"] = "The server will now be started. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "stop":
                        c.SendStop();
                        Session["toggleMessage"] = "The server will now be stopped. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "restart":
                        c.SendRestart();
                        Session["toggleMessage"] = "The server will now be restarted. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                    case "kill":
                        c.SendKill();
                        Session["toggleMessage"] = "The server will now be killed. Please refresh the page in a few seconds.";
                        Session["requiresDelay"] = true;
                        break;
                }
                Helper.CachedVariables.UpdateCachedVariables();
                return Response.AsRedirect("/admin");
            };

            Get["/statistics"] = parameters =>
            {
                return View["statistics.cshtml"];
            };

            Get["/log/clear"] = parameters =>
            {
                c.ClearLogMessage();
                return Response.AsRedirect("/admin");
            };

            Get["/watcher/{action}"] = parameters =>
            {
                string action = parameters["action"].ToString();
                switch (action.ToLower())
                {
                    case "start":
                        c.StartWatcher();
                        break;
                    case "stop":
                        c.StopWatcher();
                        break;
                }
                return Response.AsRedirect("/admin");
            };

            Post["/watcher"] = parameters =>
            {
                string timeout = (string)Request.Form.Timeout;
                UInt32 seconds;

                if (UInt32.TryParse(timeout, out seconds))
                    c.SetWatcherTimeout(seconds);

                return Response.AsRedirect("/admin");
            };
        }
    }
}
