using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    /// <summary>
    /// Contains extension methods
    /// </summary>
    public static sealed class Extensions
    {
        public static bool Contains(this Collection<PlayerInfo> list, IPAddress IP)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (IP == null)
                throw new ArgumentNullException("IP");

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Address.Equals(IP))
                    return true;
            }

            return false;
        }

        public static bool Contains(this ObservableCollection<PlayerInfo> list, IPAddress IP)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Address.Equals(IP))
                    return true;
            }

            return false;
        }

        public static void Add(this ObservableCollection<PlayerInfo> list, IPAddress address)
        {
            list.Add(new PlayerInfo(address));
        }
    }
}
