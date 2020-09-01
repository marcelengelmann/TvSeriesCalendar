using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private AboutViewModel _aboutVm;
        private object _currentView;
        private SearchViewModel _searchVm;
        private SeriesViewModel _seriesVm;
        private SettingsViewModel _settingsVm;
        private PackIconKind _maximizeRestorePackIcon = PackIconKind.WindowMaximize;

        public MainWindowViewModel(SeriesOnlineDataService onlineDataService, SeriesLocalDataService localDataService)
        {
            SeriesVm = new SeriesViewModel(localDataService, onlineDataService);
            SearchVm = new SearchViewModel(localDataService, onlineDataService);
            SettingsVm = new SettingsViewModel();
            AboutVm = new AboutViewModel();
            ChangeViewToSeriesCommand = new RelayCommand(ChangeViewToSeries);
            ChangeViewToSearchCommand = new RelayCommand(ChangeViewToSearch);
            ChangeViewToSettingsCommand = new RelayCommand(ChangeViewToSettings);
            ChangeViewToAboutCommand = new RelayCommand(ChangeViewToAbout);
            CloseApplicationCommand = new RelayCommand(CloseApplication);
            MinimizeApplicationCommand = new RelayCommand(MinimizeApplication);
            MaximizeApplicationCommand = new RelayCommand(MaximizeRestoreApplication);
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Closed += MainWindow_Closed;
            CurrentView = SeriesVm;
        }

        public ICommand ChangeViewToSeriesCommand { get; }
        public ICommand ChangeViewToSearchCommand { get; }
        public ICommand ChangeViewToSettingsCommand { get; }
        public ICommand ChangeViewToAboutCommand { get; }
        public ICommand CloseApplicationCommand { get; }
        public ICommand MinimizeApplicationCommand { get; }
        public ICommand MaximizeApplicationCommand { get; }

        public object CurrentView
        {
            get => _currentView;
            set => OnPropertyChanged(ref _currentView, value);
        }

        public SeriesViewModel SeriesVm
        {
            get => _seriesVm;
            set => OnPropertyChanged(ref _seriesVm, value);
        }

        public SearchViewModel SearchVm
        {
            get => _searchVm;
            set => OnPropertyChanged(ref _searchVm, value);
        }

        public SettingsViewModel SettingsVm
        {
            get => _settingsVm;
            set => OnPropertyChanged(ref _settingsVm, value);
        }

        public AboutViewModel AboutVm
        {
            get => _aboutVm;
            set => OnPropertyChanged(ref _aboutVm, value);
        }

        public PackIconKind MaximizeRestorePackIcon
        {
            get => _maximizeRestorePackIcon;
            set => OnPropertyChanged(ref _maximizeRestorePackIcon, value);
        }


        public void ChangeViewToSeries()
        {
            if (CurrentView != SeriesVm) CurrentView = SeriesVm;
        }

        public void ChangeViewToSearch()
        {
            if (CurrentView != SearchVm)
                CurrentView = SearchVm;
        }

        public void ChangeViewToSettings()
        {
            if (CurrentView != SettingsVm)
                CurrentView = SettingsVm;
        }

        public void ChangeViewToAbout()
        {
            if (CurrentView != AboutVm)
                CurrentView = AboutVm;
        }

        private void CloseApplication()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Shutdown();
        }
        
        private void MaximizeRestoreApplication()
        {
            if (Application.Current.MainWindow != null){
                if(Application.Current.MainWindow.WindowState == WindowState.Maximized){
                    MaximizeRestorePackIcon = PackIconKind.WindowMaximize;
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    MaximizeRestorePackIcon = PackIconKind.WindowRestore;
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                }
            }
        }
        
        private void MinimizeApplication()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            IEnumerable<string> seriesImages = new List<TvSeries>(SeriesVm.Series).Select(x => x.ImagePath);
            string[] allImages = Directory.GetFiles("images/");
            foreach (string t in allImages)
            {
                if (seriesImages.Contains(t) == false)
                {
                    File.Delete(t);
                }
            }

            if(File.Exists("error.txt"))
                File.Delete("error.txt");
        }
    }
}