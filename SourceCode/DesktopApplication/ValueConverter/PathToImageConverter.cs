using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ValueConverter
{
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string path)) return null;
            try
            {
                BitmapImage image = new BitmapImage();
                using (FileStream stream = File.OpenRead(path))
                {
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad; // load the image from the stream
                    image.EndInit();
                } // close the stream

                return image;
            }
            catch (FileNotFoundException ex)
            {
                Logger.Exception(ex, "PathToImageConverter");
                return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}