using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MusicMink.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        private bool _invert = false;
        public bool Invert
        {
            get
            {
                return _invert;
            }
            set
            {
                _invert = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((value != null && (bool)value) ^ Invert)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((value != null && value is Visibility))
            {
                switch ((Visibility)value)
                {
                    case Visibility.Visible: return (true ^ Invert);
                    case Visibility.Collapsed: return (false ^ Invert);
                    default:
                        return false;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
