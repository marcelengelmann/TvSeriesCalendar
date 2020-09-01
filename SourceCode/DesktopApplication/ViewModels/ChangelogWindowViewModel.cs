using System.Threading.Tasks;
using Markdig;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    internal class ChangelogWindowViewModel : ObservableObject
    {
        private string _markdownText;


        public ChangelogWindowViewModel()
        {
            const string url = "https://raw.githubusercontent.com/Death-Truction/TvSeriesCalendar/master/Changelog.md";
            Task<string> t = Task.Run(async () => await GetFromGithub.DownloadAsString(url));
            string changelog = t.Result;
            MarkdownText = Markdown.ToHtml(changelog);
        }

        public string MarkdownText
        {
            get => _markdownText;
            set => OnPropertyChanged(ref _markdownText, value);
        }
    }
}