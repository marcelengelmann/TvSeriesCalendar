using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvSeriesCalendar.Models;

namespace TvSeriesCalendar.Services
{
    class TvSeriesUpdater
    {
        public static (List<TvSeries> _updatedSeries, List<TvSeries> _todayReleasedSeasonSeries) Update(LocalDataService _localDataService, OnlineDataService _onlineDataService)
        {

            List<TvSeries> _series = new List<TvSeries>(_localDataService.GetTvSeries());
            bool changesMade = false;
            List<TvSeries> _updatedSeries = new List<TvSeries>(); ;
            List<TvSeries> _todayReleasedSeasonSeries = new List<TvSeries>(); ;
            string today = DateTime.Today.ToString("dd/MM/yyyy");
            _series.ForEach(e =>
            {
                if (e.NextSeasonReleaseDate == null && e.WatchedSeasons < e.NumberReleasedSeasons)
                {
                    e.NextSeasonReleaseDate = _onlineDataService.getNextSeasonReleaseDate(e.TMDbID, e.WatchedSeasons);
                    if (e.NextSeasonReleaseDate != null)
                    {
                        changesMade = true;
                        if (_updatedSeries != null) ;
                        _updatedSeries.Add(e);
                    }
                }
                else if (Convert.ToDateTime(e.NextSeasonReleaseDate).ToString("dd/MM/yyyy") == today)
                {
                    _todayReleasedSeasonSeries.Add(e);
                }
            });
            if (changesMade)
                _localDataService.Save(new ObservableCollection<TvSeries>(_series));
            return (_updatedSeries, _todayReleasedSeasonSeries);
        }
    }
}
