﻿using System;
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
        private readonly SeriesLocalDataService _seriesLocalDataService;
        private readonly string _nextMainWindow;
        private readonly SeriesOnlineDataService _seriesOnlineDataService;
        private Window _currentMainWindow;
        private int _downloadProgress;
        private string _statusText;


        public StartUpViewModel()
        {
            _currentMainWindow = Application.Current.MainWindow;
            if (_currentMainWindow != null)
                _currentMainWindow.Visibility = Visibility.Hidden;
            _seriesOnlineDataService = new SeriesOnlineDataService();
            _seriesLocalDataService = new SeriesLocalDataService();
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

            Task.Run(Updating).ContinueWith(e =>
            {
                if(e.Result)
                    Application.Current.Dispatcher.Invoke(ConfigLoaded);
            });
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

        public async Task<bool> Updating()
        {
            InitializeRequiredData();
            if (_nextMainWindow != "main") return true;
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
                    Logger.Exception(ex, "StartUpViewModel.Updating");
                    MessageBox.Show("An Error occured during the update process");
                }
                return false;
            }
            StatusText = "Updating Tv Series";
            await _seriesOnlineDataService.FetchTMDbConfig();
            await TvSeriesUpdater.Update(_seriesLocalDataService, _seriesOnlineDataService, SeriesUpdateProgress);
            return true;
        }

        private void ConfigLoaded()
        {
            _currentMainWindow = Application.Current.MainWindow;
            if (_nextMainWindow == "updater")
            {
                Application.Current.MainWindow = new UpdaterView
                {
                    DataContext = new UpdaterViewModel(_seriesOnlineDataService, _seriesLocalDataService)
                };
            }
            else //main
            {
                Application.Current.MainWindow = new MainWindowView();
                Application.Current.MainWindow.DataContext =
                    new MainWindowViewModel(_seriesOnlineDataService, _seriesLocalDataService);
                Application.Current.MainWindow.Show();
            }

            _currentMainWindow?.Close();
        }

        private static void InitializeRequiredData()
        {
            try
            {
                if (Directory.Exists("update")) Directory.Delete("update", true);
                if (File.Exists("update.vbs"))  File.Delete("update.vbs");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "StartUpViewModel.InitializeRequiredData");
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