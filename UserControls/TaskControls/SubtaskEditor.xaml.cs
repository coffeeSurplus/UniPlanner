using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class SubtaskEditor : UserControl
	{
		public required TasksPage TasksPage { get; init; }
		public required Classes.Task Task { get; set; }

		private Subtask Subtask = new() { Title = string.Empty };
		private bool newSubtask = false;

		public SubtaskEditor() => InitializeComponent();
		public void SetDefaultDisplay(Classes.Task task)
		{
			Task = task;
			Subtask = new() { Title = string.Empty };
			SubtaskInput.Text = string.Empty;
			newSubtask = true;
		}
		public void SetDisplay(Subtask task)
		{
			Subtask = task;
			SubtaskInput.Text = task.Title;
			newSubtask = false;
		}
		public void FocusKeyboard() => Keyboard.Focus(SubtaskInput);

		private void CancelEdit() => TasksPage.SubtaskEditorPopup.IsOpen = false;
		private bool CheckInputs()
		{
			if (string.IsNullOrWhiteSpace(SubtaskInput.Text))
			{
				MessageBox.Show("Subtask title must not be empty.", "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				TasksPage.SubtaskEditorPopup.IsOpen = true;
				return false;
			}
			return true;
		}
		private void SaveSubtask()
		{
			if (CheckInputs())
			{
				Subtask.SetValues(SubtaskInput.Text);

				if (newSubtask)
					Task.Subtasks.Add(Subtask);

				TasksPage.UpdateTaskList();
				TasksPage.SubtaskEditorPopup.IsOpen = false;
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelEdit();
		private void SaveButtonClick(object sender, RoutedEventArgs e) => SaveSubtask();
	}
}