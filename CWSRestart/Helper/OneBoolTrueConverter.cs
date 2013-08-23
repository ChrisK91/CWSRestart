using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CWSRestart.Helper
{
    class OneBoolTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
    System.Globalization.CultureInfo culture)
        {
            foreach (object val in values)
            {
                if (val is bool && (bool)val == true)
                    return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
