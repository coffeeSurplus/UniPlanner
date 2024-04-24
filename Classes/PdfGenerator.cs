using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace UniPlanner.Classes
{
	static class TaskGenerator
	{
		public static byte[] GeneratePdf(List<Task> taskList, int groupMode, bool ascending, bool includeCompleted)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			taskList = SortTaskList(taskList, groupMode, ascending, includeCompleted);

			return
			Document.Create(x =>
			{
				x.Page(x =>
				{
					x.SetPage();

					x.Content().Column(x =>
					{
						x.AddTitle();

						for (int taskIndex = 0; taskIndex < taskList.Count; taskIndex++)
						{
							string header = groupMode switch
							{
								1 => taskList[taskIndex].SubjectHeader(),
								2 => taskList[taskIndex].DateHeader(),
								_ => taskList[taskIndex].PriorityHeader(),
							};

							if (taskIndex == 0 || header != groupMode switch
							{
								1 => taskList[taskIndex - 1].SubjectHeader(),
								2 => taskList[taskIndex - 1].DateHeader(),
								_ => taskList[taskIndex - 1].PriorityHeader()
							})
								x.AddHeader(taskList, groupMode, header);

							x.Item().Decoration(x =>
							{
								x.AddTask(taskList[taskIndex]);

								if (taskList[taskIndex].Subtasks.Count != 0)
									x.AddSubtasks(taskList[taskIndex]);
							});
						}
					});
				});
			}).GeneratePdf();
		}

		private static List<Task> SortTaskList(List<Task> taskList, int groupMode, bool ascending, bool includeCompleted)
		{
			taskList = groupMode switch
			{
				1 => OrderBySubject(taskList, ascending),
				2 => OrderByDate(taskList, ascending),
				_ => OrderByPriority(taskList, ascending)
			};

			taskList = OrderSubtasks(taskList);

			if (!includeCompleted)
				RemoveCompleted(taskList);

			return taskList;
		}
		private static List<Task> OrderBySubject(List<Task> taskList, bool ascending)
		{
			return taskList = ascending

				? ([.. taskList
					.OrderBy(x => x.Subject == null)
					.ThenBy(x => x.Subject)
					.ThenBy(x => x.Completed)
					.ThenBy(x => x.Title)])

				: ([.. taskList
					.OrderBy(x => x.Subject == null)
					.ThenByDescending(x => x.Subject)
					.ThenBy(x => x.Completed)
					.ThenByDescending(x => x.Title)]);
		}
		private static List<Task> OrderByDate(List<Task> taskList, bool ascending)
		{
			return taskList = ascending

				? ([.. taskList
					.OrderBy(x => x.Date == null)
					.ThenBy(x => x.Date)
					.ThenBy(x => x.Completed)
					.ThenBy(x => x.Time != null)
					.ThenBy(x => x.Time)
					.ThenBy(x => x.Title)])

				: ([.. taskList
					.OrderBy(x => x.Date == null)
					.ThenByDescending(x => x.Date)
					.ThenBy(x => x.Completed)
					.ThenBy(x => x.Time != null)
					.ThenBy(x => x.Time)
					.ThenByDescending(x => x.Title)]);
		}
		private static List<Task> OrderByPriority(List<Task> taskList, bool ascending)
		{
			return taskList = ascending

				? [.. taskList
					.OrderBy(x => x.Priority == null)
					.ThenByDescending(x => x.Priority)
					.ThenBy(x => x.Completed)
					.ThenBy(x => x.Title)]

				: [.. taskList
					.OrderByDescending(x => x.Priority == null)
					.ThenBy(x => x.Priority)
					.ThenBy(x => x.Completed)
					.ThenByDescending(x => x.Title)];
		}
		private static List<Task> OrderSubtasks(List<Task> taskList)
		{
			foreach (Task task in taskList)
				task.Subtasks = [.. task.Subtasks.OrderBy(x => x.Completed).ThenBy(x => x.Title)];

			return taskList;
		}
		private static List<Task> RemoveCompleted(List<Task> taskList)
		{
			foreach (Task task in taskList)
				task.Subtasks.RemoveAll(x => x.Completed);

			taskList.RemoveAll(x => x.Completed && x.Subtasks.Count == 0);
			return taskList;
		}

		private static int ReturnTaskCount(List<Task> taskList, int groupMode, string header)
		{
			return taskList.Count(x => !x.Completed && groupMode switch
			{
				1 => x.SubjectHeader() == header,
				2 => x.DateHeader() == header,
				_ => x.PriorityHeader() == header
			});
		}
		private static int ReturnSubtaskCount(List<Task> taskList, int groupMode, string header)
		{
			return groupMode switch
			{
				1 => taskList.Where(x => x.SubjectHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed)),
				2 => taskList.Where(x => x.DateHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed)),
				_ => taskList.Where(x => x.PriorityHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed))
			};
		}

		private static void SetPage(this PageDescriptor x)
		{
			x.Size(PageSizes.A4);
			x.Margin(36);
			x.DefaultTextStyle(x => x.FontColor("#19223F").FontFamily("Roboto").FontSize(10));
		}
		private static void AddTitle(this ColumnDescriptor x)
		{
			x.Spacing(12);
			x.Item().AlignCenter().Text($"tasks - {DateTime.Now:dddd d MMMM}".ToLower()).Style(Styles.Title);
		}
		private static void AddHeader(this ColumnDescriptor x, List<Task> taskList, int groupMode, string header)
		{
			x.Item().Text(x =>
			{
				List<string> subtitleText = [];
				int taskCount = ReturnTaskCount(taskList, groupMode, header);
				int subtaskCount = ReturnSubtaskCount(taskList, groupMode, header);
				if (taskCount != 0)
					subtitleText.Add(taskCount + (taskCount == 1 ? " task" : " tasks"));
				if (subtaskCount != 0)
					subtitleText.Add(subtaskCount + (subtaskCount == 1 ? " subtask" : " subtasks"));

				x.Line(header).Style(Styles.Header);
				x.Span(subtitleText.Count != 0 ? string.Join(" • ", subtitleText) : "completed").Style(Styles.Subtitle);
			});
		}
		private static void AddTask(this DecorationDescriptor x, Task task)
		{
			x.Content().Row(x =>
			{
				if (task.Completed)
					x.ConstantItem(24).AlignMiddle().AlignCenter().Height(16).Width(16).Border(1).BorderColor("#E5E5E5")
						.Canvas((x, y) => x.DrawPath(Styles.LargeTick, Styles.GreenLarge));
				else
					x.ConstantItem(24).AlignMiddle().AlignCenter().Height(16).Width(16).Border(1).BorderColor("#E5E5E5");

				x.RelativeItem(1).Text(x =>
				{
					List<string> subtitleText = [];
					if (task.Date != null)
						subtitleText.Add(task.DateTimeText());
					if (task.Subject != null)
						subtitleText.Add(task.Subject!);
					if (task.Details != null)
						subtitleText.Add(task.Details!);

					if (subtitleText.Count != 0)
					{
						if (task.Completed)
						{
							x.Line(task.PriorityText() + task.Title).Style(Styles.TaskStrikethrough);
							x.Span(string.Join(" • ", subtitleText)).Style(Styles.SubtitleStrikethrough);
						}
						else
						{
							x.Span(task.PriorityText()).Style(Styles.Priority);
							x.Line(task.Title).Style(Styles.Task);
							x.Span(string.Join(" • ", subtitleText)).Style(Styles.Subtitle);
						}
					}
					else
						x.Span(task.Title).Style(task.Completed ? Styles.TaskStrikethrough : Styles.Task);
				});
			});
		}
		private static void AddSubtasks(this DecorationDescriptor x, Task task)
		{
			x.After().Row(x =>
			{
				x.ConstantItem(24);

				x.RelativeItem(1).BorderLeft(2).BorderColor("#E5E5E5").Column(x =>
				{
					foreach (Subtask subtask in task.Subtasks)
						x.Item().Row(x =>
						{
							if (subtask.Completed)
							{
								x.ConstantItem(24).AlignMiddle().AlignCenter().Height(12).Width(12).Border(1).BorderColor("#E5E5E5")
									.Canvas((x, y) => x.DrawPath(Styles.SmallTick, Styles.GreenSmall));

								x.RelativeItem(1).Text(subtask.Title).Style(Styles.SubtaskStrikethrough);
							}
							else
							{
								x.ConstantItem(24).AlignMiddle().AlignCenter().Height(12).Width(12).Border(1).BorderColor("#E5E5E5");
								x.RelativeItem(1).Text(subtask.Title).Style(Styles.Subtask);
							}
						});
				});
			});
		}
	}

	static class TimetableGenerator
	{
		public static byte[] GeneratePdf(List<Timetable> timetableList)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			int minTime = MinTime(timetableList);
			int maxTime = MaxTime(timetableList);

			return
			Document.Create(x =>
			{
				x.Page(x =>
				{
					x.SetPage(minTime, maxTime);
					x.Content().Column(x =>
					{
						x.Item().AddTitle();
						x.Item().Table(x =>
						{
							x.AddColumns();
							x.AddDays();
							x.AddCells(minTime, maxTime);
							x.AddTimes(minTime, maxTime);

							foreach (Timetable timetable in timetableList)
								x.AddEvent(timetable, minTime);
						});
					});
				});
			}).GeneratePdf();
		}

		private static int MinTime(List<Timetable> timetableList) => timetableList.Any(x => x.StartTime.ToTimeSpan().TotalHours < 9)
			? (int)Math.Floor(timetableList.Min(x => x.StartTime.ToTimeSpan().TotalHours))
			: 9;

		private static int MaxTime(List<Timetable> timetableList) => timetableList.Any(x => x.EndTime.ToTimeSpan().TotalHours > 16)
			? (int)Math.Ceiling(timetableList.Max(x => x.EndTime.ToTimeSpan().TotalHours))
			: 16;

		private static void SetPage(this PageDescriptor x, int minTime, int maxTime)
		{
			x.Size(maxTime - minTime <= 8 ? PageSizes.A4.Landscape() : PageSizes.A4.Portrait());
			x.MarginTop(24);
			x.MarginHorizontal(36);
			x.DefaultTextStyle(x => x.FontColor("#19223F").FontFamily("Roboto").FontSize(10));
		}
		private static void AddTitle(this IContainer x) => x.AlignCenter().Text("timetable").Style(Styles.Title);
		private static void AddColumns(this TableDescriptor x)
		{
			x.ColumnsDefinition(x =>
			{
				x.ConstantColumn(40);
				x.RelativeColumn();
				x.RelativeColumn();
				x.RelativeColumn();
				x.RelativeColumn();
				x.RelativeColumn();
			});
		}
		private static void AddDays(this TableDescriptor x)
		{
			x.Cell().Row(1).Column(2).Height(20).AlignCenter().AlignBottom().Text("monday");
			x.Cell().Row(1).Column(3).Height(20).AlignCenter().AlignBottom().Text("tuesday");
			x.Cell().Row(1).Column(4).Height(20).AlignCenter().AlignBottom().Text("wednesday");
			x.Cell().Row(1).Column(5).Height(20).AlignCenter().AlignBottom().Text("thursday");
			x.Cell().Row(1).Column(6).Height(20).AlignCenter().AlignBottom().Text("friday");
		}
		private static void AddCells(this TableDescriptor x, int minTime, int maxTime)
		{
			for (int column = 2; column <= 6; column++)
				x.Cell().Row(2).Column((uint)column).Height(10).BorderVertical(1).BorderColor("#19223F");

			for (int row = 3; row < maxTime - minTime + 3; row++)
			{
				for (int column = 2; column <= 6; column++)
					x.Cell().Row((uint)row).Column((uint)column).Height(60).Border(1).BorderColor("#19223F");
			}

			for (int column = 2; column <= 6; column++)
				x.Cell().Row((uint)(maxTime - minTime + 3)).Column((uint)column).Height(10).BorderVertical(1).BorderColor("#19223F");
		}
		private static void AddTimes(this TableDescriptor x, int minTime, int maxTime)
		{
			for (int row = minTime; row <= maxTime; row++)
				x.Cell().Column(0).Row((uint)(row - minTime + 2)).RowSpan(2)
					.AlignCenter().AlignMiddle().PaddingBottom(row == minTime ? 50 : row == maxTime ? -50 : 0)
					.Text($"{row:00}:00");
		}
		private static void AddEvent(this TableDescriptor x, Timetable timetable, int minTime)
		{
			x.Cell().Column((uint)timetable.Day + 1).Row((uint)(Math.Floor(timetable.StartTime.ToTimeSpan().TotalHours) - minTime + 3))
				.RowSpan((uint)(Math.Ceiling(timetable.EndTime.ToTimeSpan().TotalHours) - Math.Floor(timetable.StartTime.ToTimeSpan().TotalHours)))
				.PaddingTop((uint)(timetable.StartTime.ToTimeSpan().TotalMinutes % 60))
				.PaddingBottom((uint)(60 - (timetable.EndTime.ToTimeSpan().TotalMinutes % 60 == 0
					? 60
					: timetable.EndTime.ToTimeSpan().TotalMinutes % 60)))
				.Background(Styles.BackgroundColour(timetable.Colour))
				.BorderTop(timetable.StartTime.ToTimeSpan().TotalMinutes % 60 == 0 ? 1 : 0)
				.BorderBottom(timetable.EndTime.ToTimeSpan().TotalMinutes % 60 == 0 ? 1 : 0)
				.BorderVertical(1).BorderColor("#19223F").AlignCenter().AlignMiddle().Text(x =>
				{
					List<string> subtitleText = [];
					if (timetable.Details != null)
						subtitleText.Add(timetable.Details);
					if (timetable.Subject != null)
						subtitleText.Add(timetable.Subject);
					if (timetable.Location != null)
						subtitleText.Add(timetable.Location);

					if (subtitleText.Count != 0 && timetable.Length() >= 30)
					{
						x.Line(timetable.Title).Style(Styles.Task);
						x.Span(string.Join(" • ", subtitleText)).Style(Styles.Subtitle);
					}
					else
						x.Span(timetable.Title).Style(Styles.Task);
				});
		}
	}

	static class Styles
	{
		public static TextStyle Title = TextStyle.Default.FontSize(14).Weight(FontWeight.SemiBold).Underline();
		public static TextStyle Header = TextStyle.Default.FontSize(12).Weight(FontWeight.SemiBold);
		public static TextStyle Task = TextStyle.Default.Weight(FontWeight.SemiBold);
		public static TextStyle TaskStrikethrough = TextStyle.Default.FontColor("666666").Weight(FontWeight.SemiBold).Strikethrough();
		public static TextStyle Priority = TextStyle.Default.FontColor("8B0000").Weight(FontWeight.SemiBold);
		public static TextStyle Subtitle = TextStyle.Default.FontColor("666666");
		public static TextStyle SubtitleStrikethrough = TextStyle.Default.FontColor("666666").Strikethrough();
		public static TextStyle Subtask = TextStyle.Default.LineHeight(1.5f);
		public static TextStyle SubtaskStrikethrough = TextStyle.Default.FontColor("666666").LineHeight(1.5f);

		public static SKPath SmallTick
		{
			get
			{
				SKPath tick = new();
				tick.MoveTo(1.5f, 7.5f);
				tick.LineTo(4.5f, 10.5f);
				tick.LineTo(10.5f, 1.5f);
				return tick;
			}
		}
		public static SKPath LargeTick
		{
			get
			{
				SKPath tick = new();
				tick.MoveTo(2, 10);
				tick.LineTo(6, 14);
				tick.LineTo(14, 2);
				return tick;
			}
		}
		public static SKPaint GreenSmall = new() { Color = SKColor.Parse("#008B00"), IsStroke = true, StrokeWidth = 1 };
		public static SKPaint GreenLarge = new() { Color = SKColor.Parse("#008B00"), IsStroke = true, StrokeWidth = 2 };
		public static SKPaint NavySmall = new() { Color = SKColor.Parse("#19223F"), IsStroke = true, StrokeWidth = 1 };

		public static string BackgroundColour(int colour) => colour switch
		{
			0 => "#F2F2F2",
			1 => "#FFADAD",
			2 => "#FFD6A5",
			3 => "#FDFFB6",
			4 => "#CAFFBF",
			5 => "#9BF6FF",
			6 => "#A0C4FF",
			7 => "#BDB2FF",
			_ => "#FFC6FF"
		};
	}
}