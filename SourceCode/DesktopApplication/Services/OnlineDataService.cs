using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    public class OnlineDataService
    {
        private TMDb Client;

        public OnlineDataService()
        {
            string APIKey = "a9c88a8f1f7e39aad266493c739ae488"; // load from Settings
            Client = new TMDb(APIKey);
        }

        public async Task FetchTMDbConfig()
        {
            await Client.FetchConfig(Client.client);
        }

        public async Task<TvSeries> FindSeriesbyTMDb(int tmdbID, int watchedSeasons = 0)
        {
            //if (User.UserTvSeries.FindIndex(i => i.TMDbID == tmdbID) >= 0) return null; //if series already exists
            (string Name, int SeasonsCount, string Status, DateTime? NextSeasonRelease, Images Images) = await Client.findByTMDb(tmdbID, watchedSeasons);
            TvSeries NewSeries = new TvSeries(tmdbID, Name, watchedSeasons, NextSeasonRelease, SeasonsCount, Status);
            saveSeriesImageLocally(Images, NewSeries);
            return NewSeries;
        }

        public async Task<int> SearchForSeries(string query, int pageNumber, ObservableCollection<TvSeries> SeriesList)
        {
            (int TotalPages, List<SearchTv> Result) = await Client.SearchForTvShows(query, page: pageNumber);
            foreach(SearchTv sTv in Result)
            {
                SeriesList.Add(await FindSeriesbyTMDb(sTv.Id));
            }
            return TotalPages;
        }
        public DateTime? getNextSeasonReleaseDate(int TMDbID, int watchedSeasons)
        {
            DateTime? release = null;
            TMDbLib.Objects.TvShows.TvSeason result = Client.getSeason(TMDbID, watchedSeasons+1);
            if (result != null)
                release = result.AirDate;
            return release;
        }

        private async void saveSeriesImageLocally(Images img, TvSeries Series, string lang = "en")
        {
            int TMDbID = Series.TMDbID;
            string savePath = $"images/{TMDbID}.jpg";
            if (!File.Exists("images/" + TMDbID + ".jpg"))
            {
                List<ImageData> langSpec = img.Posters.FindAll(e => e.Iso_639_1 == lang);
                //TODO: if language of user available: sort by language preferences (main lang/backup lang)
                if (langSpec.Count == 0)
                    langSpec = img.Posters;
                if (langSpec.Count == 0)
                    return;
                Uri url = Client.GetImageUrl(langSpec[0].FilePath);
                await new WebClient().DownloadFileTaskAsync(url, savePath);
            }
            Series.ImagePath = savePath;
        }
    }
}