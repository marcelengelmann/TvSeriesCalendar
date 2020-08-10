using System;
using System.Diagnostics;
using System.Windows.Input;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class AboutViewModel : ObservableObject
    {
        public AboutViewModel()
        {
            OpenWebsiteCommand = new RelayCommand<string>(OpenWebsite);
        }

        public ICommand OpenWebsiteCommand { get; }

        private static void OpenWebsite(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}