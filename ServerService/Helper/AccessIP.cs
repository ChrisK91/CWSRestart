using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public class AccessIP : AccessListEntry
    {
        public IPAddress Address { get; private set; }

        public AccessIP(IPAddress address)
        {
            Address = address;
        }

        public override bool Equals(object obj)
        {
            if (obj is AccessIP)
            {
                return ((AccessIP)obj).Address.Equals(Address);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Address.ToString();
        }

        public override bool Matches(System.Net.IPAddress target)
        {
            return Address.Equals(target);
        }

        new public static bool TryParse(string source, out AccessListEntry target)
        {
            IPAddress address;

            if (IPAddress.TryParse(source, out address))
            {
                target = new AccessIP(address);
                return true;
            }
            target = null;
            return false;
        }
    }
}
