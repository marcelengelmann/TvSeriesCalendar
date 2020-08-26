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
        private readonly SeriesLocalDataService _seriesLocalDataService;
        private readonly SeriesOnlineDataService _seriesOnlineDataService;
        private TvSeries _selectedTvSeries;
        private bool _showDetailedView;
        private int _watchedSeasonsCounter;


        public SeriesViewModel(SeriesLocalDataService localDataService, SeriesOnlineDataService onlineDataService)
        {
            _seriesLocalDataService = localDataService;
            _seriesOnlineDataService = onlineDataService;
            CloseDetailedViewCommand = new RelayCommand(CloseDetailedView);
            CloseDetailedViewByXCommand = new RelayCommand(CloseDetailedViewByX);
            UpdateWatchedSeasonsCommand = new RelayCommand<string>(UpdateWatchedSeasons);
            SaveSelectedSeriesCommand = new RelayCommand(SaveSelectedSeries);
            RemoveSelectedSeriesCommand = new RelayCommand(RemoveSelectedSeries);
            LoadSeries(_seriesLocalDataService.GetTvSeries());
        }

        public ICommand CloseDetailedViewCommand { get; }
        public ICommand UpdateWatchedSeasonsCommand { get; }
        public ICommand SaveSelectedSeriesCommand { get; }
        public ICommand CloseDetailedViewByXCommand { get; }
        public ICommand RemoveSelectedSeriesCommand { get; }
        public ObservableCollection<TvSeries> Series { get; private set; }

        public TvSeries SelectedSeries
        {
            get => _selectedTvSeries;
            set
            {
                if (_showDetailedView && value != null)
                {
                    return;
                }

                if (value != null)
                {
                    WatchedSeasonsCounter = value.WatchedSeasons;
                    OpenDetailedView();
                }

                OnPropertyChanged(ref _selectedTvSeries, value);
            }
        }

        public int WatchedSeasonsCounter
        {
            get => _watchedSeasonsCounter;
            set => OnPropertyChanged(ref _watchedSeasonsCounter, value);
        }

        private void OpenDetailedView()
        {
            _showDetailedView = true;
        }

        private void CloseDetailedViewByX()
        {
            if (_watchedSeasonsCounter != SelectedSeries.WatchedSeasons)
            {
                MessageBoxResult dialogResult =
                    MessageBox.Show("There are Unsaved Changes.\nDo you want to Save them now?", "Unsaved Changes",
                        MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes) SaveSelectedSeries();
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
            _seriesLocalDataService.Save(Series);
            CloseDetailedView();
        }

        private void UpdateWatchedSeasons(string arithChar)
        {
            switch (arithChar)
            {
                case "-" when WatchedSeasonsCounter > 0:
                    WatchedSeasonsCounter--;
                    break;
                case "+" when WatchedSeasonsCounter < SelectedSeries.NumberReleasedSeasons:
                    WatchedSeasonsCounter++;
                    break;
            }
        }


        public void LoadSeries(ObservableCollection<TvSeries> series)
        {
            Series = series;
            OnPropertyChanged("Series");
        }

        private void SaveSelectedSeries()
        {
            SelectedSeries.WatchedSeasons = WatchedSeasonsCounter;
            SelectedSeries.NextSeasonReleaseDate =
                _seriesOnlineDataService.GetNextSeasonReleaseDate(SelectedSeries.TMDbId, WatchedSeasonsCounter);
            CloseDetailedView();
            _seriesLocalDataService.Save(Series);
        }
    }
}