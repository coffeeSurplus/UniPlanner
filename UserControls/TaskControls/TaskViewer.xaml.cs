using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class TaskViewer : UserControl
	{
		public required TasksPage TasksPage { get; init; }
		public required Classes.Task Task { get; init; }

		private readonly Style completeButtonStyle = (Style)Application.Current.FindResource("CompleteButton");
		private readonly Style strikethroughTitleStyle = (Style)Application.Current.FindResource("StrikethroughTitle");
		private readonly Style strikethroughSubtitleStyle = (Style)Application.Current.FindResource("StrikethroughSubtitle");

		public TaskViewer() => InitializeComponent();
		public TaskViewer SetDisplay()
		{
			if (Task.Completed)
			{
				TitleText.Text = Task.PriorityText() + Task.Title;
				CompleteButton.Style = completeButtonStyle;
				TitleText.Style = strikethroughTitleStyle;
				SubtitleText.Style = strikethroughSubtitleStyle;
			}
			else
			{
				PriorityText.Text = Task.PriorityText();
				TitleText.Text = Task.Title;
			}

			DisplaySubtitle();
			DisplaySubtasks();
			return this;
		}
		private void DisplaySubtitle()
		{
			List<string> subtitleText = [];

			if (Task.Date != null)
				subtitleText.Add(Task.DateTimeText());

			if (Task.Subject != null)
				subtitleText.Add(Task.Subject);

			if (Task.Details != null)
				subtitleText.Add(Task.Details);

			if (subtitleText.Count != 0)
				SubtitleText.Text = string.Join(" • ", subtitleText);
			else
				TextArea.Children.Remove(SubtitleText);
		}
		private void DisplaySubtasks()
		{
			if (Task.Subtasks.Count != 0)
			{
				foreach (Subtask subtask in Task.Subtasks)
				{
					if (TasksPage.showCompleted || !subtask.Completed)
						SubtaskArea.Children.Add(new SubtaskViewer() { TasksPage = TasksPage, Task = Task, Subtask = subtask }.SetDisplay());
				}
			}
		}

		private void CompleteTask()
		{
			Task.Completed ^= true;
			TasksPage.UpdateTaskList();
		}
		private void AddSubtask()
		{
			((SubtaskEditor)TasksPage.SubtaskEditorPopup.Child).SetDefaultDisplay(Task);
			TasksPage.SubtaskEditorPopup.IsOpen = true;
			((SubtaskEditor)TasksPage.SubtaskEditorPopup.Child).FocusKeyboard();
		}
		private void EditTask()
		{
			((TaskEditor)TasksPage.TaskEditorPopup.Child).SetDisplay(Task);
			TasksPage.TaskEditorPopup.IsOpen = true;
			((TaskEditor)TasksPage.TaskEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteTask()
		{
			TasksPage.DataManager.TaskList.Remove(Task);
			TasksPage.UpdateTaskList();
		}
		private void CompleteButtonClick(object sender, RoutedEventArgs e) => CompleteTask();
		private void AddButtonClick(object sender, RoutedEventArgs e) => AddSubtask();
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditTask();
		private void DeleteButtonClick(object sender, RoutedEventArgs e) => DeleteTask();

		private void ShowButtons()
		{
			AddButton.Visibility = Visibility.Visible;
			EditButton.Visibility = Visibility.Visible;
			DeleteButton.Visibility = Visibility.Visible;
		}
		private void HideButtons()
		{
			AddButton.Visibility = Visibility.Collapsed;
			EditButton.Visibility = Visibility.Collapsed;
			DeleteButton.Visibility = Visibility.Collapsed;
		}
		private void ShowButtonsEvent(object sender, MouseEventArgs e) => ShowButtons();
		private void HideButtonsEvent(object sender, MouseEventArgs e) => HideButtons();
	}
}