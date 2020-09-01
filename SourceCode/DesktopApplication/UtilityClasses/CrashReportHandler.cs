using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using TvSeriesCalendar.Services;

namespace TvSeriesCalendar.UtilityClasses
{
    internal static class CrashReportHandler
    {
        internal static void ReportCrash(Exception exception, string source)
        {
            MessageBoxResult answer =
                MessageBox.Show(
                    "The Application encountered an error and crashed.\n Would you like to send a crash Report?",
                    "Crash Report", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No,
                    MessageBoxOptions.DefaultDesktopOnly);
            if (answer != MessageBoxResult.Yes)
                return;
            string errorMessage = GetErrorTextFile();
            if (errorMessage == "")
                errorMessage = Logger.CreateExceptionOutput(exception, source);
            Task.Run(async () => await FormspreeReport.Send(errorMessage, FormspreeReport.ReportType.ErrorReport));
        }

        private static string GetErrorTextFile()
        {
            return File.Exists("error.txt") ? File.ReadAllText("error.txt") : "";
        }
    }
}