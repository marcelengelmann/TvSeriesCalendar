using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class SeriesViewModel : ObservableObject
    {
        private TvSeries _selectedTvSeries;
        private int _watchedSeasonsCounter;
        private LocalDataService _localDataService;
        private OnlineDataService _onlineDataService;
        private bool _showDetailedView;
        public ICommand CloseDetailedViewCommand { get; private set; }
        public ICommand UpdateWatchedSeasonsCommand { get; private set; }
        public ICommand SaveSelectedSeriesCommand { get; private set; }
        public ICommand CloseDetailedViewByXCommand { get; private set; }
        public ICommand RemoveSelectedSeriesCommand { get; private set; }
        public ObservableCollection<TvSeries> Series { get; private set; }

        public TvSeries SelectedSeries
        {
            get { return _selectedTvSeries; }
            set
            {
                if (_showDetailedView == true && value != null)
                    return;
                else if (value != null)
                {
                    WatchedSeasonsCounter = value.WatchedSeasons;
                    OpenDetailedView();
                }

                OnPropertyChanged(ref _selectedTvSeries, value);
            }
        }

        private void OpenDetailedView()
        {
            _showDetailedView = true;
        }

        public int WatchedSeasonsCounter
        {
            get { return _watchedSeasonsCounter; }
            set { OnPropertyChanged(ref _watchedSeasonsCounter, value); }
        }


        public SeriesViewModel(LocalDataService localDataService, OnlineDataService onlineDataService)
        {
            _localDataService = localDataService;
            _onlineDataService = onlineDataService;
            CloseDetailedViewCommand = new RelayCommand(CloseDetailedView);
            CloseDetailedViewByXCommand = new RelayCommand(CloseDetailedViewByX);
            UpdateWatchedSeasonsCommand = new RelayCommand<string>(UpdateWatchedSeasons);
            SaveSelectedSeriesCommand = new RelayCommand(SaveSelectedSeries);
            RemoveSelectedSeriesCommand = new RelayCommand(RemoveSelectedSeries);
            LoadSeries(_localDataService.GetTvSeries());
        }

        private void CloseDetailedViewByX()
        {
            if(_watchedSeasonsCounter != SelectedSeries.WatchedSeasons)
            {
                MessageBoxResult dialogResult = MessageBox.Show("There are Unsaved Changes.\nDo you want to Save them now?", "Unsaved Changes", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    SaveSelectedSeries();
                }
            }
            CloseDetailedView();
        }

        private void CloseDetailedView()
        {
            _showDetailedView = false;
            SelectedSeries = null;
        }

        private void RemoveSelectedSeries()
        {
            Series.Remove(SelectedSeries);
            _localDataService.Save(Series);
            CloseDetailedView();
        }

        private void UpdateWatchedSeasons(string ArithChar)
        {
            if (ArithChar == "-" && WatchedSeasonsCounter > 0)
                WatchedSeasonsCounter--;
            else if (ArithChar == "+" && WatchedSeasonsCounter < SelectedSeries.NumberReleasedSeasons)
                WatchedSeasonsCounter++;
        }


        public void LoadSeries(ObservableCollection<TvSeries> series)
        {
            Series = series;
            OnPropertyChanged("Series");
        }

        private void SaveSelectedSeries()
        {
            SelectedSeries.WatchedSeasons = WatchedSeasonsCounter;
            SelectedSeries.NextSeasonReleaseDate = _onlineDataService.getNextSeasonReleaseDate(SelectedSeries.TMDbID, WatchedSeasonsCounter);
            CloseDetailedView();
            _localDataService.Save(Series);
        }

        /*private async void CheckForTvSeriesUpdates()
        {
            foreach(TvSeries e in Series) { 
                if (e.NextSeasonReleaseDate == null && e.WatchedSeasons < e.NumberReleasedSeasons)
                {
                    e.NextSeasonReleaseDate = _onlineDataService.getNextSeasonReleaseDate(e.TMDbID, e.WatchedSeasons);
                }
            }
        }*/
    }
}