using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    public class PageNumberToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string button;
            int currentPage;
            int pagesNumber;
            try
            {
                button = (string) values[0];
                currentPage = (int) values[1];
                pagesNumber = (int) values[2];
            }
            catch (InvalidCastException)
            {
                return Visibility.Hidden;
            }

            if (currentPage == 0 || pagesNumber == 0)
                return Visibility.Hidden;
            if (button == "Previous")
            {
                if (currentPage != 1)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }

            if (currentPage == pagesNumber)
                return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}