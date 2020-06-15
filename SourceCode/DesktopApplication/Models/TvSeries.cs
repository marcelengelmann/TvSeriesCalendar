using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.Models
{
    public class TvSeries : ObservableObject
    {
        private int _watchedSeasons;
        private DateTime? _nextSeasonReleaseDate;
        private string _imagePath;
        public int TMDbID { get; private set; }
        public string Name { get; private set; }
        public int NumberReleasedSeasons { get; set; }
        public string Status { get; set; }
        [JsonConstructor] // Defines the Constructor for the JsonConvert.DeserializeObject method
        public TvSeries(int TMDbID, string Name, int WatchedSeasons, DateTime? NextSeasonReleaseDate, int NumberReleasedSeasons, string Status)
        {
            this.TMDbID = TMDbID;
            this.Name = Name;
            this.WatchedSeasons = WatchedSeasons;
            this.NextSeasonReleaseDate = NextSeasonReleaseDate;
            this.NumberReleasedSeasons = NumberReleasedSeasons;
            this.Status = Status;
        }

        public string ImagePath {
            get { return _imagePath; }
            set { OnPropertyChanged(ref _imagePath, value); } 
        }
        public int WatchedSeasons
        {
            get { return _watchedSeasons; }
            set { OnPropertyChanged(ref _watchedSeasons, value); }
        }

        public DateTime? NextSeasonReleaseDate
        {
            get { return _nextSeasonReleaseDate; }
            set { OnPropertyChanged(ref _nextSeasonReleaseDate, value); }
        }
    }
}