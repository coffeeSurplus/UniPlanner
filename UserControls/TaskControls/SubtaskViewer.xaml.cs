using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class SubtaskViewer : UserControl
	{
		public required TasksPage TasksPage { get; init; }
		public required Classes.Task Task { get; init; }
		public required Subtask Subtask { get; init; }

		private readonly Style completeButtonSmallStyle = (Style)Application.Current.FindResource("CompleteButtonSmall");
		private readonly Style strikethroughSubtitleStyle = (Style)Application.Current.FindResource("StrikethroughSubtitle");

		public SubtaskViewer() => InitializeComponent();
		public SubtaskViewer SetDisplay()
		{
			if (Subtask.Completed)
			{
				CompleteButton.Style = completeButtonSmallStyle;
				SubtaskText.Style = strikethroughSubtitleStyle;
			}

			SubtaskText.Text = Subtask.Title;
			return this;
		}

		private void CompleteSubtask()
		{
			Subtask.Completed ^= true;
			TasksPage.UpdateTaskList();
		}
		private void EditSubtask()
		{
			((SubtaskEditor)TasksPage.SubtaskEditorPopup.Child).SetDisplay(Subtask);
			TasksPage.SubtaskEditorPopup.IsOpen = true;
			((SubtaskEditor)TasksPage.SubtaskEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteSubtask()
		{
			Task.Subtasks.Remove(Subtask);
			TasksPage.UpdateTaskList();
		}
		private void CompleteButtonClick(object sender, RoutedEventArgs e) => CompleteSubtask();
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditSubtask();
		private void DeleteButtonClick(object sender, RoutedEventArgs e) => DeleteSubtask();

		private void ShowButtons()
		{
			EditButton.Visibility = Visibility.Visible;
			DeleteButton.Visibility = Visibility.Visible;
		}
		private void HideButtons()
		{
			EditButton.Visibility = Visibility.Collapsed;
			DeleteButton.Visibility = Visibility.Collapsed;
		}
		private void ShowButtonsEvent(object sender, MouseEventArgs e) => ShowButtons();
		private void HideButtonsEvent(object sender, MouseEventArgs e) => HideButtons();
	}
}