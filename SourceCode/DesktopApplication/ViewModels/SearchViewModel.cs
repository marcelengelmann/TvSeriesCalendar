using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class SearchViewModel : ObservableObject
    {
        private readonly SeriesLocalDataService _seriesLocalDataService;
        private readonly SeriesOnlineDataService _seriesOnlineDataService;
        private int _currentPageNumber;
        private int _searchPagesNumber;
        private string _searchText;
        private TvSeries _selectedTvSeries;
        private bool _showDetailedView;
        private bool _showSearchLabel;
        private int _watchedSeasonsCounter;

        public SearchViewModel(SeriesLocalDataService dataService, SeriesOnlineDataService TMDbId)
        {
            _seriesLocalDataService = dataService;
            SearchText = "";
            StartNewSearchCommand = new RelayCommand(StartNewSearch);
            UpdateSearchPageNumberCommand = new RelayCommand<string>(UpdateSearchPageNumber);
            UpdateWatchedSeasonsCommand = new RelayCommand<string>(UpdateWatchedSeasons);
            AddSelectedSeriesCommand = new RelayCommand(AddSelectedSeries);
            CloseDetailedViewCommand = new RelayCommand(CloseDetailedView);
            _seriesOnlineDataService = TMDbId;
            SearchSeries = new ObservableCollection<TvSeries>();
        }

        public ICommand StartNewSearchCommand { get; }
        public ICommand UpdateSearchPageNumberCommand { get; }
        public ICommand UpdateWatchedSeasonsCommand { get; }
        public ICommand AddSelectedSeriesCommand { get; }
        public ICommand CloseDetailedViewCommand { get; }

        public ObservableCollection<TvSeries> SearchSeries { get; }

        public TvSeries SelectedSeries
        {
            get => _selectedTvSeries;
            set
            {
                if (_showDetailedView && value != null)
                    return;
                if (value != null)
                    OpenDetailedView();

                OnPropertyChanged(ref _selectedTvSeries, value);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                OnPropertyChanged(ref _searchText, value);
                if (value.Length == 0)
                    ShowSearchLabel = true;
            }
        }

        public bool ShowSearchLabel
        {
            get => _showSearchLabel;
            set => OnPropertyChanged(ref _showSearchLabel, value);
        }

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                OnPropertyChanged(ref _currentPageNumber, value);
                SearchSeries.Clear();
                StartSearch();
            }
        }

        public int SearchPagesNumber
        {
            get => _searchPagesNumber;
            set => OnPropertyChanged(ref _searchPagesNumber, value);
        }

        public int WatchedSeasonsCounter
        {
            get => _watchedSeasonsCounter;
            set => OnPropertyChanged(ref _watchedSeasonsCounter, value);
        }

        private void OpenDetailedView()
        {
            WatchedSeasonsCounter = 0;
            _showDetailedView = true;
        }

        private void CloseDetailedView()
        {
            _showDetailedView = false;
            SelectedSeries = null;
        }

        private void StartNewSearch()
        {
            if (SearchText.Length == 0)
                return;
            SearchPagesNumber = 1;
            CurrentPageNumber = 1;
        }

        private async void StartSearch()
        {
            SearchPagesNumber = await _seriesOnlineDataService.SearchForSeries(SearchText, CurrentPageNumber, SearchSeries);
        }

        private void UpdateSearchPageNumber(string arithChar)
        {
            switch (arithChar)
            {
                case "+":
                    CurrentPageNumber++;
                    break;
                case "-":
                    CurrentPageNumber--;
                    break;
            }
        }

        private void AddSelectedSeries()
        {
            ObservableCollection<TvSeries> tvSeries = _seriesLocalDataService.GetTvSeries();
            if (tvSeries.All(e => e.TMDbId != SelectedSeries.TMDbId))
            {
                if (WatchedSeasonsCounter > 0)
                    SelectedSeries.NextSeasonReleaseDate =
                        _seriesOnlineDataService.GetNextSeasonReleaseDate(SelectedSeries.TMDbId, WatchedSeasonsCounter);
                SelectedSeries.WatchedSeasons = WatchedSeasonsCounter;
                tvSeries.Add(SelectedSeries);
                _seriesLocalDataService.Save(tvSeries);
            }

            CloseDetailedView();
        }

        private void UpdateWatchedSeasons(string arithChar)
        {
            if (arithChar == "-" && WatchedSeasonsCounter > 0)
                WatchedSeasonsCounter--;
            else if (arithChar == "+" && WatchedSeasonsCounter < SelectedSeries.NumberReleasedSeasons)
                WatchedSeasonsCounter++;
        }
    }
}