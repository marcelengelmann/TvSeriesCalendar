using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    public class PageNumberToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(value => value == DependencyProperty.UnsetValue)) return Visibility.Hidden;
            string source = (string) values[0];
            int currentPage = (int) values[1];
            int pagesNumber = (int) values[2];

            if (currentPage == 0 || pagesNumber == 0)
                return Visibility.Hidden;
            if (source == "Previous") return currentPage != 1 ? Visibility.Visible : Visibility.Hidden;
            if (source == "Next")
                return currentPage == pagesNumber ? Visibility.Hidden : Visibility.Visible;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}