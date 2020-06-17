using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using TMDbLib.Objects.Languages;
using System.Reflection;

namespace TvSeriesCalendar.Services
{
    class ApplicationUpdater
    {
        internal static string NewVersionExists()
        {
            string versionFileURL = "https://raw.githubusercontent.com/Death-Truction/TvSeriesCalendar/master/Releases/newestVersion.txt";
            string newestVersion = (new WebClient()).DownloadString(versionFileURL).Replace("\n", "").Replace("\r", "");
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string currentVersion = fvi.FileVersion;
            if(newestVersion.Equals(currentVersion)) return "";
            return newestVersion;
        }

        internal static async Task Update(string version, DownloadProgressChangedEventHandler progressUpdate)
        {
            string configuration = "";
            string language = "en";

            //TODO: Get language from regestry key
            if (IntPtr.Size == 4) configuration = "x86"; //using 32Bit
            else configuration = "x64";


            string fileName = $"TvSeriesCalendar-{version}-{configuration}_{language}.msi";
            string link = $"https://raw.githubusercontent.com/Death-Truction/TvSeriesCalendar/master/Releases/Updates/{version}/{fileName}";
            string saveFilePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + $@"\updates\{fileName}";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += progressUpdate;
                await wc.DownloadFileTaskAsync(
                    new Uri(link),
                    saveFilePath
                );
            }
            //TODO: Run msi file with /passive mode
            Process process = new Process();
            process.StartInfo.FileName = "msiexec";
            process.StartInfo.Arguments = $" /passive /i {saveFilePath}";
            process.Start();
            process.Close();
        }
    }
}

