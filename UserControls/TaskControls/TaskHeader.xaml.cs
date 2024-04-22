using System.Windows;
using System.Windows.Controls;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class TaskHeader : UserControl
	{
		public required TasksPage TasksPage { get; init; }
		public required string GroupText { get; init; }
		public required int GroupMode { get; init; }

		public TaskHeader() => InitializeComponent();
		public TaskHeader SetDisplay(int taskCount, int subtaskCount)
		{
			HeaderText.Text = GroupText;
			List<string> subtitleText = [];

			if (taskCount != 0)
				subtitleText.Add($"{taskCount} {(taskCount == 1 ? "task" : "tasks")}");

			if (subtaskCount != 0)
				subtitleText.Add($"{subtaskCount} {(subtaskCount == 1 ? "subtask" : "subtasks")}");

			SubtitleText.Text = subtitleText.Count != 0 ? string.Join(" • ", subtitleText) : "completed";
			CollapseButton.Visibility = Visibility.Visible;
			return this;
		}

		private void CollapseHeader()
		{
			CollapseButton.Visibility = Visibility.Collapsed;
			ExpandButton.Visibility = Visibility.Visible;
			TasksPage.CollapseGroup(GroupText, GroupMode);
		}
		private void ExpandHeader()
		{
			CollapseButton.Visibility = Visibility.Visible;
			ExpandButton.Visibility = Visibility.Collapsed;
			TasksPage.ExpandGroup(GroupText, GroupMode);
		}
		private void CollapseButtonClick(object sender, RoutedEventArgs e) => CollapseHeader();
		private void ExpandButtonClick(object sende, RoutedEventArgs e) => ExpandHeader();
	}
}