using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TvSeriesCalendar.ValueConverter
{
    internal class ActiveSearchFunctionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return null;
            return (string) value == (string) parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}