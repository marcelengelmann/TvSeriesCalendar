using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TvSeriesCalendar
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception) e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                LogException(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                LogException(exception, message);
            }

            throw exception;
        }

        private void LogException(Exception exception, string message)
        {
            using (StreamWriter writer = new StreamWriter("error.txt", true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now);
                writer.WriteLine();
                writer.WriteLine(exception.GetType().FullName);
                writer.WriteLine("Message : " + message);
                writer.WriteLine("StackTrace : " + exception.StackTrace);
            }
        }
    }
}