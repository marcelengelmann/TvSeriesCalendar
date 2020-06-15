using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace TvSeriesCalendar.ValueConverter
{
    public class StatusToBorderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string Status = (string)values[0];
            DateTime? NextSeasonReleaseDate = (DateTime?)values[1];
            if (NextSeasonReleaseDate != null)
                return new SolidColorBrush(Colors.Green);
            else if (Status == "Returning Series")
                return new SolidColorBrush(Colors.Orange);
            else if (Status == "Ended" || Status == "Canceled")
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Purple);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
