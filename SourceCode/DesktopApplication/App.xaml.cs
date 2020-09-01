using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            DispatcherUnhandledException += (s, e) =>
            {
                string exceptionSource = "Application.Current.DispatcherUnhandledException";
                Logger.Exception(e.Exception, exceptionSource);
                CrashReportHandler.ReportCrash(e.Exception, exceptionSource);
            };
        }
    }
}