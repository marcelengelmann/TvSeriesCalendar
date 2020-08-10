using System;
using Newtonsoft.Json;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.Models
{
    public class TvSeries : ObservableObject
    {
        private string _imagePath;
        private DateTime? _nextSeasonReleaseDate;
        private int _watchedSeasons;

        [JsonConstructor] // Defines the Constructor for the JsonConvert.DeserializeObject method
        public TvSeries(int TMDbId, string name, int watchedSeasons, DateTime? nextSeasonReleaseDate,
            int numberReleasedSeasons, string status)
        {
            this.TMDbId = TMDbId;
            Name = name;
            WatchedSeasons = watchedSeasons;
            NextSeasonReleaseDate = nextSeasonReleaseDate;
            NumberReleasedSeasons = numberReleasedSeasons;
            Status = status;
        }

        public int TMDbId { get; }
        public string Name { get; }
        public int NumberReleasedSeasons { get; set; }
        public string Status { get; set; }

        public string ImagePath
        {
            get => _imagePath;
            set => OnPropertyChanged(ref _imagePath, value);
        }

        public int WatchedSeasons
        {
            get => _watchedSeasons;
            set => OnPropertyChanged(ref _watchedSeasons, value);
        }

        public DateTime? NextSeasonReleaseDate
        {
            get => _nextSeasonReleaseDate;
            set => OnPropertyChanged(ref _nextSeasonReleaseDate, value);
        }
    }
}