using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TvSeriesCalendar.Properties;
using TvSeriesCalendar.UtilityClasses;

// ReSharper disable InconsistentNaming

namespace TvSeriesCalendar.Services
{
    internal class ApplicationUpdater
    {
        /// <summary>
        ///     Gets the Releases of the project from github and compares the latest release version with the assembly version
        /// </summary>
        /// <returns></returns>
        internal static async Task<Assets[]> NewVersionExists()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string currentVersion = fvi.FileVersion;
            const string releasesUrl = "https://api.github.com/repos/Death-Truction/TvSeriesCalendar/releases";
            string githubReleases = await GetFromGithub.DownloadAsString(releasesUrl);
            if (githubReleases == "") //e.g. happens when api quota reached
                return null;

            List<Release> allReleases = JsonConvert.DeserializeObject<List<Release>>(githubReleases);
            allReleases.Sort((a, b) => string.CompareOrdinal(b.Tag_Name, a.Tag_Name));
            Release latestRelease = allReleases.Find(release => release.PreRelease == true);
            if (latestRelease == null)
                return null;
            return string.CompareOrdinal(currentVersion, latestRelease.Tag_Name) < 0 ? latestRelease.Assets : null;
        }

        internal static async Task Update(Assets[] assets, DownloadProgressChangedEventHandler progressUpdate)
        {
            string url;
            if (IntPtr.Size == 4)
                //using 32Bit
                url = assets[0].Name.Contains("x86") ? assets[0].Browser_download_url : assets[1].Browser_download_url;
            else // using 64Bit
                url = assets[0].Name.Contains("x64") ? assets[0].Browser_download_url : assets[1].Browser_download_url;

            await GetFromGithub.DownloadFileAsync(url, progressUpdate);

            ZipFile.ExtractToDirectory("update.zip", "update\\");
            File.Delete("update.zip");
            try
            {
                Process scriptProc = new Process
                {
                    StartInfo =
                    {
                        FileName = @"cscript",
                        Arguments = "//B //Nologo \"" + AppDomain.CurrentDomain.BaseDirectory +
                                    "update\\update.vbs\" " + Process.GetCurrentProcess().Id + " \"" +
                                    AppDomain.CurrentDomain.BaseDirectory + "\"",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                scriptProc.Start();
                scriptProc.Close();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "ApplicationUpdater.Update");
            }
        }

        internal class Release
        {
            public string Tag_Name { get; set; }
            public Assets[] Assets { get; set; }
            public bool PreRelease { get; set; }
        }

        internal class Assets
        {
            public string Name { get; set; }
            public string Browser_download_url { get; set; }
        }
    }
}