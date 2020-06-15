using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace TvSeriesCalendar.Services
{
    /*
     * TMDB Class
     * Get all required TMDB Informations for the required purpose
     */

    public class TMDb
    {
        public TMDbClient client { get; private set; }
        /*
         * Create a new Client with the given APIKey
         */

        public TMDb(string APIKey)
        {
            client = new TMDbClient(APIKey, useSsl: true);
        }

        /*
         * Get Image of TvSeries and save it locally
         * Param: the URL of the Image
         * Return: local filepath to image
         */

        public Uri GetImageUrl(string ImageLink)
        {
            return client.GetImageUrl(client.Config.Images.PosterSizes[3], ImageLink, useSsl: true);
        }

        /*
         * Get TvShow by given TMDB ID
         * Param: The TMDb ID of the Series
         * Return: the required general Information fields for the Series Object class
         */

        public async Task<(string Name, int SeasonsCount, string Status, DateTime? NextSeasonrelease, Images Images)> findByTMDb(int tmdbID, int NextSeason = 0)
        {
            TvShow result = await client.GetTvShowAsync(tmdbID, TvShowMethods.Images);
            DateTime? nextSeasonRelease = null;
            if (result.Seasons.Count > NextSeason)
                nextSeasonRelease = result.Seasons.ElementAt(NextSeason).AirDate;
            return (result.Name, result.NumberOfSeasons, result.Status, nextSeasonRelease, result.Images);
        }

        /*
         * Get Season by given TMDB ID and season number
         * Param: The TMDb ID and wanted Season number of the Series
         * Return: the required Season related fields for the TvSeries object Class
         */

        public TvSeason getSeason(int tmdbID, int season)
        {
            TvSeason result = client.GetTvSeasonAsync(tmdbID, season).Result;
            return result;
        }

        /*
         * Search for Series by Query(user input)
         * Param: The Query to search for and the number of the current searchpage
         * Return: Search results of given page
         */

        public async Task<(int TotalPages, List<SearchTv> Result)> SearchForTvShows(string query, int page = 0)
        {
            SearchContainer<SearchTv> result = await client.SearchTvShowAsync(query, page);
            return (result.TotalPages, result.Results);
        }

        public async Task FetchConfig(TMDbClient client)
        {
            FileInfo configJson = new FileInfo("settings\\config.json");

            Console.WriteLine("Config file: " + configJson.FullName + ", Exists: " + configJson.Exists);

            if (configJson.Exists)
            {
                Console.WriteLine("Using stored config");
                string json = File.ReadAllText(configJson.FullName, Encoding.UTF8);

                client.SetConfig(JsonConvert.DeserializeObject<TMDbConfig>(json));
            }
            else
            {
                Console.WriteLine("Getting new config");
                var config = await client.GetConfigAsync();

                Console.WriteLine("Storing config");
                string json = JsonConvert.SerializeObject(config);
                File.WriteAllText(configJson.FullName, json, Encoding.UTF8);
            }
        }
    }
}