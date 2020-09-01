using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using TvSeriesCalendar.Views;

namespace TvSeriesCalendar.ViewModels
{
    public class UpdaterViewModel : ObservableObject
    {
        private readonly Window _mainWindow;
        private readonly SeriesLocalDataService _seriesLocalDataService;
        private readonly SeriesOnlineDataService _seriesOnlineDataService;
        private string _newReleaseDateText = "";
        private List<TvSeries> _todayReleasedSeasonSeries;
        private string _todayReleaseText = "";
        private List<TvSeries> _updatedSeries;


        public UpdaterViewModel(SeriesOnlineDataService onlineDataService, SeriesLocalDataService localDataService)
        {
            _mainWindow = Application.Current.MainWindow;
            if (_mainWindow != null)
            {
                _mainWindow.Visibility = Visibility.Hidden;
                _mainWindow.Loaded += MainWindow_Loaded;
            }

            _seriesOnlineDataService = onlineDataService;
            _seriesLocalDataService = localDataService;
            CloseWindowCommand = new RelayCommand(CloseWindow);
            OpenMainProgramCommand = new RelayCommand(OpenMainProgram);
            UpdateTvSeries();
        }

        public ICommand CloseWindowCommand { get; }
        public ICommand OpenMainProgramCommand { get; }

        public string NewReleaseDateText
        {
            get => _newReleaseDateText;
            set => OnPropertyChanged(ref _newReleaseDateText, value);
        }

        public string TodayReleaseText
        {
            get => _todayReleaseText;
            set => OnPropertyChanged(ref _todayReleaseText, value);
        }


        /*
         * Exit the Application
         */
        private static void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        private void OpenMainProgram()
        {
            Window current = Application.Current.MainWindow;
            if (current != null)
                current.Visibility = Visibility.Hidden;
            Application.Current.MainWindow = new MainWindowView();
            Application.Current.MainWindow.DataContext =
                new MainWindowViewModel(_seriesOnlineDataService, _seriesLocalDataService);
            Application.Current.MainWindow.Show();
            current?.Close();
        }

        /*
         * Sets the position of the MainWindow to the bottom right corner
         */
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            _mainWindow.Left = desktopWorkingArea.Right - _mainWindow.Width - 10;
            _mainWindow.Top = desktopWorkingArea.Bottom - _mainWindow.Height - 10;
        }

        /*
         * Checks the locally Saved Series for new season releases from the onlineDataService
         */
        private async Task UpdateTvSeries()
        {
            (_updatedSeries, _todayReleasedSeasonSeries) =
                await TvSeriesUpdater.Update(_seriesLocalDataService, _seriesOnlineDataService, null);
            if (_updatedSeries.Count > 0 || _todayReleasedSeasonSeries.Count > 0)
                Notify();
            else
                CloseWindow();
        }

        /*
         * Sets the MainWindow Notification Text and changes the Visibility to visible
         */
        public void Notify()
        {
            _updatedSeries.ForEach(e =>
            {
                if (e.NextSeasonReleaseDate != null)
                    NewReleaseDateText +=
                        $"{e.Name} Season {e.WatchedSeasons + 1} Releases: {e.NextSeasonReleaseDate.Value:dd.MM.yyyy}\n";
            });

            _todayReleasedSeasonSeries.ForEach(e =>
            {
                TodayReleaseText += $"{e.Name} Season {e.WatchedSeasons + 1} Releases Today!";
            });
            _mainWindow.Visibility = Visibility.Visible;
        }
    }
}