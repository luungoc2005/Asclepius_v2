using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Asclepius.Converters
{
    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int secs = System.Convert.ToInt32(value);

            CultureInfo ci = new CultureInfo("en-GB");

            string format = @"hh\:mm\:ss";

            TimeSpan time = TimeSpan.FromSeconds(secs);
            return time.ToString(format, ci);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
