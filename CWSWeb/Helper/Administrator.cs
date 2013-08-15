using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    internal class Administrator : IUserIdentity 
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}
