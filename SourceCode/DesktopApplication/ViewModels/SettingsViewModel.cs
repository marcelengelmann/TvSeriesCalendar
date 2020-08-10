using System.Diagnostics;
using System.Security.Principal;
using TvSeriesCalendar.UtilityClasses;

namespace TvSeriesCalendar.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly Scheduler _scheduler;
        private readonly bool _startup;
        private bool _enableScheduler;

        public SettingsViewModel()
        {
            _scheduler = new Scheduler("TvSeriesCalendar");
            try
            {
                EnableScheduler = _scheduler.GetTask("RunUpdater").Enabled;
            }
            catch
            {
                EnableScheduler = false;
            }

            _startup = false;
        }

        public bool EnableScheduler
        {
            get => _enableScheduler;
            set
            {
                OnPropertyChanged(ref _enableScheduler, value);
                if (value && _startup == false)
                    CreateOrEditScheduler();
                else if (value == false && _startup == false)
                    DeleteScheduledTask();
            }
        }


        private void CreateOrEditScheduler()
        {
            ProcessModule processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
                _scheduler.AddAutorunTask("RunUpdater", processModule.FileName,
                    WindowsIdentity.GetCurrent().Name, null);
        }

        private void DeleteScheduledTask()
        {
            _scheduler.RemoveTask("RunUpdater");
        }
    }
}