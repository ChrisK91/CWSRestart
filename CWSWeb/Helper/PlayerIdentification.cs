using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    public static class PlayerIdentification
    {
        public static string IdentifyPlayer(string ip)
        {
            if (Helper.CachedVariables.KnownPlayers != null)
            {
                string name = Helper.CachedVariables.KnownPlayers.GetConnectedPlayerName(ip);

                if (name == null)
                {
                    IList<string> names = Helper.CachedVariables.KnownPlayers.GetKnownNames(ip);

                    if (names.Count > 0)
                    {
                        return String.Format("{0} - unidentified - known names: {1}", ip, String.Join(", ", names));
                    }
                }
                else
                {
                    return String.Format("{0} - {1}", ip, name);
                }
            }
            return ip;
        }

        public static void IdentifyPlayers(ref List<string> accessList)
        {
            for (int i = 0; i < accessList.Count(); i++)
                accessList[i] = IdentifyPlayer(accessList[i]);
        }
    }
}
