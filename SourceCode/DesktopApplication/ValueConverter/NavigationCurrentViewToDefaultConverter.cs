using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using TvSeriesCalendar.ViewModels;

namespace TvSeriesCalendar.ValueConverter
{
    internal class NavigationCurrentViewToDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return false;
                case SeriesViewModel _ when (string)parameter == "Series":
                case SearchViewModel _ when (string)parameter == "Search":
                case SettingsViewModel _ when (string)parameter == "Settings":
                case AboutViewModel _ when (string)parameter == "About":
                    return true;
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
