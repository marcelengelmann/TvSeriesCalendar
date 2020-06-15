using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Linq;
using TvSeriesCalendar.Models;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using TvSeriesCalendar.ViewModels;
using System.Windows;
using System;

namespace TvSeriesCalendar.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private object _currentView;
        private SeriesViewModel _seriesVM;
        private SearchViewModel _searchVM;
        private SettingsViewModel _settingsVM;
        private AboutViewModel _aboutVM;
        public ICommand changeViewToSeriesCommand { get; private set; }
        public ICommand changeViewToSearchCommand { get; private set; }
        public ICommand changeViewToSettingsCommand { get; private set; }
        public ICommand changeViewToAboutCommand { get; private set; }
        public ICommand closeApplicationCommand { get; private set; }

        public object CurrentView
        {
            get { return _currentView; }
            set { OnPropertyChanged(ref _currentView, value); }
        }
        
        public SeriesViewModel SeriesVM
        {
            get { return _seriesVM; }
            set { OnPropertyChanged(ref _seriesVM, value); }
        }

        public SearchViewModel SearchVM
        {
            get { return _searchVM; }
            set { OnPropertyChanged(ref _searchVM, value); }
        }

        public SettingsViewModel SettingsVM
        {
            get { return _settingsVM; }
            set { OnPropertyChanged(ref _settingsVM, value); }
        }

        public AboutViewModel AboutVM
        {
            get { return _aboutVM; }
            set { OnPropertyChanged(ref _aboutVM, value); }
        }


        public void changeViewToSeries()
        {
            if (CurrentView != SeriesVM) { 
                CurrentView = SeriesVM;
            }
        }
        public void changeViewToSearch()
        {
            if (CurrentView != SearchVM)
                CurrentView = SearchVM;
        }
        public void changeViewToSettings()
        {
            if (CurrentView != SettingsVM)
                CurrentView = SettingsVM;
        }
        public void changeViewToAbout()
        {
            if (CurrentView != AboutVM)
                CurrentView = AboutVM;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            IEnumerable<string> seriesImages = new List<TvSeries>(SeriesVM.Series).Select(x => x.ImagePath);
            string[] allImages = Directory.GetFiles("images/");
            for(int i = 0; i < allImages.Length; i++)
            {
                if (!seriesImages.Contains(allImages[i]))
                {
                    File.Delete(allImages[i]);
                }
            }
        }

        public MainWindowViewModel(OnlineDataService onlineDataService, LocalDataService localDataService)
        {
            SeriesVM = new SeriesViewModel(localDataService, onlineDataService);
            SearchVM = new SearchViewModel(localDataService, onlineDataService);
            SettingsVM = new SettingsViewModel();
            AboutVM = new AboutViewModel();
            changeViewToSeriesCommand = new RelayCommand(changeViewToSeries);
            changeViewToSearchCommand = new RelayCommand(changeViewToSearch);
            changeViewToSettingsCommand = new RelayCommand(changeViewToSettings);
            changeViewToAboutCommand = new RelayCommand(changeViewToAbout);
            Application.Current.MainWindow.Closed += MainWindow_Closed;
            CurrentView = SeriesVM;
        }

    }
}