using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Security;
using Nancy.Authentication;
using CWSWeb.Helper;

namespace CWSWeb.Modules
{
    public class Premium : NancyModule
    {
        public Premium()
            : base("/premium")
        {
            IReadOnlyList<string> requiredClaims = new List<string> { Helper.Users.Authentication.PREMIUM };

            this.RequiresAuthentication();
            this.RequiresClaims(requiredClaims);

            Get["/"] = parameters =>
            {
                if (CachedVariables.PremiumslotsEnabled)
                {
                    if (Helper.CachedVariables.PremiumPlayers != null)
                        Helper.CachedVariables.PremiumPlayers.AddPremiumPlayerAsync(Request.UserHostAddress, (Context.CurrentUser as Helper.Users.User).UserId);
                    return View["index"];
                }
                else
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
            };
        }
    }
}
