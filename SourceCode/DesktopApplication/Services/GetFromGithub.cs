using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TvSeriesCalendar.Services
{
    internal static class GetFromGithub
    {
        internal static async Task<string> DownloadAsString(string URL)
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "TvSeriesCalendar");

            try
            {
                return await client.DownloadStringTaskAsync(URL);
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal static async Task DownloadFileAsync(string url, DownloadProgressChangedEventHandler progressUpdate = null)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += progressUpdate;
                wc.Headers.Add("User-Agent", "TvSeriesCalendar");
                await wc.DownloadFileTaskAsync(
                    new Uri(url),
                    "update.zip"
                );
            }
        }
    }
}
