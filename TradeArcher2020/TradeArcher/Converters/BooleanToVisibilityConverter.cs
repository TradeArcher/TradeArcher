using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TradeArcher.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        private object GetVisibility(object value)
        {

            if (value is int intValue)
            {
                return Inverse ^ (intValue != 0) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (!(value is bool))
            {
                return Visibility.Collapsed;
            }

            return Inverse ^ ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                switch (visibility)
                {
                    case Visibility.Visible:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
