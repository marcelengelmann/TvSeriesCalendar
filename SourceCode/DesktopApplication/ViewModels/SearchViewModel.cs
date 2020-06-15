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
        private TvSeries _selectedTvSeries;
        private LocalDataService _localDataService;
        private OnlineDataService _onlineDataService;
        private string _searchText;
        private bool _showSearchLabel;
        private bool _showDetailedView;
        private int _currentPageNumber;
        private int _searchPagesNumber;
        private int _watchedSeasonsCounter;
        public ICommand StartNewSearchCommand { get; private set; }
        public ICommand UpdateSearchPageNumberCommand { get; private set; }
        public ICommand UpdateWatchedSeasonsCommand { get; private set; }
        public ICommand AddSelectedSeriesCommand { get; private set; }
        public ICommand CloseDetailedViewCommand { get; private set; }

        public ObservableCollection<TvSeries> SearchSeries { get; private set; }

        public TvSeries SelectedSeries
        {
            get { return _selectedTvSeries; }
            set { 
                if(_showDetailedView == true && value != null)
                    return;
                else if (value != null)
                    OpenDetailedView();

                OnPropertyChanged(ref _selectedTvSeries, value);
            }                
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

        public string SearchText
        {
            get { return _searchText; }
            set {
                OnPropertyChanged(ref _searchText, value);
                if (value.Length == 0)
                    ShowSearchLabel = true;
            }
        }

        public bool ShowSearchLabel
        {
            get { return _showSearchLabel; }
            set { OnPropertyChanged(ref _showSearchLabel, value); }
        }

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set {
                OnPropertyChanged(ref _currentPageNumber, value);
                SearchSeries.Clear();
                StartSearch();
            }
        }

        public int SearchPagesNumber
        {
            get { return _searchPagesNumber; }
            set { OnPropertyChanged(ref _searchPagesNumber, value); }
        }

        public int WatchedSeasonsCounter { get => _watchedSeasonsCounter; set => OnPropertyChanged(ref _watchedSeasonsCounter, value); }

        private void StartNewSearch()
        {
            if (SearchText.Length == 0)
                return;
            SearchPagesNumber = 1;
            CurrentPageNumber = 1;
        }

        private async void StartSearch()
        {
            SearchPagesNumber = await _onlineDataService.SearchForSeries(SearchText, CurrentPageNumber, SearchSeries);
        }

        private void UpdateSearchPageNumber(string ArithChar)
        {
            if(ArithChar == "+")
                CurrentPageNumber++;
            else if (ArithChar == "-")
                CurrentPageNumber--;
        }

        private void AddSelectedSeries()
        {
            ObservableCollection<TvSeries> tvSeries = _localDataService.GetTvSeries();
            if(!tvSeries.Any(e => e.TMDbID == SelectedSeries.TMDbID)){
                if(WatchedSeasonsCounter > 0)
                    SelectedSeries.NextSeasonReleaseDate = _onlineDataService.getNextSeasonReleaseDate(SelectedSeries.TMDbID, WatchedSeasonsCounter);
                SelectedSeries.WatchedSeasons = WatchedSeasonsCounter;
                tvSeries.Add(SelectedSeries);
                _localDataService.Save(tvSeries);
            }
            CloseDetailedView();
        }

        private void UpdateWatchedSeasons(string ArithChar)
        {
            if (ArithChar == "-" && WatchedSeasonsCounter > 0)
                WatchedSeasonsCounter--;
            else if (ArithChar == "+" && WatchedSeasonsCounter < SelectedSeries.NumberReleasedSeasons)
                WatchedSeasonsCounter++;
        }

        public SearchViewModel(LocalDataService dataService, OnlineDataService TMDb)
        {
            _localDataService = dataService;
            SearchText = "";
            StartNewSearchCommand = new RelayCommand(StartNewSearch);
            UpdateSearchPageNumberCommand = new RelayCommand<string>(UpdateSearchPageNumber);
            UpdateWatchedSeasonsCommand = new RelayCommand<string>(UpdateWatchedSeasons);
            AddSelectedSeriesCommand = new RelayCommand(AddSelectedSeries);
            CloseDetailedViewCommand = new RelayCommand(CloseDetailedView);
            _onlineDataService = TMDb;
            SearchSeries = new ObservableCollection<TvSeries>();
        }
    }
}