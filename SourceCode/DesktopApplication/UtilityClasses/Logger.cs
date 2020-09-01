using System;
using System.IO;
using System.Reflection;

namespace TvSeriesCalendar.UtilityClasses
{
    public static class Logger
    {
        public static void Exception(Exception exception, string source)
        {
            string exceptionOutput = CreateExceptionOutput(exception, source);
            using (StreamWriter writer = new StreamWriter("error.txt", true))
            {
                writer.WriteLine(exceptionOutput);
            }
        }

        public static string CreateExceptionOutput(Exception exception, string source)
        {
            return "-----------------------------------------------------------------------------\n" +
                   $"Date : {DateTime.Now}\n" +
                   $"{exception.GetType().FullName}\n" +
                   $"Source : {source}\n" +
                   $"Message: {exception.Message}\n" +
                   $"StackTrace : {exception.StackTrace}\n";

        }
    }
}