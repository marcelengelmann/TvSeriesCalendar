using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using System.Linq;
using MaterialDesignThemes.Wpf;

namespace TvSeriesCalendar.ViewModels
{
    public class SeriesViewModel : ObservableObject
    {
        private readonly SeriesLocalDataService _seriesLocalDataService;
        private readonly SeriesOnlineDataService _seriesOnlineDataService;
        private TvSeries _selectedTvSeries;
        private bool _showDetailedView;
        private int _watchedSeasonsCounter;
        private string _filterByNameText;
        private PackIconKind _sortByNamePackIconKind;
        private PackIconKind _sortByStatusPackIconKind;
        private PackIconKind _sortByReleaseDatePackIconKind;
        private PackIconKind _sortByAddedOrderPackIconKind;
        private ObservableCollection<TvSeries> _filteredSeries;
        private string _activeSortFunction;


        public SeriesViewModel(SeriesLocalDataService localDataService, SeriesOnlineDataService onlineDataService)
        {
            _seriesLocalDataService = localDataService;
            _seriesOnlineDataService = onlineDataService;
            CloseDetailedViewCommand = new RelayCommand(CloseDetailedView);
            CloseDetailedViewByXCommand = new RelayCommand(CloseDetailedViewByX);
            UpdateWatchedSeasonsCommand = new RelayCommand<string>(UpdateWatchedSeasons);
            SaveSelectedSeriesCommand = new RelayCommand(SaveSelectedSeries);
            RemoveSelectedSeriesCommand = new RelayCommand(RemoveSelectedSeries);
            SortByNameCommand = new RelayCommand(SortByName);
            SortByStatusCommand = new RelayCommand(SortByStatus);
            SortByReleaseDateCommand = new RelayCommand(SortByReleaseDate);
            SortByAddedOrderCommand = new RelayCommand(SortByAddedOrder);
            SortByNamePackIconKind = PackIconKind.SortAlphabeticalAscending;
            SortByStatusPackIconKind = PackIconKind.CheckBold;
            SortByReleaseDatePackIconKind = PackIconKind.CalendarImport;
            SortByAddedOrderPackIconKind = PackIconKind.Restore;
            LoadSeries(_seriesLocalDataService.GetTvSeries());
            FilteredSeries = new ObservableCollection<TvSeries>(Series);
            FilterByNameText = "";
            SortByAddedOrder();
            Series.CollectionChanged += SeriesOnCollectionChanged;
        }

        public ICommand CloseDetailedViewCommand { get; }
        public ICommand UpdateWatchedSeasonsCommand { get; }
        public ICommand SaveSelectedSeriesCommand { get; }
        public ICommand CloseDetailedViewByXCommand { get; }
        public ICommand RemoveSelectedSeriesCommand { get; }
        public ICommand SortByNameCommand { get; }
        public ICommand SortByStatusCommand { get; }
        public ICommand SortByReleaseDateCommand { get; }
        public ICommand SortByAddedOrderCommand { get; }

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


        public ObservableCollection<TvSeries> FilteredSeries
        {
            get => _filteredSeries; 
            set => OnPropertyChanged(ref _filteredSeries, value);
        }

        public int WatchedSeasonsCounter
        {
            get => _watchedSeasonsCounter;
            set => OnPropertyChanged(ref _watchedSeasonsCounter, value);
        }

        public string FilterByNameText
        {
            get => _filterByNameText;
            set
            {
                OnPropertyChanged(ref _filterByNameText, value);
                FilterByName();
            }
        }

        public PackIconKind SortByNamePackIconKind
        {
            get => _sortByNamePackIconKind;
            set => OnPropertyChanged(ref _sortByNamePackIconKind, value);
        }

        public PackIconKind SortByStatusPackIconKind
        {
            get => _sortByStatusPackIconKind;
            set => OnPropertyChanged(ref _sortByStatusPackIconKind, value);
        }

