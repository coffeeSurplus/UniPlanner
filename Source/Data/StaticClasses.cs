using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System.IO;
using System.Text;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.Data;

internal static class ExtensionMethods
{
	public static string ToPageTitle(this int pageNumber) => pageNumber switch { 0 => "home", 1 => "timetable", 2 => "tasks", 3 => "events", 4 => "links", 5 => "timers", _ => "settings" };
	public static int UKDayOfWeek(this DateOnly date) => (int)date.DayOfWeek switch { 1 => 0, 2 => 1, 3 => 2, 4 => 3, 5 => 4, 6 => 5, _ => 6 };
	public static DateOnly FirstDayOfWeek(this DateOnly date) => date.AddDays(date.UKDayOfWeek() * -1);
}

internal static class TaskPdfExtensionMethods
{
	public static void SetTaskPage(this PageDescriptor x)
	{
		x.Size(PageSizes.A4);
		x.Margin(36);
		x.DefaultTextStyle(x => x.FontColor("#19223F").FontFamily("Roboto").FontSize(10));
	}
	public static void AddTaskTitle(this ColumnDescriptor x)
	{
		x.Spacing(12);
		x.Item().AlignCenter().Text($"tasks - {DateTime.Now:dddd d MMMM}".ToLower()).Style(PdfStyles.Title);
	}
	public static void AddTaskHeader(this ColumnDescriptor x, string header, int taskCount, int subtaskCount)
	{
		x.Item().Text(x =>
		{
			List<string> subtitle = [];
			if (taskCount > 0)
			{
				subtitle.Add($"{taskCount} {(taskCount is not 1 ? "tasks" : "task")}");
			}
			if (subtaskCount > 0)
			{
				subtitle.Add($"{subtaskCount} {(subtaskCount is not 1 ? "subtasks" : "subtask")}");
			}
			x.Line(header).Style(PdfStyles.Header);
			x.Span(subtitle.Count is not 0 ? string.Join(" • ", subtitle) : "completed").Style(PdfStyles.Subtitle);
		});
	}
	public static void AddTaskTask(this ColumnDescriptor x, TaskModel taskModel)
	{
		x.Item().Row(x =>
		{
			if (taskModel.Completed)
			{
				x.ConstantItem(24).AlignMiddle().AlignCenter().Height(16).Width(16).Border(1).BorderColor("#E5E5E5").Canvas((x, y) => x.DrawPath(PdfStyles.LargeTick, PdfStyles.GreenLarge));
			}
			else
			{
				x.ConstantItem(24).AlignMiddle().AlignCenter().Height(16).Width(16).Border(1).BorderColor("#E5E5E5");
			}
			x.RelativeItem(1).Text(x =>
			{
				List<string> subtitle = [];
				if (taskModel.Details is not null)
				{
					subtitle.Add(taskModel.Details);
				}
				if (taskModel.Subject is not null)
				{
					subtitle.Add(taskModel.Subject);
				}
				if (taskModel.Date != DateOnly.MaxValue)
				{
					subtitle.Add(taskModel.Date.ToString("dddd d MMMM"));
				}
				if (subtitle.Count is not 0)
				{
					if (taskModel.Completed)
					{
						x.Line($"{taskModel.Priority switch { 1 => "!!! ", 2 => "!! ", 3 => "! ", _ => string.Empty }}{taskModel.Title}").Style(PdfStyles.TaskStrikethrough);
						x.Span(string.Join(" • ", subtitle)).Style(PdfStyles.SubtitleStrikethrough);
					}
					else
					{
						x.Span($"{taskModel.Priority switch { 1 => "!!! ", 2 => "!! ", 3 => "! ", _ => string.Empty }}").Style(PdfStyles.Priority);
						x.Line(taskModel.Title).Style(PdfStyles.Task);
						x.Span(string.Join(" • ", subtitle)).Style(PdfStyles.Subtitle);
					}
				}
				else
				{
					if (taskModel.Completed)
					{
						x.Span($"{taskModel.Priority switch { 1 => "!!! ", 2 => "!! ", 3 => "! ", _ => string.Empty }}{taskModel.Title}").Style(PdfStyles.TaskStrikethrough);
					}
					else
					{
						x.Span($"{taskModel.Priority switch { 1 => "!!! ", 2 => "!! ", 3 => "! ", _ => string.Empty }}").Style(PdfStyles.Priority);
						x.Span(taskModel.Title).Style(PdfStyles.TaskStrikethrough);
					}
				}
			});
		});
	}
	public static void AddTaskSubtasks(this ColumnDescriptor x, List<SubtaskModel> subtasks)
	{
		if (subtasks.Count > 0)
		{
			x.Item().Row(x =>
			{
				x.ConstantItem(24);
				x.RelativeItem(1).BorderLeft(2).BorderColor("#E5E5E5").Column(x =>
				{
					foreach (SubtaskModel subtaskModel in subtasks)
					{
						x.Item().Row(x =>
						{
							if (subtaskModel.Completed)
							{
								x.ConstantItem(24).AlignMiddle().AlignCenter().Height(12).Width(12).Border(1).BorderColor("#E5E5E5").Canvas((x, y) => x.DrawPath(PdfStyles.SmallTick, PdfStyles.GreenSmall));
							}
							else
							{
								x.ConstantItem(24).AlignMiddle().AlignCenter().Height(12).Width(12).Border(1).BorderColor("#E5E5E5");
							}
							x.RelativeItem(1).Text(subtaskModel.Title).Style(subtaskModel.Completed ? PdfStyles.SubtaskStrikethrough : PdfStyles.Subtask);
						});
					}
				});
			});
		}
	}
}

