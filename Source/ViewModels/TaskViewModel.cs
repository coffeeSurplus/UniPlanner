using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class TaskViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
	private readonly List<TaskModel> taskList;
	private readonly TaskPdfGenerator pdfGenerator;
	private bool newTask = true;
	private bool newSubtask = true;
	private TaskModel currentTask = new();
	private SubtaskModel currentSubtask = new();

	private string group = "Subject";
	private bool orderByAscending = true;
	private bool showCompleted = true;
	private bool taskEditorVisible = false;
	private bool subtaskEditorVisible = false;
	private bool pdfEditorVisible = false;
	private bool defaultMessageVisible = false;
	private string currentTaskTitle = string.Empty;
	private string currentTaskDetails = string.Empty;
	private string currentTaskSubject = string.Empty;
	private string currentTaskDate = string.Empty;
	private int currentTaskPriority = 4;
	private string currentSubtaskTitle = string.Empty;
	private string pdfGroup = "Subject";
	private bool pdfOrderByAscending = true;
	private bool pdfShowCompleted = true;

	public string Group
	{
		get => group;
		set => SetValue(ref group, value);
	}
	public bool OrderByAscending
	{
		get => orderByAscending;
		set => SetValue(ref orderByAscending, value);
	}
	public bool ShowCompleted
	{
		get => showCompleted;
		set => SetValue(ref showCompleted, value);
	}
	public bool TaskEditorVisible
	{
		get => taskEditorVisible;
		set => SetValue(ref taskEditorVisible, value);
	}
	public bool SubtaskEditorVisible
	{
		get => subtaskEditorVisible;
		set => SetValue(ref subtaskEditorVisible, value);
	}
	public bool PdfEditorVisible
	{
		get => pdfEditorVisible;
		set => SetValue(ref pdfEditorVisible, value);
	}
	public bool DefaultMessageVisible
	{
		get => defaultMessageVisible;
		set => SetValue(ref defaultMessageVisible, value);
	}
	public string CurrentTaskTitle
	{
		get => currentTaskTitle;
		set => SetValue(ref currentTaskTitle, value);
	}
	public string CurrentTaskDetails
	{
		get => currentTaskDetails;
		set => SetValue(ref currentTaskDetails, value);
	}
	public string CurrentTaskSubject
	{
		get => currentTaskSubject;
		set => SetValue(ref currentTaskSubject, value);
	}
	public string CurrentTaskDate
	{
		get => currentTaskDate;
		set => SetValue(ref currentTaskDate, value);
	}
	public int CurrentTaskPriority
	{
		get => currentTaskPriority;
		set => SetValue(ref currentTaskPriority, value);
	}
	public string CurrentSubtaskTitle
	{
		get => currentSubtaskTitle;
		set => SetValue(ref currentSubtaskTitle, value);
	}
	public string PdfGroup
	{
		get => pdfGroup;
		set => SetValue(ref pdfGroup, value);
	}
	public bool PdfOrderByAscending
	{
		get => pdfOrderByAscending;
		set => SetValue(ref pdfOrderByAscending, value);
	}
	public bool PdfShowCompleted
	{
		get => pdfShowCompleted;
		set => SetValue(ref pdfShowCompleted, value);
	}

	public RelayCommand UpdateViewCommand { get; }
	public RelayCommand NewTaskCommand { get; }
	public RelayCommand RemoveCompletedCommand { get; }
	public RelayCommand RemoveOverdueCommand { get; }
	public RelayCommand SaveAsPdfCommand { get; }
	public RelayCommand TaskCompletedCommand { get; }
	public RelayCommand CancelEditTaskCommand { get; }
	public RelayCommand SaveEditTaskCommand { get; }
	public RelayCommand SubtaskCompletedCommand { get; }
	public RelayCommand CancelEditSubtaskCommand { get; }
	public RelayCommand SaveEditSubtaskCommand { get; }
	public RelayCommand CancelEditPdfCommand { get; }
	public RelayCommand SaveEditPdfCommand { get; }
	public RelayCommand<TaskModel> AddSubtaskCommand { get; }
	public RelayCommand<TaskModel> EditTaskCommand { get; }
	public RelayCommand<TaskModel> RemoveTaskCommand { get; }
	public RelayCommand<SubtaskModel> EditSubtaskCommand { get; }
	public RelayCommand<Tuple<TaskModel, SubtaskModel>> RemoveSubtaskCommand { get; }

	public TaskCollectionView TaskCollectionView { get; }

	public TaskViewModel(DataAccess data)
	{
		dataAccess = data;
		taskList = dataAccess.TaskList;
		pdfGenerator = new(taskList, "tasks", dataAccess.Settings);
		TaskCollectionView = new(taskList);
		UpdateViewCommand = new(UpdateView);
		NewTaskCommand = new(NewTask);
		RemoveCompletedCommand = new(RemoveCompleted);
		RemoveOverdueCommand = new(RemoveOverdue);
		SaveAsPdfCommand = new(SaveAsPdf);
		TaskCompletedCommand = new(TaskCompleted);
		CancelEditTaskCommand = new(CancelEditTask);
		SaveEditTaskCommand = new(SaveEditTask);
		SubtaskCompletedCommand = new(SubtaskCompleted);
		CancelEditSubtaskCommand = new(CancelEditSubtask);
		SaveEditSubtaskCommand = new(SaveEditSubtask);
		CancelEditPdfCommand = new(CancelEditPdf);
		SaveEditPdfCommand = new(SaveEditPdf);
		AddSubtaskCommand = new(AddSubtask);
		EditTaskCommand = new(EditTask);
		RemoveTaskCommand = new(RemoveTask);
		EditSubtaskCommand = new(EditSubtask);
		RemoveSubtaskCommand = new(RemoveSubtask);
		UpdateView(false);
	}

	private void UpdateView() => UpdateView(true);
	private void NewTask()
	{
		newTask = true;
		currentTask = new();
		CurrentTaskTitle = string.Empty;
		CurrentTaskDetails = string.Empty;
		CurrentTaskSubject = string.Empty;
		CurrentTaskDate = string.Empty;
		CurrentTaskPriority = 4;
		TaskEditorVisible = true;
	}
	private void RemoveCompleted()
	{
		int taskRemovedCount = taskList.RemoveAll(x => x.Completed && x.Subtasks.All(x => x.Completed));
		int subtaskRemovedCount = 0;
		foreach (TaskModel taskModel in taskList)
		{
			subtaskRemovedCount += taskModel.Subtasks.RemoveAll(x => x.Completed);
		}
		if (taskRemovedCount > 0 || subtaskRemovedCount > 0)
		{
			UpdateData();
			UpdateView(true);
			List<string> message = [];
			if (taskRemovedCount > 0)
			{
				message.Add($"{taskRemovedCount} {(taskRemovedCount != 1 ? "tasks" : "task")}");
			}
			if (subtaskRemovedCount > 0)
			{
				message.Add($"{subtaskRemovedCount} {(subtaskRemovedCount != 1 ? "subtasks" : "subtask")}");
			}
			Popup.MessageBox($"{string.Join(" and ", message)} removed.");
		}
	}
	private void RemoveOverdue()
	{
		int removedCount = taskList.RemoveAll(x => x.Date.CompareTo(DateOnly.FromDateTime(DateTime.Now)) < 0);
		if (removedCount > 0)
		{
			UpdateData();
			UpdateView(true);
			Popup.MessageBox($"{removedCount} {(removedCount != 1 ? "tasks" : "task")} removed.");
		}
	}
	private void SaveAsPdf() => PdfEditorVisible = true;
	private void TaskCompleted()
	{
		UpdateData();
		UpdateView(true);
	}
	private void CancelEditTask() => TaskEditorVisible = false;
	private void SaveEditTask()
	{
		if (CheckTaskValues())
		{
			currentTask.Title = CurrentTaskTitle.Trim().ToLower();
			currentTask.Details = !string.IsNullOrWhiteSpace(CurrentTaskDetails) ? CurrentTaskDetails.Trim().ToLower() : null;
			currentTask.Subject = !string.IsNullOrWhiteSpace(CurrentTaskSubject) ? CurrentTaskSubject.Trim().ToLower() : null;
			currentTask.Date = !string.IsNullOrWhiteSpace(CurrentTaskDate) ? DateOnly.Parse(CurrentTaskDate) : DateOnly.MaxValue;
			currentTask.Priority = CurrentTaskPriority;
			if (newTask)
			{
				taskList.Add(currentTask);
			}
			TaskEditorVisible = false;
			UpdateData();
			UpdateView(true);
		}
	}
	private void SubtaskCompleted()
	{
		UpdateData();
		UpdateView(true);
	}
	private void CancelEditSubtask() => SubtaskEditorVisible = false;
	private void SaveEditSubtask()
	{
		if (CheckSubtaskValues())
		{
			currentSubtask.Title = CurrentSubtaskTitle.Trim().ToLower();
			if (newSubtask)
			{
				currentTask.Subtasks.Add(currentSubtask);
			}
			SubtaskEditorVisible = false;
			UpdateData();
			UpdateView(true);
		}
	}
	private void CancelEditPdf() => PdfEditorVisible = false;
	private void SaveEditPdf()
	{
		pdfGenerator.UpdateValues(PdfGroup, PdfOrderByAscending, PdfShowCompleted);
		pdfGenerator.SavePdf();
		PdfEditorVisible = false;
	}
	private void AddSubtask(TaskModel parameter)
	{
		newSubtask = true;
		currentTask = parameter;
		currentSubtask = new();
		CurrentSubtaskTitle = string.Empty;
		SubtaskEditorVisible = true;
	}
	private void EditTask(TaskModel parameter)
	{
		newTask = false;
		currentTask = parameter;
		CurrentTaskTitle = currentTask.Title;
		CurrentTaskDetails = currentTask.Details ?? string.Empty;
		CurrentTaskSubject = currentTask.Subject ?? string.Empty;
		CurrentTaskDate = currentTask.Date != DateOnly.MaxValue ? currentTask.Date.ToString("d/M") : string.Empty;
		CurrentTaskPriority = currentTask.Priority;
		TaskEditorVisible = true;
	}
	private void RemoveTask(TaskModel parameter)
	{
		taskList.Remove(parameter);
		UpdateData();
		UpdateView(true);
	}
	private void EditSubtask(SubtaskModel parameter)
	{
		newSubtask = false;
		currentSubtask = parameter;
		CurrentSubtaskTitle = currentSubtask.Title;
		SubtaskEditorVisible = true;
	}
	private void RemoveSubtask(Tuple<TaskModel, SubtaskModel> parameter)
	{
		parameter.Item1.Subtasks.Remove(parameter.Item2);
		UpdateData();
		UpdateView(true);
	}

	private bool CheckTaskValues()
	{
		if (!string.IsNullOrWhiteSpace(CurrentTaskTitle))
		{
			if (string.IsNullOrWhiteSpace(CurrentTaskDate) || DateOnly.TryParse(CurrentTaskDate, out _))
			{
				return true;
			}
			else
			{
				Popup.MessageBox("Invalid date.");
			}
		}
		else
		{
			Popup.MessageBox("Invalid title.");
		}
		TaskEditorVisible = true;
		return false;
	}
	private bool CheckSubtaskValues()
	{
		if (!string.IsNullOrWhiteSpace(CurrentSubtaskTitle))
		{
			return true;
		}
		else
		{
			Popup.MessageBox("Invalid title.");
			SubtaskEditorVisible = true;
			return false;
		}
	}
	private void UpdateData() => dataAccess.UpdateTaskList();
	private void UpdateView(bool updateHomeView)
	{
		TaskCollectionView.UpdateView(Group, OrderByAscending, ShowCompleted);
		DefaultMessageVisible = taskList.Count == 0 || !ShowCompleted && taskList.All(x => x.Completed && x.Subtasks.All(x => x.Completed));
		if (updateHomeView)
		{
			DataAccess.MainWindow.UpdateHomeView();
		}
	}
}