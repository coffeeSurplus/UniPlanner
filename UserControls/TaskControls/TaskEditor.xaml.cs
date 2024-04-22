using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class TaskEditor : UserControl
	{
		public required TasksPage TasksPage { get; init; }

		private readonly Style SelectedColourButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style DeselectedColourButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private Classes.Task task = new() { Title = string.Empty };
		private int? selectedPriority;
		private bool newTask = false;

		public TaskEditor() => InitializeComponent();
		public void SetDefaultDisplay()
		{
			task = new() { Title = string.Empty };
			TaskInput.Text = string.Empty;
			DetailsInput.Text = null;
			SubjectInput.Text = null;
			DateInput.Text = null;
			TimeInput.Text = null;
			selectedPriority = null;
			newTask = true;
			SetPriorityButtons(NoPriorityButton);
		}
		public void SetDisplay(Classes.Task task)
		{
			this.task = task;
			TaskInput.Text = task.Title;
			DetailsInput.Text = task.Details;
			SubjectInput.Text = task.Subject;
			DateInput.Text = task.Date != null ? $"{(DateOnly)task.Date:d/M/yy}" : string.Empty;
			TimeInput.Text = task.Time != null ? $"{(TimeOnly)task.Time:H:mm}" : string.Empty;
			selectedPriority = task.Priority;
			newTask = false;

			SetPriorityButtons(selectedPriority switch
			{
				1 => LowPriorityButton,
				2 => MediumPriorityButton,
				3 => HighPriorityButton,
				_ => NoPriorityButton
			});
		}
		public void FocusKeyboard() => Keyboard.Focus(TaskInput);

		private void SetPriorityButtons(Button selectedButton)
		{
			foreach (Button button in PriorityButtons.Children)
				button.Style = DeselectedColourButtonStyle;

			selectedButton.Style = SelectedColourButtonStyle;

			selectedPriority = selectedButton.Name switch
			{
				"LowPriorityButton" => 1,
				"MediumPriorityButton" => 2,
				"HighPriorityButton" => 3,
				_ => null
			};
		}
		private void PriorityButtonClick(object sender, RoutedEventArgs e) => SetPriorityButtons((Button)sender);

		private void CancelEdit() => TasksPage.TaskEditorPopup.IsOpen = false;
		private bool CheckInputs()
		{
			List<string> errorList = [];

			if (string.IsNullOrWhiteSpace(TaskInput.Text))
				errorList.Add("Task title must not be empty.");

			if (!string.IsNullOrWhiteSpace(DateInput.Text) && !DateOnly.TryParse(DateInput.Text, out _))
				errorList.Add("Invalid date input.");

			if (!string.IsNullOrWhiteSpace(TimeInput.Text) && !TimeOnly.TryParse(TimeInput.Text, out _))
				errorList.Add("Invalid time input.");

			if (errorList.Count != 0)
			{
				MessageBox.Show(string.Join('\n', errorList), "UniPlanner", MessageBoxButton.OK, MessageBoxImage.Error);
				TasksPage.TaskEditorPopup.IsOpen = true;
			}

			return errorList.Count == 0;
		}
		private void SaveTask()
		{
			if (CheckInputs())
			{
				task.SetValues(TaskInput.Text, DetailsInput.Text, SubjectInput.Text, DateInput.Text, TimeInput.Text, selectedPriority);

				if (newTask)
					TasksPage.DataManager.TaskList.Add(task);

				TasksPage.UpdateTaskList();
				TasksPage.TaskEditorPopup.IsOpen = false;
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelEdit();
		private void SaveButtonClick(object sender, RoutedEventArgs e) => SaveTask();
	}
}