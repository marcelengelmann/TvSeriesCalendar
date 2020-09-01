using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TvSeriesCalendar.Services
{
    /// <summary>
    ///     Class used to Interact with Formspree
    /// </summary>
    internal static class FormspreeReport
    {
        public enum ReportType
        {
            ErrorReport,
            FeatureRequest
        }


        /// <summary>
        ///     Send a HTTP Post Message to Formspree
        /// </summary>
        /// <param name="message">The data to transmit</param>
        /// <param name="reportType">The Type of Report</param>
        /// <param name="name">Optional: The Name of the User</param>
        /// <param name="eMail">Optional: The E-Mail of the User</param>
        /// <returns></returns>
        internal static async Task<string> Send(string message, ReportType reportType, string name = "unknown",
            string eMail = "None")
        {
            string reportTypeString;
            switch (reportType)
            {
                case ReportType.ErrorReport:
                    reportTypeString = "Error Report";
                    break;
                case ReportType.FeatureRequest:
                    reportTypeString = "Feature Request";
                    break;
                default:
                    reportTypeString = "";
                    break;
            }

            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", name),
                new KeyValuePair<string, string>("E-Mail", eMail),
                new KeyValuePair<string, string>("ReportType", reportTypeString),
                new KeyValuePair<string, string>("Message", message)
            });

            HttpClient myHttpClient = new HttpClient();
            HttpResponseMessage response = await myHttpClient.PostAsync("https://formspree.io/xnqggald", formContent);

            return response.IsSuccessStatusCode ? "Success" : "Error";
        }
    }
}