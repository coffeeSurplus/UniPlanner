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

		private const string timetablePath = @"data\timetablelist.json";
		private const string taskPath = @"data\tasklist.json";
		private const string eventPath = @"data\eventlist.json";
		private const string linkPath = @"data\linklist.json";
		private const string settingsPath = @"data\settings.json";

		public DataManager()
		{
			Directory.CreateDirectory(@"data");
			TimetableList = GetTimetableList();
			TaskList = GetTaskList();
			EventList = GetEventList();
			LinkList = GetLinkList();
			Settings = GetSettings();
		}

		private static List<Timetable> GetTimetableList()
		{
			if (!File.Exists(timetablePath))
				WriteFile(timetablePath, new List<Timetable>());
			return JsonSerializer.Deserialize<List<Timetable>>(ReadFile(timetablePath))!;
		}
		private static List<Task> GetTaskList()
		{
			if (!File.Exists(taskPath))
				WriteFile(taskPath, new List<Task>());
			return JsonSerializer.Deserialize<List<Task>>(ReadFile(taskPath))!;
		}
		private static List<Event> GetEventList()
		{
			if (!File.Exists(eventPath))
				WriteFile(eventPath, new List<Event>());
			return JsonSerializer.Deserialize<List<Event>>(ReadFile(eventPath))!;
		}
		private static List<Link> GetLinkList()
		{
			if (!File.Exists(linkPath))
				WriteFile(linkPath, new List<Link>());
			return JsonSerializer.Deserialize<List<Link>>(ReadFile(linkPath))!;
		}
		private static Settings GetSettings()
		{
			if (!File.Exists(settingsPath))
				WriteFile(settingsPath, new Settings());
			return JsonSerializer.Deserialize<Settings>(ReadFile(settingsPath))!;
		}

		public void UpdateTimetableList() => WriteFile(timetablePath, TimetableList);
		public void UpdateTaskList() => WriteFile(taskPath, TaskList);
		public void UpdateEventList() => WriteFile(eventPath, EventList);
		public void UpdateLinkList() => WriteFile(linkPath, LinkList);
		public void UpdateSettings() => WriteFile(settingsPath, Settings);

		private static string ReadFile(string path)
		{
			bool successful = false;
			string output = string.Empty;
			do
			{
				try
				{
					output = File.ReadAllText(path);
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
					File.WriteAllText(path, JsonSerializer.Serialize(data));
					updated = true;
				}
				catch { }
			} while (!updated);
		}
	}
}