using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Security;
using Nancy.Authentication.Forms;
using System.Net;
using System.IO;

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
                Models.Admin.ControlPanel m = new Models.Admin.ControlPanel(null, false, true, 0, false, false, false);

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
                            watcherSettings.ContainsKey("TIMEOUT") ? UInt32.Parse(watcherSettings["TIMEOUT"].ToString()) : 0,
                            watcherSettings.ContainsKey("CHECKINTERNET") ? Boolean.Parse(watcherSettings["CHECKINTERNET"].ToString()) : false,
                            watcherSettings.ContainsKey("CHECKLAN") ? Boolean.Parse(watcherSettings["CHECKLAN"].ToString()) : false,
                            watcherSettings.ContainsKey("CHECKLOOPBACK") ? Boolean.Parse(watcherSettings["CHECKLOOPBACK"].ToString()) : false);
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
                int seconds;

                string checkInternet = (string)Request.Form.CheckInternet;
                string checkLan = (string)Request.Form.CheckLAN;
                string checkLoopback = (string)Request.Form.CheckLoopback;

                if (Int32.TryParse(timeout, out seconds))
                    c.SetWatcherTimeout(seconds);

                c.SetWatcherCheckAccess(
                    checkInternet == "on" ? true : false,
                    checkLan == "on" ? true : false,
                    checkLoopback == "on" ? true : false
                    );

                Session["toggleMessage"] = "The configuration has been updated.";
                Session["requiresDelay"] = true;
                return Response.AsRedirect("/admin");
            };

            Get["/access"] = parameters =>
            {
                var message = Session.FirstOrDefault(o => o.Key == "kickMessage");

                if (message.Key != null)
                {
                    Context.ViewBag["kickMessage"] = message.Value;
                    Session.Delete("kickMessage");
                }

                List<string> connected = c.GetConnectedPlayers();
                List<string> accessList = c.GetAccessListEntries();

                /*if (Helper.CachedVariables.PlayeridentificationEnabled)
                    Helper.PlayerIdentification.IdentifyPlayers(ref accessList);*/

                ServerService.AccessControl.AccessMode mode = c.GetAccessMode();

                Models.Admin.Access m = new Models.Admin.Access(connected, accessList, mode);
                return View["access", m];
            };

            Post["/access"] = parameters =>
                {
                    ServerService.AccessControl.AccessMode mode = ServerService.AccessControl.AccessMode.Blacklist;
                    string modeRaw = (string)Request.Form.Mode;

                    if (modeRaw != null)
                        mode = (ServerService.AccessControl.AccessMode)Enum.Parse(typeof(ServerService.AccessControl.AccessMode), modeRaw);

                    List<string> accessList = null;

                    string rawAccess = (string)Request.Form.List;
                    if (rawAccess != null)
                    {
                        accessList = new List<string>();

                        using (StringReader sr = new StringReader(rawAccess))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                                accessList.Add(line);
                        }
                    }

                    if (rawAccess != null)
                        c.SetAccess(accessList, mode);

                    return Response.AsRedirect("/admin/access");
                };

            Get["/access/kick/{ip}"] = parameters =>
            {
                string raw = parameters["ip"].ToString();
                IPAddress ip;

                if (IPAddress.TryParse(raw, out ip))
                {
                    c.KickPlayer(ip.ToString());
                    Session["kickMessage"] = String.Format("The player {0} should now be kicked", ip.ToString());
                }

                return Response.AsRedirect("/admin/access");
            };
        }
    }
}
