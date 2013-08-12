using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CWSRestart.Helper
{
    class IPAddressToNamedIPStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!Settings.Instance.PlayeridentificationEnabled || ServerService.Helper.Settings.Instance.KnownPlayers == null)
                return value.ToString();

            if (value is IPAddress)
            {
                IPAddress ip = value as IPAddress;
                string s = ServerService.Helper.Settings.Instance.KnownPlayers.GetConnectedPlayerName(ip.ToString());

                if (s == null)
                {
                    List<string> names = ServerService.Helper.Settings.Instance.KnownPlayers.GetKnownNames(ip.ToString());

                    if(names.Count == 0)
                        return ip.ToString();

                    return String.Format("{0} - {1}", ip.ToString(), string.Join(", ", names.ToArray()));
                }
                else
                {
                    return String.Format("{0} - {1} (currently online)", ip.ToString(), s);
                }
            }
            else if (value is ServerService.Helper.AccessIP)
            {
                ServerService.Helper.AccessIP aip = value as ServerService.Helper.AccessIP;
                IPAddress ip = aip.Address;

                string s = ServerService.Helper.Settings.Instance.KnownPlayers.GetConnectedPlayerName(ip.ToString());

                if (s == null)
                {
                    List<string> names = ServerService.Helper.Settings.Instance.KnownPlayers.GetKnownNames(ip.ToString());

                    if (names.Count == 0)
                        return ip.ToString();

                    return String.Format("{0} - {1}", ip.ToString(), string.Join(", ", names.ToArray()));
                }
                else
                {
                    return String.Format("{0} - {1} (currently online)", ip.ToString(), s);
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
