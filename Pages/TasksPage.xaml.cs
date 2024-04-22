using System.Windows;
using System.Windows.Controls;
using UniPlanner.Classes;
using UniPlanner.UserControls.TaskControls;

namespace UniPlanner.Pages
{
	public partial class TasksPage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private List<Classes.Task> taskList = [];
		private int groupMode = 1;
		private bool ascending = true;
		public bool showCompleted = true;

		public TasksPage() => InitializeComponent();
		public TasksPage SetDisplay()
		{
			TaskEditorPopup.Child = new TaskEditor() { TasksPage = this };
			SubtaskEditorPopup.Child = new SubtaskEditor() { TasksPage = this, Task = new Classes.Task() { Title = string.Empty } };
			PdfEditorPopup.Child = new PdfEditor() { TasksPage = this, TaskList = taskList }.SetDisplay(taskList);
			taskList = DataManager.TaskList;
			UpdateView();
			return this;
		}

		public void CollapseGroup(string header, int mode)
		{
			foreach (var obj in TaskArea.Children)
				if (obj is TaskViewer taskViewer && header == mode switch
				{
					1 => taskViewer.Task.SubjectHeader(),
					2 => taskViewer.Task.DateHeader(),
					_ => taskViewer.Task.PriorityHeader()
				})
					taskViewer.Visibility = Visibility.Collapsed;
		}
		public void ExpandGroup(string header, int mode)
		{
			foreach (var obj in TaskArea.Children)
				if (obj is TaskViewer taskViewer && header == mode switch
				{
					1 => taskViewer.Task.SubjectHeader(),
					2 => taskViewer.Task.DateHeader(),
					_ => taskViewer.Task.PriorityHeader()
				})
					taskViewer.Visibility = Visibility.Visible;
		}
		public void UpdateTaskList()
		{
			DataManager.UpdateTaskList();
			taskList = DataManager.TaskList;
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}

		private void UpdateView()
		{
			OrderTaskList();
			UpdateTaskArea();
		}

		private void OrderTaskList()
		{
			switch (groupMode)
			{
				case 1:
					OrderBySubject();
					break;
				case 2:
					OrderByDate();
					break;
				case 3:
					OrderByPriority();
					break;
			}
			OrderSubtasks();
		}
		private void OrderBySubject()
		{
			taskList = ascending

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
		private void OrderByDate()
		{
			taskList = ascending

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
		private void OrderByPriority()
		{
			taskList = ascending

				? ([.. taskList
				.OrderBy(x => x.Priority == null)
				.ThenByDescending(x => x.Priority)
				.ThenBy(x => x.Completed)
				.ThenBy(x => x.Title)])

				: ([.. taskList
				.OrderByDescending(x => x.Priority == null)
				.ThenBy(x => x.Priority)
				.ThenBy(x => x.Completed)
				.ThenByDescending(x => x.Title)]);
		}
		private void OrderSubtasks()
		{
			foreach (Classes.Task task in taskList)
				task.Subtasks = [.. task.Subtasks.OrderBy(x => x.Completed).ThenBy(x => x.Title)];
		}

		private void UpdateTaskArea()
		{
			TaskArea.Children.RemoveRange(0, TaskArea.Children.Count);
			AddDefaultHeader();

			for (int index = 0; index < taskList.Count; index++)
			{
				AddHeader(index);
				AddTask(index);
			}
		}
		private void AddDefaultHeader()
		{
			if (taskList.Count == 0 || (!showCompleted && !taskList.Any(x => !x.Completed || x.Subtasks.Any(x => !x.Completed))))
				TaskArea.Children.Add(new TaskHeader() { TasksPage = this, GroupText = string.Empty, GroupMode = 0 });
		}
		private void AddHeader(int index)
		{
			string header = groupMode switch
			{
				1 => taskList[index].SubjectHeader(),
				2 => taskList[index].DateHeader(),
				_ => taskList[index].PriorityHeader(),
			};

			if ((index == 0 || header != groupMode switch
			{
				1 => taskList[index - 1].SubjectHeader(),
				2 => taskList[index - 1].DateHeader(),
				_ => taskList[index - 1].PriorityHeader()
			})
			&& (showCompleted || taskList.Any(x => header == groupMode switch
			{
				1 => x.SubjectHeader(),
				2 => x.DateHeader(),
				_ => x.PriorityHeader()
			}
			&& (!x.Completed || x.Subtasks.Any(x => !x.Completed)))))
				TaskArea.Children.Add(new TaskHeader()
				{
					TasksPage = this,
					GroupText = header,
					GroupMode = groupMode
				}.SetDisplay(ReturnTaskCount(header), ReturnSubtaskCount(header)));
		}
		private void AddTask(int index)
		{
			if (showCompleted || !taskList[index].Completed || taskList[index].Subtasks.Any(x => !x.Completed))
				TaskArea.Children.Add(new TaskViewer() { TasksPage = this, Task = taskList[index] }.SetDisplay());
		}

		private int ReturnTaskCount(string header)
		{
			return taskList.Count(x => !x.Completed && groupMode switch
			{
				1 => x.SubjectHeader() == header,
				2 => x.DateHeader() == header,
				_ => x.PriorityHeader() == header
			});
		}
		private int ReturnSubtaskCount(string header)
		{
			return groupMode switch
			{
				1 => taskList.Where(x => x.SubjectHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed)),
				2 => taskList.Where(x => x.DateHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed)),
				_ => taskList.Where(x => x.PriorityHeader() == header).Sum(x => x.Subtasks.Count(x => !x.Completed))
			};
		}

