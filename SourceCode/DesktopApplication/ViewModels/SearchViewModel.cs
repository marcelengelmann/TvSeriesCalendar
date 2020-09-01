using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private CancellationTokenSource _cancelSearchSuggestionToken;
        private int _currentPageNumber;
        private bool _searchIsNotNotLocked;
        private int _searchPagesNumber;
        private ObservableCollection<string> _searchSuggestions;
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
            SearchIsNotLocked = true;
            _cancelSearchSuggestionToken = new CancellationTokenSource();
            _searchSuggestions = new ObservableCollection<string>();
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
                if (SearchSeries?.Count > 0)
                    SearchSeries.Clear();
                if (value.Length > 0)
                {
                    _cancelSearchSuggestionToken?.Cancel();
                    _cancelSearchSuggestionToken = new CancellationTokenSource();
                    Task.Run(async () =>
                        SearchSuggestions =
                            await _seriesOnlineDataService.GetSearchSuggestions(value,
                                _cancelSearchSuggestionToken.Token));
                }

                if (SearchPagesNumber > 0)
                    SearchPagesNumber = 0;
                if (CurrentPageNumber > 0)
                    CurrentPageNumber = 0;
            }
        }

        public ObservableCollection<string> SearchSuggestions
        {
            get => _searchSuggestions;
            set => OnPropertyChanged(ref _searchSuggestions, value);
        }

        public bool ShowSearchLabel
        {
            get => _showSearchLabel;
            set => OnPropertyChanged(ref _showSearchLabel, value);
        }

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set => OnPropertyChanged(ref _currentPageNumber, value);
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

        public bool SearchIsNotLocked
        {
            get => _searchIsNotNotLocked;
            set => OnPropertyChanged(ref _searchIsNotNotLocked, value);
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

        private async void StartNewSearch()
        {
            if (SearchText.Length == 0)
                return;
            CurrentPageNumber = 1;
            await StartSearch();
        }

        private async Task StartSearch()
        {
            SearchIsNotLocked = false;

            SearchSeries?.Clear();
            SearchPagesNumber =
                await _seriesOnlineDataService.SearchForSeries(SearchText, CurrentPageNumber, SearchSeries);

            SearchIsNotLocked = true;
        }

        private async void UpdateSearchPageNumber(string arithChar)
        {
            switch (arithChar)
            {
                case "+":
                    CurrentPageNumber++;
                    await StartSearch();
                    break;
                case "-":
                    CurrentPageNumber--;
                    await StartSearch();
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