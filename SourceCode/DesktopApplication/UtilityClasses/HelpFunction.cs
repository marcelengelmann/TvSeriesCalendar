using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TvSeriesCalendar.UtilityClasses
{
    public class HelpFunction
    {
        public static string StringToValidFilename(string Filename)
        {

            string pattern = string.Format("[{0}]", Regex.Escape(String.Join("", System.IO.Path.GetInvalidFileNameChars())));
            return Regex.Replace(Filename, pattern, "");
        }
    }
}
