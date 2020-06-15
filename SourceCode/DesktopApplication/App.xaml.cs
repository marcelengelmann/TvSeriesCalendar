using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace TvSeriesCalendar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

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
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                logException(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                logException(exception, message);
            }
            throw exception;
        }

        private void logException(Exception exception, string message)
        {
            using (StreamWriter writer = new StreamWriter("error.txt", true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();
                writer.WriteLine(exception.GetType().FullName);
                writer.WriteLine("Message : " + message);
                writer.WriteLine("StackTrace : " + exception.StackTrace);
            }
        }
    }
}
