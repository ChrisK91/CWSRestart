using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public class AccessIPRange : AccessListEntry
    {
        public IPAddress StartAddress { get; private set; }
        public IPAddress EndAddress { get; private set; }

        public override string FriendlyName
        {
            get
            {
                return String.Format("{0} - {1}", StartAddress, EndAddress);
            }
        }

        private AccessIPRange(IPAddress start, IPAddress end)
        {
            StartAddress = start;
            EndAddress = end;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", StartAddress, EndAddress);
        }

        public override bool Equals(object obj)
        {
            if (obj is AccessIPRange)
            {
                AccessIPRange tmp = obj as AccessIPRange;
                return (StartAddress.Equals(tmp.StartAddress)) && (EndAddress.Equals(tmp.EndAddress));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Matches(System.Net.IPAddress target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            byte[] addressBytes = target.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < StartAddress.GetAddressBytes().Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < StartAddress.GetAddressBytes()[i]) ||
                    (upperBoundary && addressBytes[i] > EndAddress.GetAddressBytes()[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == StartAddress.GetAddressBytes()[i]);
                upperBoundary &= (addressBytes[i] == EndAddress.GetAddressBytes()[i]);
            }

            return true;
        }

        new public static bool TryParse(string source, out AccessListEntry target)
        {
            if (String.IsNullOrEmpty(source))
                throw new ArgumentException("source can neither be null nor empty", "source");

            string[] parts = source.Split('-');
            if (parts.Length == 2)
            {
                IPAddress start;
                IPAddress end;

                if (IPAddress.TryParse(parts[0].Trim(), out start) && IPAddress.TryParse(parts[1].Trim(), out end))
                {
                    for (int i = 0; i < start.GetAddressBytes().Length; i++)
                    {
                        if (start.GetAddressBytes()[i] > end.GetAddressBytes()[i])
                        {
                            target = null;
                            return false;
                        }
                    }

                    target = new AccessIPRange(start, end);
                    return true;
                }
            }
            target = null;
            return false;
        }
    }
}
