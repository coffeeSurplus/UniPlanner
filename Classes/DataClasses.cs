using System.IO;

namespace UniPlanner.Classes
{
	public class Timetable
	{
		public required string Title { get; set; }
		public string? Details { get; set; }
		public string? Subject { get; set; }
		public string? Location { get; set; }
		public required int Day { get; set; }
		public required TimeOnly StartTime { get; set; }
		public required TimeOnly EndTime { get; set; }
		public required int Colour { get; set; }

		public void SetValues(string title, string details, string subject, string location, int day, string startTime, string endTime, int colour)
		{
			Title = title.Trim().ToLower();
			Details = !string.IsNullOrWhiteSpace(details) ? details.Trim().ToLower() : null;
			Subject = !string.IsNullOrWhiteSpace(subject) ? subject.Trim().ToLower() : null;
			Location = !string.IsNullOrWhiteSpace(location) ? location.Trim().ToLower() : null;
			Day = day;
			StartTime = TimeOnly.Parse(startTime);
			EndTime = TimeOnly.Parse(endTime);
			Colour = colour;
		}

		public int Length() => (int)(EndTime - StartTime).TotalMinutes;
	}

	public class Task
	{
		public required string Title { get; set; }
		public string? Details { get; set; }
		public string? Subject { get; set; }
		public DateOnly? Date { get; set; }
		public TimeOnly? Time { get; set; }
		public List<Subtask> Subtasks { get; set; } = [];
		public int? Priority { get; set; }
		public bool Completed { get; set; } = false;

		public void SetValues(string title, string details, string subject, string date, string time, int? priority)
		{
			Title = title.Trim().ToLower();
			Details = !string.IsNullOrWhiteSpace(details) ? details.Trim().ToLower() : null;
			Subject = !string.IsNullOrWhiteSpace(subject) ? subject.Trim().ToLower() : null;
			Date = !string.IsNullOrWhiteSpace(date) ? DateOnly.Parse(date) : null;
			Time = !(string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(time)) ? TimeOnly.Parse(time) : null;
			Priority = priority;
		}

		public string DateTimeText() => DateText() != "overdue" ? Time != null ? string.Join(" ", [DateText(), TimeText()]) : DateText() : "overdue";
		public string DateText() => Date != null ? (((DateOnly)Date).DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) switch
		{
			< 0 => "overdue",
			0 => "today",
			1 => "tomorrow",
			_ => $"{(DateOnly)Date:dddd d MMMM}".ToLower()
		} : string.Empty;

		public string TimeText() => Time != null ? $"{(TimeOnly)Time:H:mm}" : string.Empty;
		public string ShortDate() => Date != null ? $"{(DateOnly)Date:d/M}" : string.Empty;
		public string PriorityText() => Priority switch
		{
			1 => "! ",
			2 => "!! ",
			3 => "!!! ",
			_ => string.Empty
		};

		public string SubjectHeader() => Subject ?? "other";
		public string DateHeader() => Date != null ? (((DateOnly)Date).DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) switch
		{
			< 0 => "overdue",
			0 => "today",
			1 => "tomorrow",
			< 7 => "this week",
			_ => "later"
		} : "no date";

		public string PriorityHeader() => Priority switch
		{
			1 => "low priority",
			2 => "medium priority",
			3 => "high priority",
			_ => "no priority"
		};
	}

	public class Subtask
	{
		public required string Title { get; set; }
		public bool Completed { get; set; } = false;

		public void SetValues(string title) => Title = title.Trim().ToLower();
	}

	public class Event
	{
		public required string Title { get; set; }
		public string? Details { get; set; }
		public string? Location { get; set; }
		public required DateOnly Date { get; set; }
		public TimeOnly? StartTime { get; set; }
		public TimeOnly? EndTime { get; set; }
		public required int Colour { get; set; }

		public void SetValues(string title, string details, string location, string date, string startTime, string endTime, int colour)
		{
			Title = title.Trim().ToLower();
			Details = !string.IsNullOrWhiteSpace(details) ? details.Trim().ToLower() : null;
			Location = !string.IsNullOrWhiteSpace(location) ? location.Trim().ToLower() : null;
			Date = DateOnly.Parse(date);
			StartTime = !string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime) ? TimeOnly.Parse(startTime) : null;
			EndTime = !string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime) ? TimeOnly.Parse(endTime) : null;
			Colour = colour;
		}

		public int Length() => (int)(StartTime != null && EndTime != null ? ((TimeSpan)(EndTime - StartTime)).TotalMinutes : 0);
		public int Day() => $"{Date:ddd}" switch
		{
			"Mon" => 1,
			"Tue" => 2,
			"Wed" => 3,
			"Thu" => 4,
			"Fri" => 5,
			"Sat" => 6,
			_ => 7
		};

		public string ShortDateTime() => $"{Date:d/M} • {(StartTime != null && EndTime != null ? $"{(TimeOnly)StartTime:H:mm} - {(TimeOnly)EndTime:H:mm}" : "all day")}";
	}

	public class Link
	{
		public required string Title { get; set; }
		public string? Group { get; set; }
		public string? Subgroup { get; set; }
		public required string Url { get; set; }
		public required bool Favourite { get; set; }

		public void SetValues(string title, string group, string subgroup, string url, bool favourite)
		{
			Title = title.Trim().ToLower();
			Group = !string.IsNullOrWhiteSpace(group) ? group.Trim().ToLower() : null;
			Subgroup = !string.IsNullOrWhiteSpace(subgroup) ? subgroup.Trim().ToLower() : null;
			Url = FormUrl(url.Trim());
			Favourite = favourite;
		}

		public string ReturnUrl()
		{
			if (Directory.Exists(Url) || Uri.IsWellFormedUriString(Url, UriKind.Absolute))
				return Url;
			else if (Url.EndsWith('.'))
				return $"https://{Url}com";
			else if (Url.Contains('.'))
				return $"https://{Url}";
			else
				return $"https://{Url}.com";
		}

		public static string FormUrl(string url)
		{
			if (Directory.Exists(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return url;
			else if (url.EndsWith('.'))
				return $"https://{url}com";
			else if (url.Contains('.'))
				return $"https://{url}";
			else
				return $"https://{url}.com";
		}
	}

	public class Settings
	{
		public bool Startup { get; set; } = true;
		public string Username { get; set; } = string.Empty;
		public bool ScrollBars { get; set; } = true;
		public string AlarmSound { get; set; } = "chimes";
		public string Browser { get; set; } = "msedge";
		public bool PrivateBrowsing { get; set; } = false;
		public string PdfSave { get; set; } = "default";

		public string Arguments() => PrivateBrowsing ? Browser switch
		{
			"msedge" => "-inprivate ",
			"chrome" => "-incognito ",
			"firefox" => "-private-window ",
			"opera" => "--private ",
			_ => string.Empty
		} : string.Empty;
	}
}