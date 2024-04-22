using System.Windows.Controls;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.HomeControls
{
	public partial class HomePageTaskViewer : UserControl
	{
		public required Classes.Task Task { get; init; }

		public HomePageTaskViewer() => InitializeComponent();
		public HomePageTaskViewer SetDisplay()
		{
			PriorityText.Text = Task.PriorityText();
			TitleText.Text = Task.Title;
			List<string> subtitleText = [];

			if (Task.Date != null)
				subtitleText.Add(Task.ShortDate());

			if (Task.Subject != null)
				subtitleText.Add(Task.Subject);

			if (Task.Details != null)
				subtitleText.Add(Task.Details);

			if (subtitleText.Count != 0)
				SubtitleText.Text = string.Join(" • ", subtitleText);
			else
				TextArea.Children.Remove(SubtitleText);

			return this;
		}
	}
}