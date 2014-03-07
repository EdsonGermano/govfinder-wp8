using System;
using System.Windows.Data;

namespace POSH.Socrata.WP8.ConverterClasses
{
    public class CommentPublishTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var publishedAt = value as Nullable<DateTime>;
            TimeSpan dateDiff = (DateTime.Now - publishedAt.Value.ToLocalTime());
            if (dateDiff.Days != 0)
            {
                return "commented on " + publishedAt.Value.ToString("MMMM dd,yyyy");
            }
            else if (dateDiff.Hours != 0)
            {
                return "commented on " + dateDiff.Hours.ToString() + " hours ago";
            }
            else if (dateDiff.Minutes != 0)
            {
                return "commented on " + dateDiff.Minutes.ToString() + " minutes ago";
            }
            else if (dateDiff.Seconds < 60)
            {
                return "commented less than a minute ago";
            }
            else if (dateDiff.Milliseconds != 0)
            {
                return "commented less than a minute ago";
            }
            else
            {
                return "commented on " + publishedAt.Value.ToString("MMMM dd,yyyy");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}