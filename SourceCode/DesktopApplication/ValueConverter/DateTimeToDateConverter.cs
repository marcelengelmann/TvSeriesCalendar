using System;
using System.Globalization;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime = value as DateTime?;
            if (dateTime != null) return dateTime.Value.ToString("dd.MM.yyyy");
            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}