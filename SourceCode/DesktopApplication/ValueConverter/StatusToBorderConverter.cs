using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TvSeriesCalendar.ValueConverter
{
    public class StatusToBorderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(value => value == DependencyProperty.UnsetValue))
                return null;
            string status = (string) values[0];
            DateTime? nextSeasonReleaseDate = (DateTime?) values[1];
            if (nextSeasonReleaseDate != null)
                return new SolidColorBrush(Colors.Green);
            switch (status)
            {
                case "Returning Series":
                    return new SolidColorBrush(Colors.Orange);
                case "Ended":
                case "Canceled":
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Purple);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}