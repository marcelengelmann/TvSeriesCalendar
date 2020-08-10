using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    internal class SearchBoxWatermarkVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEmpty = (bool) values[0];
            bool isFocused = (bool) values[1];
            if (isEmpty && !isFocused) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}