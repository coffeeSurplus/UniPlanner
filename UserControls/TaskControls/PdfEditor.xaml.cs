using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TaskControls
{
	public partial class PdfEditor : UserControl
	{
		public required TasksPage TasksPage { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		private int groupMode;
		private bool ascending;
		private bool includeCompleted;

		public required List<Classes.Task> TaskList { get; set; }

		public PdfEditor() => InitializeComponent();
		public PdfEditor SetDisplay(List<Classes.Task> taskList)
		{
			TaskList = taskList;
			SetGroupButtons(SubjectButton);
			SetOrderButtons(AscendingButton);
			SetCompletedTasksButtons(IncludeButton);
			return this;
		}

		private void SetGroupButtons(Button selectedButton)
		{
			foreach (Button button in GroupButtons.Children)
				button.Style = deselectedButtonStyle;

			selectedButton.Style = selectedButtonStyle;

			groupMode = selectedButton.Name switch
			{
				"DateButton" => 2,
				"PriorityButton" => 3,
				_ => 1
			};
		}
		private void GroupButtonClick(object sender, RoutedEventArgs e) => SetGroupButtons((Button)sender);

		private void SetOrderButtons(Button selectedButton)
		{
			foreach (Button button in DateButtons.Children)
				button.Style = deselectedButtonStyle;

			selectedButton.Style = selectedButtonStyle;
			ascending = selectedButton.Name == "AscendingButton";
		}
		private void OrderButtonClick(object sender, RoutedEventArgs e) => SetOrderButtons((Button)sender);

		private void SetCompletedTasksButtons(Button selectedButton)
		{
			foreach (Button button in CompletedTasksButtons.Children)
				button.Style = deselectedButtonStyle;

			selectedButton.Style = selectedButtonStyle;
			includeCompleted = selectedButton.Name == "IncludeButton";
		}
		private void CompletedTasksButtonClick(object sender, RoutedEventArgs e) => SetCompletedTasksButtons((Button)sender);

		private void CancelPdf() => TasksPage.PdfEditorPopup.IsOpen = false;
		private void GeneratePdf()
		{
			SaveFileDialog saveFileDialog = new()
			{
				Filter = "Pdf File (*.pdf)|*.pdf|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
				FileName = "UniPlanner tasks",
				DefaultExt = ".pdf"
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				File.WriteAllBytes(saveFileDialog.FileName, TaskGenerator.GeneratePdf(new(TaskList), groupMode, ascending, includeCompleted));
				switch (TasksPage.DataManager.Settings.PdfSave)
				{
					case "default":
						Process.Start("explorer.exe", saveFileDialog.FileName);
						break;

					case "browser":
						Process.Start(new ProcessStartInfo()
						{
							UseShellExecute = true,
							FileName = TasksPage.DataManager.Settings.Browser,
							Arguments = TasksPage.DataManager.Settings.Arguments() + Uri.EscapeDataString(saveFileDialog.FileName)
						});
						break;

					case "explorer":
						Process.Start("explorer.exe", Directory.GetParent(saveFileDialog.FileName)!.FullName);
						break;
				}
			}
		}
		private void CancelButtonClick(object sender, RoutedEventArgs e) => CancelPdf();
		private void GeneratePdfButtonClick(object sender, RoutedEventArgs e) => GeneratePdf();
	}
}