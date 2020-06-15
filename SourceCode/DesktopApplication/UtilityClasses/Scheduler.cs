using System;
using TaskScheduler;

namespace TvSeriesCalendar.UtilityClasses
{
    class Scheduler
    {
		private ITaskFolder folder;
		private TaskScheduler.TaskScheduler taskService;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Path">Der vollständige Pfad innerhalb des Aufgabenplaners, unter dem die Aufgaben bearbeitet werden.</param>
		public Scheduler(String Path)
		{
			taskService = new TaskScheduler.TaskScheduler();
			taskService.Connect();
			folder = CreatePath(Path);
			if (folder == null)
				throw new Exception("Folder access denied!");  // Konstruktor Fehler :-/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Path">Vollständiger Pfad wie "\diub\Test"</param>
		/// <returns></returns>
		public ITaskFolder CreatePath(String Path)
		{
			int i;
			String path;
			String[] parts;
			ITaskFolder folder;

			path = Path.Replace('/', '\\').Trim('\\');
			parts = path.Split('\\');
			path = "";
			folder = taskService.GetFolder("\\");
			for (i = 0; i < parts.Length; i++)
			{
				try
				{
					folder.CreateFolder(parts[i]);
				}
				catch (Exception) { } // bei jedem bereits existierenden Pfad, daher einfach ignorieren
				path += "\\" + parts[i];
				try
				{
					folder = taskService.GetFolder(path);
				}
				catch (Exception)
				{
					return null;    // das sollte nicht vorkommen: NULL als Fehler
				}
			}
			return folder;
		}

		public bool TaskExists(String Name)
		{
			return GetTask(Name) != null;
		}

		public IRegisteredTask GetTask(String Name)
		{
			IRegisteredTask task;

			try
			{
				task = folder.GetTask(Name);
				return task;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public bool RemoveTask(String Name)
		{
			try
			{
				folder.DeleteTask(Name, 0);    // Flags not supported - also egal
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="PathFilename"></param>
		/// <param name="Username">Darf Null sein für den gerade aktiven Benutzer</param>
		/// <param name="Password">Darf Null sein für den gerade aktiven Benutzer</param>
		public void AddAutorunTask(String Name, String PathFilename, String Username, String Password)
		{
			ITaskDefinition definition;
			ITrigger trigger;
			IActionCollection actions;
			IAction action;
			IExecAction exec_action;
			IRegisteredTask registerd_task;

			definition = taskService.NewTask(0);
			definition.Settings.Enabled = true;
			// taskDefinition.Settings.Priority  // :: die Priorität der Aufgabe, NICHT die Priorität des erzeugten Prozesses
			definition.Settings.Compatibility = _TASK_COMPATIBILITY.TASK_COMPATIBILITY_V2_1;
			definition.Settings.StopIfGoingOnBatteries = false;
			definition.Settings.DisallowStartIfOnBatteries = false;

			// Rechte des Processes
			definition.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_LUA;
			definition.Principal.LogonType = _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN;

			//create trigger for task creation.
			trigger = definition.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
			(trigger as ILogonTrigger).UserId = taskService.ConnectedUser;
			(trigger as ILogonTrigger).Delay = "PT5M"; // 5 min
			//_trigger.StartBoundary = DateTime.Now.AddSeconds (15).ToString ("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
			//_trigger.EndBoundary = DateTime.Now.AddMinutes (1).ToString ("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
			trigger.Enabled = true;
			actions = definition.Actions;
			action = actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
			exec_action = action as IExecAction;
			exec_action.Path = "\"" + PathFilename + "\"";     // tatsächlich PathFilename in Hochkommata, Bezeichner "Path" ist schlicht falsch
			exec_action.WorkingDirectory = System.IO.Path.GetDirectoryName(PathFilename); // darf nicht in Hochkommata eingeschlossen werden
			exec_action.Arguments = "update";

			// das Toklen muss(!) mit dem der Aufgabe oben übereinstimmen: _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN
			registerd_task = folder.RegisterTaskDefinition(Name, definition, 6, Username, Password, _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, null);
		}
	}
}