internal static class TimetablePdfExtensionMethods
{
	public static void SetTimetablePage(this PageDescriptor x)
	{
		x.Size(PageSizes.A4.Landscape());
		x.MarginTop(24);
		x.MarginHorizontal(36);
		x.DefaultTextStyle(x => x.FontColor("#19223F").FontFamily("Roboto").FontSize(10));
	}
	public static void AddTimetableTitle(this ColumnDescriptor x, string title) => x.Item().AlignCenter().Text(title).Style(PdfStyles.Title);
	public static void AddTimetableColumns(this TableDescriptor x)
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
	public static void AddTimetableDays(this TableDescriptor x)
	{
		x.Cell().Row(1).Column(2).Height(20).AlignCenter().AlignBottom().Text("monday");
		x.Cell().Row(1).Column(3).Height(20).AlignCenter().AlignBottom().Text("tuesday");
		x.Cell().Row(1).Column(4).Height(20).AlignCenter().AlignBottom().Text("wednesday");
		x.Cell().Row(1).Column(5).Height(20).AlignCenter().AlignBottom().Text("thursday");
		x.Cell().Row(1).Column(6).Height(20).AlignCenter().AlignBottom().Text("friday");
	}
	public static void AddTimetableCells(this TableDescriptor x)
	{
		for (uint column = 2; column <= 6; column++)
		{
			x.Cell().Row(2).Column(column).Height(10).BorderVertical(1).BorderColor("#19223F");
		}

		for (uint row = 3; row <= 11; row++)
		{
			for (uint column = 2; column <= 6; column++)
			{
				x.Cell().Row(row).Column(column).Height(60).Border(1).BorderColor("#19223F");
			}
		}

		for (uint column = 2; column <= 6; column++)
		{
			x.Cell().Row(11).Column(column).Height(10).BorderVertical(1).BorderColor("#19223F");
		}
	}
	public static void AddTimetableTimes(this TableDescriptor x)
	{
		for (uint row = 2; row <= 11; row++)
		{
			x.Cell().Column(0).Row(row).RowSpan(2).AlignCenter().AlignMiddle().PaddingBottom(row is 2 ? 50 : row is 11 ? -50 : 0).Text($"{row + 7:00}:00");
		}
	}
	public static void AddTimetableModel(this TableDescriptor x, TimetableModel timetableModel)
	{
		x.Cell().Column((uint)timetableModel.Day + 2).Row((uint)(Math.Floor(timetableModel.StartTime.ToTimeSpan().TotalHours) - 6)).RowSpan((uint)(Math.Ceiling(timetableModel.EndTime.ToTimeSpan().TotalHours) - Math.Floor(timetableModel.StartTime.ToTimeSpan().TotalHours))).PaddingTop((uint)(timetableModel.StartTime.ToTimeSpan().TotalMinutes % 60)).PaddingBottom((uint)(60 - (timetableModel.EndTime.ToTimeSpan().TotalMinutes % 60 is 0 ? 60 : timetableModel.EndTime.ToTimeSpan().TotalMinutes % 60))).Background(PdfStyles.BackgroundColour(timetableModel.Colour)).BorderTop(timetableModel.StartTime.ToTimeSpan().TotalMinutes % 60 is 0 ? 1 : 0).BorderBottom(timetableModel.EndTime.ToTimeSpan().TotalMinutes % 60 is 0 ? 1 : 0).BorderVertical(1).BorderColor("#19223F").AlignCenter().AlignMiddle().Text(x =>
		{
			List<string> subtitleText = [];
			if (timetableModel.Details is not null)
			{
				subtitleText.Add(timetableModel.Details);
			}
			if (timetableModel.Subject is not null)
			{
				subtitleText.Add(timetableModel.Subject);
			}
			if (timetableModel.Location is not null)
			{
				subtitleText.Add(timetableModel.Location);
			}
			if (subtitleText.Count is not 0 && (timetableModel.EndTime - timetableModel.StartTime).TotalMinutes >= 30)
			{
				x.Line(timetableModel.Title).Style(PdfStyles.Task);
				x.Span(string.Join(" • ", subtitleText)).Style(PdfStyles.Subtitle);
			}
			else
			{
				x.Span(timetableModel.Title).Style(PdfStyles.Task);
			}
		});
	}
}

internal static class PdfStyles
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

	public static string BackgroundColour(int colour) => colour switch { 0 => "#F2F2F2", 1 => "#FFADAD", 2 => "#FFD6A5", 3 => "#FDFFB6", 4 => "#CAFFBF", 5 => "#9BF6FF", 6 => "#A0C4FF", 7 => "#BDB2FF", _ => "#FFC6FF" };
	public static void Canvas(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
	{
		container.Svg(size =>
		{
			using MemoryStream stream = new();
			using (SKCanvas canvas = SKSvgCanvas.Create(new SKRect(0, 0, size.Width, size.Height), stream))
			{
				drawOnCanvas(canvas, size);
			}
			return Encoding.UTF8.GetString(stream.ToArray());
		});
	}
}

internal static class Popup
{
	public static void MessageBox(string message) => System.Windows.MessageBox.Show(message, "UniPlanner");
}