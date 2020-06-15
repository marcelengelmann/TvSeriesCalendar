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

namespace TvSeriesCalendar.Services
{
    class CheckForApplicationUpdates
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
            /*string configuration = "";

            if (IntPtr.Size == 4) configuration = "x86"; //using 32Bit
            else configuration = "x64";

            string link = $"https://raw.githubusercontent.com/Death-Truction/TvSeriesCalendar/master/Releases/Updates/{version}/{version}_{configuration}.zip";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += progressUpdate;
                await wc.DownloadFileTaskAsync(
                    new Uri(link),
                    "update.zip"
                );
            }
            ZipFile.ExtractToDirectory("update.zip", "update\\");
            File.Delete("update.zip");
            File.WriteAllText("update\\update.vbs", Properties.Resources.update);
            try
            {
                Process scriptProc = new Process();
                scriptProc.StartInfo.FileName = @"cscript";
                scriptProc.StartInfo.Arguments = "//B //Nologo \"" + AppDomain.CurrentDomain.BaseDirectory + "update\\update.vbs\" " + Process.GetCurrentProcess().Id + " \"" + AppDomain.CurrentDomain.BaseDirectory + "\"";
                scriptProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                scriptProc.Start();
                scriptProc.Close();
            }
            catch(Exception ex)
            {
            }
            */
        }
    }
}

