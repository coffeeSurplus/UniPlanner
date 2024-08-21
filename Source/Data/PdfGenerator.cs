using Microsoft.Win32;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.Data;

internal abstract class PdfGenerator
{
	private readonly string fileName;
	private readonly SettingsModel settings;
	protected readonly SaveFileDialog saveFileDialog = new() { DefaultExt = ".pdf", Filter = "Pdf File (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1 };

	public PdfGenerator(string fileName, SettingsModel settingsModel)
	{
		settings = settingsModel;
		Settings.License = LicenseType.Community;
		this.fileName = $"UniPlanner - {fileName}";
	}

	public void SavePdf()
	{
		saveFileDialog.FileName = fileName;
		if (saveFileDialog.ShowDialog() == true)
		{
			File.WriteAllBytes(saveFileDialog.FileName, GeneratePdf());
			if (settings.PdfSaveAction != 3)
			{
				Process.Start(settings.PdfSaveAction switch { 0 => new("explorer.exe", saveFileDialog.FileName), 1 => new ProcessStartInfo() { FileName = settings.ReturnBrowser(), Arguments = settings.ReturnArguments() + Uri.EscapeDataString(saveFileDialog.FileName), UseShellExecute = true }, _ => new("explorer.exe", Directory.GetParent(saveFileDialog.FileName)!.FullName) });
			}
		}
	}

	protected abstract byte[] GeneratePdf();
}

internal class TaskPdfGenerator(List<TaskModel> taskList, string fileName, SettingsModel settingsModel) : PdfGenerator(fileName, settingsModel)
{
	private readonly List<TaskModel> pdfTaskList = [];
	private readonly List<TaskModel> taskList = taskList;
	private (string Group, bool OrderByAscending, bool ShowCompleted) settings = ("Subject", true, true);

	public void UpdateValues(string group, bool orderByAscending, bool showCompleted) => settings = new(group, orderByAscending, showCompleted);

	protected override byte[] GeneratePdf()
	{
		SortTasklist();
		return Document.Create(x =>
		{
			x.Page(x =>
			{
				x.SetTaskPage();
				x.Content().Column(x =>
				{
					x.AddTaskTitle();
					for (int index = 0; index < pdfTaskList.Count; index++)
					{
						if (NewHeader(index))
						{
							x.AddTaskHeader(ReturnHeader(index), ReturnTaskCount(index), ReturnSubtaskCount(index));
						}
						x.AddTaskTask(pdfTaskList[index]);
						x.AddTaskSubtasks(pdfTaskList[index].Subtasks);
					}
				});
			});
		}).GeneratePdf();
	}

	private void SortTasklist()
	{
		pdfTaskList.Clear();
		pdfTaskList.AddRange(((settings.Group, settings.OrderByAscending) switch { ("Subject", true) => taskList.OrderBy(x => x.Subject).ThenBy(x => x.Completed).ThenBy(x => x.Title), ("Subject", false) => taskList.OrderByDescending(x => x.Subject).ThenBy(x => x.Completed).ThenByDescending(x => x.Title), ("Date", true) => taskList.OrderBy(x => x.Date).ThenBy(x => x.Completed).ThenBy(x => x.Title), ("Date", false) => taskList.OrderByDescending(x => x.Date).ThenBy(x => x.Completed).ThenByDescending(x => x.Title), ("Priority", true) => taskList.OrderBy(x => x.Priority).ThenBy(x => x.Completed).ThenBy(x => x.Title), _ => taskList.OrderByDescending(x => x.Priority).ThenBy(x => x.Completed).ThenByDescending(x => x.Title) }).Select(x => new TaskModel(x)));
		foreach (TaskModel taskModel in pdfTaskList)
		{
			taskModel.Subtasks = [.. settings.OrderByAscending ? taskModel.Subtasks.OrderBy(x => x.Completed).ThenBy(x => x.Title) : taskModel.Subtasks.OrderBy(x => x.Completed).ThenByDescending(x => x.Title)];
		}
		if (!settings.ShowCompleted)
		{
			foreach (TaskModel taskModel in pdfTaskList)
			{
				taskModel.Subtasks.RemoveAll(x => x.Completed);
			}
			pdfTaskList.RemoveAll(x => x.Completed && x.Subtasks.Count == 0);
		}
	}
	private bool NewHeader(int index) => index == 0 || ReturnHeader(index) != ReturnHeader(index - 1);
	private string ReturnHeader(int index) => settings.Group switch { "Subject" => pdfTaskList[index].Subject ?? "other", "Date" => pdfTaskList[index].Date != DateOnly.MaxValue ? (pdfTaskList[index].Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) switch { < 0 => "overdue", 0 => "today", 1 => "tomorrow", < 7 => "this week", _ => "later" } : "no date", _ => pdfTaskList[index].Priority switch { 1 => "high priority", 2 => "medium priority", 3 => "low priority", _ => "no priority" } };
	private int ReturnTaskCount(int index) => pdfTaskList.Count(x => settings.Group switch { "Subject" => x.Subject == pdfTaskList[index].Subject, "Date" => x.Date == pdfTaskList[index].Date, _ => x.Priority == pdfTaskList[index].Priority } && !x.Completed);
	private int ReturnSubtaskCount(int index) => pdfTaskList.Where(x => settings.Group switch { "Subject" => x.Subject == pdfTaskList[index].Subject, "Date" => x.Date == pdfTaskList[index].Date, _ => x.Priority == pdfTaskList[index].Priority }).Sum(x => x.Subtasks.Count(x => !x.Completed));
}

internal class TimetablePdfGenerator(List<TimetableModel> timetableList, string fileName, SettingsModel settingsModel) : PdfGenerator(fileName, settingsModel)
{
	private readonly List<TimetableModel> timetableList = timetableList;

	protected override byte[] GeneratePdf()
	{
		return Document.Create(x =>
		{
			x.Page(x =>
			{
				x.SetTimetablePage();
				x.Content().Column(x =>
				{
					x.AddTimetableTitle();
					x.Item().Table(x =>
					{
						x.AddTimetableColumns();
						x.AddTimetableDays();
						x.AddTimetableCells();
						x.AddTimetableTimes();
						foreach (TimetableModel timetableModel in timetableList)
						{
							x.AddTimetableModel(timetableModel);
						}
					});
				});
			});
		}).GeneratePdf();
	}
}