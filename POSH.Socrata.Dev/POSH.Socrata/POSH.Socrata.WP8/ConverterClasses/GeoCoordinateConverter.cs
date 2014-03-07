using System;
using System.Device.Location;
using System.Windows.Data;

namespace POSH.Socrata.WP8.ConverterClasses
{
    public class GeoCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as POSH.Socrata.Entity.Models.Altitude;
            if (val != null)
            {
                return new GeoCoordinate(val.Latitude, val.Longitude);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}