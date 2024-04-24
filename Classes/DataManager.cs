using System.IO;
using System.Text.Json;

namespace UniPlanner.Classes
{
	public class DataManager
	{
		public List<Timetable> TimetableList { get; set; }
		public List<Task> TaskList { get; set; }
		public List<Event> EventList { get; set; }
		public List<Link> LinkList { get; set; }
		public Settings Settings { get; set; }

		private const string timetablePath = @"TimetableList.json";
		private const string taskPath = @"TaskList.json";
		private const string eventPath = @"EventList.json";
		private const string linkPath = @"LinkList.json";
		private const string settingsPath = @"Settings.json";

		public DataManager()
		{
			Directory.CreateDirectory(GetPath(string.Empty));
			TimetableList = GetTimetableList();
			TaskList = GetTaskList();
			EventList = GetEventList();
			LinkList = GetLinkList();
			Settings = GetSettings();
		}

		private static List<T> GetList<T>(string path)
		{
			if (!File.Exists(GetPath(path)))
				WriteFile(path, new List<T>());

			return JsonSerializer.Deserialize<List<T>>(ReadFile(path))!;
		}

		private static List<Timetable> GetTimetableList() => GetList<Timetable>(timetablePath);
		private static List<Task> GetTaskList() => GetList<Task>(taskPath);
		private static List<Event> GetEventList() => GetList<Event>(eventPath);
		private static List<Link> GetLinkList() => GetList<Link>(linkPath);
		private static Settings GetSettings()
		{
			if (!File.Exists(GetPath(settingsPath)))
				WriteFile(settingsPath, new Settings());

			return JsonSerializer.Deserialize<Settings>(ReadFile(settingsPath))!;
		}

		public void UpdateTimetableList() => WriteFile(timetablePath, TimetableList);
		public void UpdateTaskList() => WriteFile(taskPath, TaskList);
		public void UpdateEventList() => WriteFile(eventPath, EventList);
		public void UpdateLinkList() => WriteFile(linkPath, LinkList);
		public void UpdateSettings() => WriteFile(settingsPath, Settings);

		private static string GetPath(string path) => Environment.ExpandEnvironmentVariables($@"%AppData%\UniPlanner\{path}");
		private static string ReadFile(string path)
		{
			bool successful = false;
			string output = string.Empty;

			do
			{
				try
				{
					output = File.ReadAllText(GetPath(path));
					successful = true;
				}
				catch { }

			} while (!successful);

			return output;
		}
		private static void WriteFile(string path, object data)
		{
			bool updated = false;

			do
			{
				try
				{
					File.WriteAllText(GetPath(path), JsonSerializer.Serialize(data));
					updated = true;
				}
				catch { }

			} while (!updated);
		}
	}
}