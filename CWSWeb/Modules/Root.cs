﻿using CWSWeb.Helper;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Authentication.Forms;

namespace CWSWeb.Modules
{
    public class Root : NancyModule
    {
        public Root()
            : base()
        {
            Get["/"] = parameters =>
            {
                if (CachedVariables.Stats.Enabled)
                    return View["index", CachedVariables.Stats];
                else
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
            };

            Get["/login"] = parameters =>
            {
                return View["login"];
            };

            Post["/login"] = parameters =>
            {
                var userGuid = Helper.Users.Authentication.ValidateUser((string)Request.Form.Username, (string)Request.Form.Password);

                if (userGuid == null)
                    return Response.AsRedirect("/login");

                return this.LoginAndRedirect(userGuid.Value, null, "/admin");
            };

            Get["/logout"] = parameters =>
            {
                return this.LogoutAndRedirect("~/");
            };

            Get["/notavailable"] = parameters =>
            {
                return View["notavailable"];
            };
        }
    }
}
