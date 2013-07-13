using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CWSRestart.Helper
{
    class AllBoolsTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
    System.Globalization.CultureInfo culture)
        {
            foreach (bool val in values)
            {
                if (!val)
                    return false;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
