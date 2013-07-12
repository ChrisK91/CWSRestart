using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CWSRestart.Helper
{
    class BooleanToInverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InverseObject(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InverseObject(value);
        }

        private static object InverseObject(object value)
        {
            bool output;

            if (Boolean.TryParse(value.ToString(), out output))
                return !output;
            else
                return false;
        }
    }
}
