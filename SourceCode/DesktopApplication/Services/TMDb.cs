using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

// ReSharper disable InconsistentNaming

namespace TvSeriesCalendar.Services
{
    /*
     * TMDb Class
     * Get all required TMDb Information for the required purpose
     */

    public class TMDb
    {
        /*
         * Create a new Client with the given APIKey
         */

        public TMDb(string apiKey)
        {
            Client = new TMDbClient(apiKey, true);
        }

        public TMDbClient Client { get; }

        /*
         * Get Image of TvSeries and save it locally
         * Param: the URL of the Image
         * Return: local filepath to image
         */

        public Uri GetImageUrl(string imageLink)
        {
            return Client.GetImageUrl(Client.Config.Images.PosterSizes[3], imageLink, true);
        }

        /*
         * Get TvShow by given TMDb ID
         * Param: The TMDb ID of the Series
         * Return: the required general Information fields for the Series Object class
         */

        public async Task<(string Name, int SeasonsCount, string Status, DateTime? NextSeasonrelease, Images Images)>
            FindByTMDb(int TMDbId, int nextSeason = 1)
        {
            TvShow result = await Client.GetTvShowAsync(TMDbId, TvShowMethods.Images);
            DateTime? nextSeasonRelease = null;
            if (result.NumberOfSeasons >= nextSeason)
                nextSeasonRelease = result.Seasons.Find(item => item.SeasonNumber == nextSeason)?.AirDate;
            return (result.Name, result.NumberOfSeasons, result.Status, nextSeasonRelease, result.Images);
        }

        /*
         * Get Season by given TMDb ID and season number
         * Param: The TMDb ID and wanted Season number of the Series
         * Return: the required Season related fields for the TvSeries object Class
         */

        public TvSeason GetSeason(int TMDbId, int season)
        {
            TvSeason result = Client.GetTvSeasonAsync(TMDbId, season).Result;
            return result;
        }

        /*
         * Search for Series by Query(user input)
         * Param: The Query to search for and the number of the current search-page
         * Return: Search results of given page
         */

        public async Task<(int TotalPages, List<SearchTv> Result)> SearchForTvShows(string query, int page = 0)
        {
            SearchContainer<SearchTv> result = await Client.SearchTvShowAsync(query, page);
            return (result.TotalPages, result.Results);
        }

        public async Task FetchConfig(TMDbClient client)
        {
            FileInfo configJson = new FileInfo("settings\\config.json");


            if (configJson.Exists)
            {
                string json = File.ReadAllText(configJson.FullName, Encoding.UTF8);

                client.SetConfig(JsonConvert.DeserializeObject<TMDbConfig>(json));
            }
            else
            {
                TMDbConfig config = await client.GetConfigAsync();

                string json = JsonConvert.SerializeObject(config);
                File.WriteAllText(configJson.FullName, json, Encoding.UTF8);
            }
        }

        public async Task<List<SearchTv>> GetSearchSuggestions(string searchText, CancellationToken cancellationToken)
        {
            return (await Client.SearchTvShowAsync(searchText, cancellationToken: cancellationToken)).Results;
        }
    }
}