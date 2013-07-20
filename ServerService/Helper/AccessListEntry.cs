using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public abstract class AccessListEntry
    {
        public abstract override string ToString();

        public static bool TryParse(string source, out AccessListEntry entry)
        {
            throw new NotImplementedException();
        }

        public abstract bool Matches(IPAddress target);
    }
}
