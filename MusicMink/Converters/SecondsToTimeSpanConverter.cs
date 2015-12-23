using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MusicMink.Converters
{
    class SecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int d = (int)((double)value);
            return TimeSpan.FromSeconds(d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            TimeSpan ts = (TimeSpan)value;
            return ts.TotalSeconds;
        }
    }
}
