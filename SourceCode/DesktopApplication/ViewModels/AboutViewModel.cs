using System;
using System.Diagnostics;
using System.Reflection;
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

        public string VersionText
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        private static void OpenWebsite(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "AboutViewModel.OpenWebsite");
            }
        }
    }
}