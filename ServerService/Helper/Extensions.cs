using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public static class Extensions
    {
        public static bool Contains(this List<PlayerInfo> list, IPAddress ip)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Address.Equals(ip))
                    return true;
            }

            return false;
        }

        public static bool Contains(this ObservableCollection<PlayerInfo> list, IPAddress ip)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Address.Equals(ip))
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
