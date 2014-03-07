using System;
using System.Windows;
using System.Windows.Data;

namespace POSH.Socrata.WP8.ConverterClasses
{
    public class IsIndeterminateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var isIndeterminate = value as Nullable<bool>;
            if (isIndeterminate.Value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}