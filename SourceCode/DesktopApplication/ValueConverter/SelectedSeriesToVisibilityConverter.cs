using System;
using System.Globalization;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    public class SelectedSeriesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) return "Visible";
            return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}