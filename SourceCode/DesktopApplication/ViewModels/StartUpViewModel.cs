using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using TvSeriesCalendar.Views;

namespace TvSeriesCalendar.ViewModels
{
    public class StartUpViewModel : ObservableObject
    {
        private readonly SeriesLocalDataService _localDataService;
        private readonly string _nextMainWindow;
        private readonly SeriesOnlineDataService _onlineDataService;
        private Window _currentMainWindow;
        private int _downloadProgress;
        private string _statusText;


        public StartUpViewModel()
        {
            _currentMainWindow = Application.Current.MainWindow;
            if (_currentMainWindow != null)
                _currentMainWindow.Visibility = Visibility.Hidden;
            _onlineDataService = new SeriesOnlineDataService();
            _localDataService = new SeriesLocalDataService();
            string[] args = Environment.GetCommandLineArgs();

            switch (args.Length)
            {
                case 2 when args[1] == "update":
                    _nextMainWindow = "updater";
                    break;
                case 2 when args[1] == "showChangelog":
                    if (_currentMainWindow != null)
                        _currentMainWindow.Visibility = Visibility.Visible;
                    _nextMainWindow = "main";
                    ChangelogWindowView changelogWindow = new ChangelogWindowView
                    {
                        DataContext = new ChangelogWindowViewModel(),
                    };
                    changelogWindow.Topmost = true;
                    changelogWindow.Show();
                    break;
                case 2:
                    throw new ArgumentException("Invalid Argument: " + args[1]);
                case 1:
                    if (_currentMainWindow != null)
                        _currentMainWindow.Visibility = Visibility.Visible;
                    _nextMainWindow = "main";
                    break;
                default:
                    throw new ArgumentException("Invalid number of Arguments!");
            }

            Task.Run(Updating).ContinueWith(e => Application.Current.Dispatcher.Invoke(ConfigLoaded));
        }

        public string StatusText
        {
            get => _statusText;
            set => OnPropertyChanged(ref _statusText, value);
        }

        public int DownloadProgress
        {
            get => _downloadProgress;
            set => OnPropertyChanged(ref _downloadProgress, value);
        }

        public async Task Updating()
        {
            InitializeRequiredData();
            if (_nextMainWindow == "main")
            {
                ApplicationUpdater.Assets[] assets = await ApplicationUpdater.NewVersionExists();
                if (assets != null)
                {
                    StatusText = "Downloading Updates";
                    try
                    {
                        await ApplicationUpdater.Update(assets, DownloadProgressUpdate);
                        ShutdownApplication();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An Error occured during the update process");
                    }
                }
                else
                {
                    StatusText = "Updating Tv Series";
                    await _onlineDataService.FetchTMDbConfig();
                    await TvSeriesUpdater.Update(_localDataService, _onlineDataService, SeriesUpdateProgress);
                }
            }
        }

        private void ConfigLoaded()
        {
            _currentMainWindow = Application.Current.MainWindow;
            if (_nextMainWindow == "updater")
            {
                Application.Current.MainWindow = new UpdaterView
                {
                    DataContext = new UpdaterViewModel(_onlineDataService, _localDataService)
                };
            }
            else //main
            {
                Application.Current.MainWindow = new MainWindowView();
                Application.Current.MainWindow.DataContext =
                    new MainWindowViewModel(_onlineDataService, _localDataService);
                Application.Current.MainWindow.Show();
            }

            _currentMainWindow?.Close();
        }

        private static void InitializeRequiredData()
        {
            try
            {
                if (Directory.Exists("update")) Directory.Delete("update", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Directory.CreateDirectory("settings");
            Directory.CreateDirectory("images");
            Directory.CreateDirectory("savedData");
        }

        private static void ShutdownApplication()
        {
            Application.Current.Dispatcher.BeginInvoke((Action) delegate { Application.Current.Shutdown(); });
        }

        private void DownloadProgressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress = e.ProgressPercentage;
        }

        private void SeriesUpdateProgress(int progressPercentage)
        {
            DownloadProgress = progressPercentage;
        }
    }
}