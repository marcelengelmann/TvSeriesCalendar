using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    public class SeriesOnlineDataService
    {
        private readonly TMDb _client;

        public SeriesOnlineDataService()
        {
            string apiKey = "a9c88a8f1f7e39aad266493c739ae488"; // load from Settings
            _client = new TMDb(apiKey);
        }

        // ReSharper disable once InconsistentNaming
        public async Task FetchTMDbConfig()
        {
            await _client.FetchConfig(_client.Client);
        }

        // ReSharper disable once InconsistentNaming
        public async Task<TvSeries> FindSeriesByTMDb(int TMDbId, int watchedSeasons = 0)
        {
            (string name, int seasonsCount, string status, DateTime? nextSeasonRelease, Images images) =
                await _client.FindByTMDb(TMDbId, watchedSeasons + 1);
            TvSeries newSeries = new TvSeries(TMDbId, name, watchedSeasons, nextSeasonRelease, seasonsCount, status);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SaveSeriesImageLocally(images, newSeries); //awaiting not required
#pragma warning restore CS4014

            return newSeries;
        }

        public async Task<int> SearchForSeries(string query, int pageNumber, ObservableCollection<TvSeries> seriesList)
        {
            (int totalPages, List<SearchTv> result) = await _client.SearchForTvShows(query, pageNumber);
            foreach (SearchTv sTv in result) seriesList.Add(await FindSeriesByTMDb(sTv.Id));
            return totalPages;
        }

        public DateTime? GetNextSeasonReleaseDate(int TMDbId, int watchedSeasons)
        {
            DateTime? release = null;
            TvSeason result = _client.GetSeason(TMDbId, watchedSeasons + 1);
            if (result != null)
                release = result.AirDate;
            return release;
        }

        private async Task SaveSeriesImageLocally(Images img, TvSeries series, string lang = "en")
        {
            int TMDbId = series.TMDbId;
            string savePath = $"images/{TMDbId}.jpg";
            if (!File.Exists("images/" + TMDbId + ".jpg"))
            {
                List<ImageData> langSpec = img.Posters.FindAll(e => e.Iso_639_1 == lang);
                //TODO: if language of user available: sort by language preferences (main lang/backup lang)
                if (langSpec.Count == 0)
                    langSpec = img.Posters;
                if (langSpec.Count == 0)
                    return;
                Uri url = _client.GetImageUrl(langSpec[0].FilePath);
                await new WebClient().DownloadFileTaskAsync(url, savePath);
            }

            series.ImagePath = savePath;
        }

        public async Task<ObservableCollection<string>> GetSearchSuggestions(string searchText,
            CancellationToken cancellationToken)
        {
            List<SearchTv> result = await _client.GetSearchSuggestions(searchText, cancellationToken);
            return new ObservableCollection<string>(result.ConvertAll(item => item.Name));
        }
    }
}