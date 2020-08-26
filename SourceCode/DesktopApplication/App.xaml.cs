using System;
using System.IO;
using System.Reflection;
using System.Windows;
using TvSeriesCalendar.UtilityClasses;

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
                Logger.Exception((Exception) e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                Logger.Exception(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };
        }    }
}