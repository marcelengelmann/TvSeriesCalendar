using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    internal class TvSeriesUpdater
    {
        public static async Task<(List<TvSeries> _updatedSeries, List<TvSeries> _todayReleasedSeasonSeries)> Update(
            SeriesLocalDataService localDataService, SeriesOnlineDataService onlineDataService,
            Action<int> progressUpdate)
        {
            List<TvSeries> series = new List<TvSeries>(localDataService.GetTvSeries());
            List<TvSeries> updatedSeries = new List<TvSeries>();
            List<TvSeries> todayReleasedSeasonSeries = new List<TvSeries>();
            string today = DateTime.Today.ToString("dd/MM/yyyy");
            for (int i = 0; i < series.Count; i++)
            {
                DateTime? oldReleaseDate = series[i].NextSeasonReleaseDate;
                series[i] = await onlineDataService.FindSeriesByTMDb(series[i].TMDbId, series[i].WatchedSeasons);
                if (Convert.ToDateTime(series[i].NextSeasonReleaseDate).ToString("dd/MM/yyyy") == today)
                    todayReleasedSeasonSeries.Add(series[i]);
                else if (oldReleaseDate == null && series[i].WatchedSeasons < series[i].NumberReleasedSeasons)
                    if (series[i].NextSeasonReleaseDate != null)
                        updatedSeries.Add(series[i]);
                int percentage = (int) ((i + 1) / (double) series.Count * 100);
                progressUpdate?.Invoke(percentage);
            }

            localDataService.Save(new ObservableCollection<TvSeries>(series));
            return (updatedSeries, todayReleasedSeasonSeries);
        }
    }
}