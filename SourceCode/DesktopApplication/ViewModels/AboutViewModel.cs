using System;
using System.Windows.Input;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class AboutViewModel : ObservableObject
    {
        public ICommand OpenWebsiteCommand { get; private set; }
        public AboutViewModel() {
            OpenWebsiteCommand = new RelayCommand<string>(OpenWebsite);
        }

        private void OpenWebsite(string Url)
        {
            try
            {
                System.Diagnostics.Process.Start(Url);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}