        public PackIconKind SortByReleaseDatePackIconKind
        {
            get => _sortByReleaseDatePackIconKind;
            set => OnPropertyChanged(ref _sortByReleaseDatePackIconKind, value);
        }

        public PackIconKind SortByAddedOrderPackIconKind
        {
            get => _sortByAddedOrderPackIconKind;
            set => OnPropertyChanged(ref _sortByAddedOrderPackIconKind, value);
        }

        public string ActiveSortFunction
        {
            get => _activeSortFunction;
            set => OnPropertyChanged(ref _activeSortFunction, value);
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

        private void SeriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Remove)
                return;
            FilteredSeries = new ObservableCollection<TvSeries>(Series);
            if (FilterByNameText?.Length > 0)
            {
                FilterByName();
            }
            switch (ActiveSortFunction)
            {
                case "SortByName":
                    SortByName();
                    break;
                case "SortByStatus":
                    SortByStatus();
                    break;
                case "SortByReleaseDate":
                    SortByReleaseDate();
                    break;
                case "SortByAddedOrder":
                    SortByAddedOrder();
                    break;
            }
        }

        // Filter an Sort Area
        public void FilterByName()
        {
            FilteredSeries.Clear();
            if (FilterByNameText.Length == 0)
            {
                FilteredSeries = new ObservableCollection<TvSeries>(Series);
            }
            else
            {
                foreach (TvSeries series in Series)
                {
                    if (series.Name.ToLower().Contains(FilterByNameText.ToLower()))
                    {
                        FilteredSeries.Add(series);
                    }
                }
            }
        }

        public void SortByName()
        {
            if (SortByNamePackIconKind == PackIconKind.SortAlphabeticalAscending && ActiveSortFunction == "SortByName")
            {
                SortByNamePackIconKind = PackIconKind.SortAlphabeticalDescending;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderByDescending(series => series.Name));
            }
            else
            {    
                SortByNamePackIconKind = PackIconKind.SortAlphabeticalAscending;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderBy(series => series.Name));
            }
            ActiveSortFunction = "SortByName";
        }

        private void SortByStatus()
        {
            if (SortByStatusPackIconKind == PackIconKind.CheckBold && ActiveSortFunction == "SortByStatus")
            {
                SortByStatusPackIconKind = PackIconKind.AlertBox;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderBy(OrderSeriesStatus));
            }
            else
            {
                SortByStatusPackIconKind = PackIconKind.CheckBold;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderByDescending(OrderSeriesStatus));
            }
            ActiveSortFunction = "SortByStatus";
        }

        private int OrderSeriesStatus(TvSeries series)
        {
            if (series.Status == "Canceled" || series.Status == "Ended")
                return 0;
            if (series.NextSeasonReleaseDate == null)
                return 1;
            return 2;
        }

        private void SortByReleaseDate()
        {
            if (SortByReleaseDatePackIconKind == PackIconKind.CalendarImport && ActiveSortFunction == "SortByReleaseDate")
            {
                SortByReleaseDatePackIconKind = PackIconKind.CalendarExport;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderByDescending(OrderByReleaseDate));
            }
            else
            {
                SortByReleaseDatePackIconKind = PackIconKind.CalendarImport;
                FilteredSeries = new ObservableCollection<TvSeries>(Series.OrderBy(OrderByReleaseDate));
            }
            ActiveSortFunction = "SortByReleaseDate";

        }

        private DateTime? OrderByReleaseDate(TvSeries series)
        {
            if (series.NextSeasonReleaseDate != null)
                return series.NextSeasonReleaseDate;
            if (SortByReleaseDatePackIconKind == PackIconKind.CalendarImport)
            {
                return new DateTime(9999, 10, 10);
            }

            return null;
        }

        private void SortByAddedOrder()
        {
            ActiveSortFunction = "SortByAddedOrder";
            FilteredSeries = new ObservableCollection<TvSeries>(Series);
        }
    }
}