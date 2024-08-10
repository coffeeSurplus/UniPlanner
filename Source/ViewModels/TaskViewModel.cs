using System.Windows;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class TaskViewModel : ViewModelBase
{
	private readonly DataManager<List<TaskModel>> dataManager = ((App)Application.Current).TaskManager;
	private readonly List<TaskModel> taskList;
	private readonly TaskPdfGenerator pdfGenerator;
	private bool newTask = true;
	private bool newSubtask = true;
	private TaskModel currentTask = new();
	private SubtaskModel currentSubtask = new();

	private string group = "Subject";
	private bool orderByAscending = true;
	private bool showCompleted = true;
	private bool taskEditorVisible;
	private bool subtaskEditorVisible;
	private bool pdfEditorVisible;
	private bool defaultMessageVisible;
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
	public RelayCommand AddSubtaskCommand { get; }
	public RelayCommand EditTaskCommand { get; }
	public RelayCommand RemoveTaskCommand { get; }
	public RelayCommand CancelEditTaskCommand { get; }
	public RelayCommand SaveEditTaskCommand { get; }
	public RelayCommand SubtaskCompletedCommand { get; }
	public RelayCommand EditSubtaskCommand { get; }
	public RelayCommand RemoveSubtaskCommand { get; }
	public RelayCommand CancelEditSubtaskCommand { get; }
	public RelayCommand SaveEditSubtaskCommand { get; }
	public RelayCommand CancelEditPdfCommand { get; }
	public RelayCommand SaveEditPdfCommand { get; }

	public TaskCollectionView TaskCollectionView { get; }

	public TaskViewModel()
	{
		UpdateViewCommand = new(UpdateView);
		NewTaskCommand = new(NewTask);
		RemoveCompletedCommand = new(RemoveCompleted);
		RemoveOverdueCommand = new(RemoveOverdue);
		SaveAsPdfCommand = new(SaveAsPdf);
		TaskCompletedCommand = new(TaskCompleted);
		AddSubtaskCommand = new(AddSubtask);
		EditTaskCommand = new(EditTask);
		RemoveTaskCommand = new(RemoveTask);
		CancelEditTaskCommand = new(CancelEditTask);
		SaveEditTaskCommand = new(SaveEditTask);
		SubtaskCompletedCommand = new(SubtaskCompleted);
		EditSubtaskCommand = new(EditSubtask);
		RemoveSubtaskCommand = new(RemoveSubtask);
		CancelEditSubtaskCommand = new(CancelEditSubtask);
		SaveEditSubtaskCommand = new(SaveEditSubtask);
		CancelEditPdfCommand = new(CancelEditPdf);
		SaveEditPdfCommand = new(SaveEditPdf);
		taskList = dataManager.Data;
		pdfGenerator = new(taskList);
		TaskCollectionView = new(taskList);
		UpdateView(false);
	}

	private void UpdateView(object parameter) => UpdateView();
	private void NewTask(object parameter)
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
	private void RemoveCompleted(object parameter)
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
			UpdateView();
			List<string> message = [];
			if (taskRemovedCount > 0)
			{
				message.Add($"{taskRemovedCount} {(taskRemovedCount != 1 ? "tasks" : "task")}");
			}
			if (subtaskRemovedCount > 0)
			{
				message.Add($"{subtaskRemovedCount} {(subtaskRemovedCount != 1 ? "subtasks" : "subtask")}");
			}
			MessageBox.Show($"{string.Join(" and ", message)} removed.", "UniPlanner");
		}
	}
	private void RemoveOverdue(object parameter)
	{
		int removedCount = taskList.RemoveAll(x => x.Date.CompareTo(DateOnly.FromDateTime(DateTime.Now)) < 0);
		if (removedCount > 0)
		{
			UpdateData();
			UpdateView();
			MessageBox.Show($"{removedCount} {(removedCount != 1 ? "tasks" : "task")} removed.", "UniPlanner");
		}
	}
	private void SaveAsPdf(object parameter) => PdfEditorVisible = true;
	private void TaskCompleted(object parameter)
	{
		UpdateData();
		UpdateView();
	}
	private void AddSubtask(object parameter)
	{
		newSubtask = true;
		currentTask = (TaskModel)parameter;
		currentSubtask = new();
		CurrentSubtaskTitle = string.Empty;
		SubtaskEditorVisible = true;
	}
	private void EditTask(object parameter)
	{
		newTask = false;
		currentTask = (TaskModel)parameter;
		CurrentTaskTitle = currentTask.Title;
		CurrentTaskDetails = currentTask.Details ?? string.Empty;
		CurrentTaskSubject = currentTask.Subject ?? string.Empty;
		CurrentTaskDate = currentTask.Date != DateOnly.MaxValue ? currentTask.Date.ToString("d/M") : string.Empty;
		CurrentTaskPriority = currentTask.Priority;
		TaskEditorVisible = true;
	}
	private void RemoveTask(object parameter)
	{
		taskList.Remove((TaskModel)parameter);
		UpdateData();
		UpdateView();
	}
	private void CancelEditTask(object parameter) => TaskEditorVisible = false;
	private void SaveEditTask(object parameter)
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
			UpdateView();
		}
	}
	private void SubtaskCompleted(object parameter)
	{
		UpdateData();
		UpdateView();
	}
	private void EditSubtask(object parameter)
	{
		newSubtask = false;
		currentSubtask = (SubtaskModel)parameter;
		CurrentSubtaskTitle = currentSubtask.Title;
		SubtaskEditorVisible = true;
	}
	private void RemoveSubtask(object parameter)
	{
		((Tuple<TaskModel, SubtaskModel>)parameter).Item1.Subtasks.Remove(((Tuple<TaskModel, SubtaskModel>)parameter).Item2);
		UpdateData();
		UpdateView();
	}
	private void CancelEditSubtask(object parameter) => SubtaskEditorVisible = false;
	private void SaveEditSubtask(object parameter)
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
			UpdateView();
		}
	}
	private void CancelEditPdf(object parameter) => PdfEditorVisible = false;
	private void SaveEditPdf(object parameter)
	{
		pdfGenerator.UpdateValues(new(PdfGroup, PdfOrderByAscending, PdfShowCompleted));
		pdfGenerator.SavePdf();
		PdfEditorVisible = false;
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
				MessageBox.Show("Invalid date.", "UniPlanner");
			}
		}
		else
		{
			MessageBox.Show("Invalid title.", "UniPlanner");
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
			MessageBox.Show("Invalid title.", "UniPlanner");
			SubtaskEditorVisible = true;
			return false;
		}
	}
	private void UpdateData() => dataManager.UpdateData();
	private void UpdateView(bool updateHomeView = true)
	{
		TaskCollectionView.UpdateView(Group, OrderByAscending, ShowCompleted);
		DefaultMessageVisible = taskList.Count == 0 || !ShowCompleted && taskList.All(x => x.Completed && x.Subtasks.All(x => x.Completed));
		if (updateHomeView)
		{
			((App)Application.Current).UpdateHomeView();
		}
	}
}