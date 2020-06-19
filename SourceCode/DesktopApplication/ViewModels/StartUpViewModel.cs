using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TvSeriesCalendar.Services;
using TvSeriesCalendar.UtilityClasses;
using TvSeriesCalendar.Views;

namespace TvSeriesCalendar.ViewModels
{
    public class StartUpViewModel : ObservableObject
    {
        private Window CurrentMainWindow;
        private string NextMainWindow;
        OnlineDataService _onlineDataService;
        LocalDataService _localDataService;
        private string _statusText;
        private int _downloadProgress = 0;




        public StartUpViewModel()
        {
            CurrentMainWindow = Application.Current.MainWindow;
            CurrentMainWindow.Visibility = Visibility.Hidden;
            _onlineDataService = new OnlineDataService();
            _localDataService = new LocalDataService();
            string[] args = Environment.GetCommandLineArgs();

            if(args.Length == 2)
            {
                if (args[1] == "update")
                {
                    NextMainWindow = "updater";
                }
                else
                    throw new ArgumentException("Invalid Argument: " + args[1]);
            }
            else if(args.Length == 1)
            {
                CurrentMainWindow.Visibility = Visibility.Visible;
                NextMainWindow = "main";
            }
            else
            {
                throw new ArgumentException("Invalid number of Arguments!");
            }
            Task.Run(Updating).ContinueWith(x =>
                {
                    RunOnUiThread(ConfigLoaded);
                });
        }

        public string StatusText
        {
            get { return _statusText; }
            set { OnPropertyChanged(ref _statusText, value); }
        }
        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set { OnPropertyChanged(ref _downloadProgress, value); }
        }

        public async Task Updating()
        {
            InitializeRequiredData();
            if (NextMainWindow == "main")
            {
                string version = ""; //ApplicationUpdater.NewVersionExists();
                if (version == "")
                {
                            Console.WriteLine("test");
                    StatusText = "Downloading Updates";
                    if(!await ApplicationUpdater.Update(version, progressUpdate))
                    {
                        //TODO: show user error
                        /*RunOnUiThread(() =>
                        {
                            throw new Exception("The Program has not been installed correctly!");
                        });*/

                    }
                    RunOnUiThread(CurrentMainWindow.Close);
                    RunOnUiThread(Application.Current.Shutdown);
                }
                else
                {
                    StatusText = "Updating Tv Series";
                    TvSeriesUpdater.Update(_localDataService, _onlineDataService);
                }
            }
            await _onlineDataService.FetchTMDbConfig();
        }

        public void ConfigLoaded()
        {
            CurrentMainWindow = Application.Current.MainWindow;
            if (NextMainWindow == "updater")
            {
                Application.Current.MainWindow = new UpdaterView();
                Application.Current.MainWindow.DataContext = new UpdaterViewModel(_onlineDataService, _localDataService);
            }
            else //main
            {

                Application.Current.MainWindow = new MainWindowView();
                Application.Current.MainWindow.DataContext = new MainWindowViewModel(_onlineDataService, _localDataService);
                Application.Current.MainWindow.Show();
            }
            CurrentMainWindow.Close();
        }
        private void InitializeRequiredData()
        {
            try
            {
                if (Directory.Exists("update")) Directory.Delete("update", true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Directory.CreateDirectory("settings");
            Directory.CreateDirectory("images");
            Directory.CreateDirectory("savedData");
        }

        private void RunOnUiThread(Action task)
        {
            Application.Current.Dispatcher.Invoke(new Action(task));
        }

        private void progressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress = e.ProgressPercentage;
        }
    }
}
