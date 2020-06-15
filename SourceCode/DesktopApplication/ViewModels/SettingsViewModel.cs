using System;
using System.Diagnostics;
using System.Windows;
using TaskScheduler;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private bool _enableScheduler;
        Scheduler scheduler;
        private bool startup = true;
        public SettingsViewModel()
        {
            scheduler = new Scheduler("TvSeriesCalendar");
            if (scheduler.GetTask("RunUpdater") != null)
                EnableScheduler = scheduler.GetTask("RunUpdater").Enabled;
            else
                EnableScheduler = false;
            startup = false;
        }

        public bool EnableScheduler
        {
            get => _enableScheduler;
            set
            {
                OnPropertyChanged(ref _enableScheduler, value);
                if (value == true && startup == false)
                    CreateOrEditScheduler();
                else if (value == false && startup == false)
                    deleteScheduledTask();
            }
        }


        private void CreateOrEditScheduler()
        {
            scheduler.AddAutorunTask("RunUpdater", Process.GetCurrentProcess().MainModule.FileName, System.Security.Principal.WindowsIdentity.GetCurrent().Name, null);
        }
        private void deleteScheduledTask()
        {
            scheduler.RemoveTask("RunUpdater");
        }
    }
}