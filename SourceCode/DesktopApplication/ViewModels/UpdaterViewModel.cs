using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using TvSeriesCalendar.ValueConverter;
using TvSeriesCalendar.Views;

namespace TvSeriesCalendar.ViewModels
{
    public class UpdaterViewModel : ObservableObject
    {
        private OnlineDataService _onlineDataService;
        private LocalDataService _localDataService;
        private List<TvSeries> _updatedSeries;
        private List<TvSeries> _todayReleasedSeasonSeries;
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand OpenMainProgramCommand { get; private set; }

        private Window MainWindow;
        private string _newReleaseDateText = "";
        private string _todayReleaseText = "";
        public string NewReleaseDateText { get => _newReleaseDateText; set => OnPropertyChanged(ref _newReleaseDateText, value); }
        public string TodayReleaseText { get => _todayReleaseText; set => OnPropertyChanged(ref _todayReleaseText, value); }


        public UpdaterViewModel(OnlineDataService onlineDataService, LocalDataService localDataService)
        {
            MainWindow = Application.Current.MainWindow;
            MainWindow.Visibility = Visibility.Hidden;
            MainWindow.Loaded += MainWindow_Loaded;
            //TODO: load settings like api key
            _onlineDataService = onlineDataService;
            _localDataService = localDataService;
            CloseWindowCommand = new RelayCommand(CloseWindow);
            OpenMainProgramCommand = new RelayCommand(OpenMainProgram);
            UpdateTvSeries();
        }


        /*
         * Exits the Application
         */
        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }
        private void OpenMainProgram()
        {
            Window current = Application.Current.MainWindow;
            current.Visibility = Visibility.Hidden;
            Application.Current.MainWindow = new MainWindowView();
            Application.Current.MainWindow.DataContext = new MainWindowViewModel(_onlineDataService, _localDataService);
            Application.Current.MainWindow.Show();
            current.Close();

        }

        /*
         * Sets the position of the MainWindow to the bottom right corner
         */
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            MainWindow.Left = desktopWorkingArea.Right - MainWindow.Width - 10;
            MainWindow.Top = desktopWorkingArea.Bottom - MainWindow.Height - 10;
        }

        /*
         * Checks the locally Saved Series for new season releases from the onlineDataService
         */
        private void UpdateTvSeries()
        {
            (_updatedSeries, _todayReleasedSeasonSeries) = TvSeriesUpdater.Update(_localDataService, _onlineDataService);
            if ((_updatedSeries.Count > 0 || _todayReleasedSeasonSeries.Count > 0))
                notify();
            else 
                CloseWindow();
        }

        /*
         * Sets the MainWindow Notification Text and changes the Visibility to visibile
         */
        public void notify()
        {
            _updatedSeries.ForEach(e =>
            {
                NewReleaseDateText += $"{e.Name} Season {e.WatchedSeasons +1 } Releases: { e.NextSeasonReleaseDate.Value:dd.MM.yyyy}\n";
            });

            _todayReleasedSeasonSeries.ForEach(e =>
            {
                TodayReleaseText += $"{e.Name} Season {e.WatchedSeasons + 1 } Releases Today!";
            });
            MainWindow.Visibility = Visibility.Visible;
        }

    }
}
