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
            IPAddress ip = (IPAddress)value;

            if (ServerService.Helper.Settings.Instance.KnownPlayers == null)
                return ip.ToString();
            else
            {
                string s = ServerService.Helper.Settings.Instance.KnownPlayers.GetConnectedPlayerName(ip.ToString());

                if (s == null)
                    return ip.ToString();
                else
                {
                    return String.Format("{0} - {1}", ip.ToString(), s);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
