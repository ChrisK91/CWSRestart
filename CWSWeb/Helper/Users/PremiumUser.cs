using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper.Users
{
    class PremiumUser : IUserIdentity
    {
        public string UserName { get; set; }

        private IReadOnlyList<string> claims = new List<string> { "premium" };

        public IEnumerable<string> Claims
        {
            get
            {
                return claims;
            }
        }
    }
}
