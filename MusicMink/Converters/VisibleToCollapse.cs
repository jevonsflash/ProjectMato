using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MusicMink.Converters
{
    class VisibleToCollapse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility get = (Visibility)value;
            return GetConvertedVisibility(get);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility get = (Visibility)value;
            return GetConvertedVisibility(get);

        }
        private Visibility GetConvertedVisibility(Visibility get)
        {
            if (get == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

    }
}