		private void NewTask()
		{
			((TaskEditor)TaskEditorPopup.Child).SetDefaultDisplay();
			TaskEditorPopup.IsOpen = true;
			((TaskEditor)TaskEditorPopup.Child).FocusKeyboard();
		}
		private void NewTaskButtonClick(object sender, RoutedEventArgs e) => NewTask();

		private void RemoveCompleted()
		{
			if (MessageBox.Show("Are you sure?", "Remove all completed", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
			{
				foreach (Classes.Task task in taskList)
					task.Subtasks.RemoveAll(x => x.Completed);

				taskList.RemoveAll(x => x.Completed && x.Subtasks.Count == 0);
				DataManager.TaskList = taskList;
				UpdateTaskList();
			}
		}
		private void RemoveOverdue()
		{
			if (MessageBox.Show("Are you sure?", "Remove all overdue", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
			{
				taskList.RemoveAll(x => x.Date != null && x.DateText() == "overdue");
				DataManager.TaskList = taskList;
				UpdateTaskList();
			}
		}
		private void RemoveCompletedButtonClick(object sender, RoutedEventArgs e) => RemoveCompleted();
		private void RemoveOverdueButtonClick(object sender, RoutedEventArgs e) => RemoveOverdue();

		private void ShowBySubject()
		{
			SubjectButton.Style = selectedButtonStyle;
			DateButton.Style = deselectedButtonStyle;
			PriorityButton.Style = deselectedButtonStyle;
			groupMode = 1;
			UpdateView();
		}
		private void ShowByDate()
		{
			SubjectButton.Style = deselectedButtonStyle;
			DateButton.Style = selectedButtonStyle;
			PriorityButton.Style = deselectedButtonStyle;
			groupMode = 2;
			UpdateView();
		}
		private void ShowByPriority()
		{
			SubjectButton.Style = deselectedButtonStyle;
			DateButton.Style = deselectedButtonStyle;
			PriorityButton.Style = selectedButtonStyle;
			groupMode = 3;
			UpdateView();
		}
		private void SubjectButtonClick(object sender, RoutedEventArgs e) => ShowBySubject();
		private void DateButtonClick(object sender, RoutedEventArgs e) => ShowByDate();
		private void PriorityButtonClick(object sender, RoutedEventArgs e) => ShowByPriority();

		private void ShowByAscending()
		{
			AscendingButton.Style = selectedButtonStyle;
			DescendingButton.Style = deselectedButtonStyle;
			ascending = true;
			UpdateView();
		}
		private void ShowByDescending()
		{
			AscendingButton.Style = deselectedButtonStyle;
			DescendingButton.Style = selectedButtonStyle;
			ascending = false;
			UpdateView();
		}
		private void AscendingButtonClick(object sender, RoutedEventArgs e) => ShowByAscending();
		private void DescendingButtonClick(object sender, RoutedEventArgs e) => ShowByDescending();

		private void ShowCompleted()
		{
			ShowCompletedButton.Style = selectedButtonStyle;
			HideCompletedButton.Style = deselectedButtonStyle;
			showCompleted = true;
			UpdateView();
		}
		private void HideCompleted()
		{
			ShowCompletedButton.Style = deselectedButtonStyle;
			HideCompletedButton.Style = selectedButtonStyle;
			showCompleted = false;
			UpdateView();
		}
		private void ShowCompletedButtonClick(object sender, RoutedEventArgs e) => ShowCompleted();
		private void HideCompletedButtonClick(object sender, RoutedEventArgs e) => HideCompleted();

		private void SaveAsPdf()
		{
			((PdfEditor)PdfEditorPopup.Child).SetDisplay(taskList);
			PdfEditorPopup.IsOpen = true;
		}
		private void SaveAsPdfButtonClick(object sender, RoutedEventArgs e) => SaveAsPdf();
	}
}