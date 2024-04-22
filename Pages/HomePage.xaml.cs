using System.Windows.Controls;
using UniPlanner.Classes;
using UniPlanner.UserControls.HomeControls;

namespace UniPlanner.Pages
{
	public partial class HomePage : Page
	{
		public required DataManager DataManager { get; init; }

		public HomePage() => InitializeComponent();
		public HomePage SetDisplay()
		{
			UpdateGreetingView();
			UpdateTimetableView();
			UpdateTaskView();
			UpdateEventView();
			UpdateLinkView();
			return this;
		}

		public void UpdateGreetingView()
		{
			Date.Text = $"{DateTime.Now:dddd d MMMM}".ToLower();

			WelcomeMessage.Text = $"{$"good {DateTime.Now.Hour switch
			{
				>= 6 and < 12 => "morning",
				>= 12 and < 18 => "afternoon",
				_ => "evening"
			}}"}{(DataManager.Settings.Username != string.Empty ? ", " : string.Empty)}{DataManager.Settings.Username}".ToLower();
		}

		private void UpdateTimetableView()
		{
			TimetableArea.Children.RemoveRange(0, TimetableArea.Children.Count);
			int TimetableCount = DataManager.TimetableList.Count(x => x.Day == $"{DateTime.Now:ddd}" switch
			{
				"Mon" => 1,
				"Tue" => 2,
				"Wed" => 3,
				"Thu" => 4,
				"Fri" => 5,
				_ => 0
			});

			TimetableHeader.Text = TimetableCount != 0
				? $"{TimetableCount} timetabled {(TimetableCount == 1 ? "event" : "events")} today"
				: "no timetabled events today";

			foreach (Timetable timetable in DataManager.TimetableList.Where(x => x.Day == $"{DateTime.Now:ddd}" switch
			{
				"Mon" => 1,
				"Tue" => 2,
				"Wed" => 3,
				"Thu" => 4,
				"Fri" => 5,
				_ => 0
			}).OrderBy(x => x.StartTime))
				TimetableArea.Children.Add(new HomePageTimetableViewer() { Timetable = timetable }.SetDisplay());
		}
		private void UpdateTaskView()
		{
			TaskArea.Children.RemoveRange(0, TaskArea.Children.Count);
			int taskCount = DataManager.TaskList.Count(x => !x.Completed || x.Subtasks.Any(x => !x.Completed));
			TaskHeader.Text = taskCount != 0 ? $"{taskCount} incomplete {(taskCount == 1 ? "task" : "tasks")}" : "no tasks (hooray!)";

			foreach (Classes.Task task in DataManager.TaskList
				.Where(x => !x.Completed || x.Subtasks.Any(x => !x.Completed))
				.OrderBy(x => x.Subject == null)
				.ThenBy(x => x.Subject)
				.ThenBy(x => x.Title))
				TaskArea.Children.Add(new HomePageTaskViewer() { Task = task }.SetDisplay());
		}
		private void UpdateEventView()
		{
			EventArea.Children.RemoveRange(0, EventArea.Children.Count);
			int eventCount = DataManager.EventList.Count(x => x.Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber is >= 0 and < 7);
			EventHeader.Text = eventCount != 0 ? $"{eventCount} {(eventCount == 1 ? "event" : "events")} this week" : "no events this week";

			foreach (Event @event in DataManager.EventList
				.Where(x => x.Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber is >= 0 and < 7)
				.OrderBy(x => x.Date)
				.ThenBy(x => x.StartTime))
				EventArea.Children.Add(new HomePageEventViewer() { Event = @event }.SetDisplay());
		}
		private void UpdateLinkView()
		{
			LinkArea.Children.RemoveRange(0, LinkArea.Children.Count);
			int LinkCount = DataManager.LinkList.Count(x => x.Favourite);

			foreach (Link link in DataManager.LinkList
				.Where(x => x.Favourite)
				.OrderBy(x => x.Group == null)
				.ThenBy(x => x.Group)
				.ThenBy(x => x.Subgroup)
				.ThenBy(x => x.Title))
				LinkArea.Children.Add(new HomePageLinkViewer() { DataManager = DataManager, Link = link }.SetDisplay());
		}
	}
}