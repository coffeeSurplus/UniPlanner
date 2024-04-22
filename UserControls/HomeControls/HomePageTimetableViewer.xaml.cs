using System.Windows.Controls;
using System.Windows.Media;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.HomeControls
{
	public partial class HomePageTimetableViewer : UserControl
	{
		public required Timetable Timetable { get; init; }

		public HomePageTimetableViewer() => InitializeComponent();
		public HomePageTimetableViewer SetDisplay()
		{
			Background = (Brush)FindResource($"{Timetable.Colour}");
			TitleText.Text = $"{Timetable.StartTime:H:mm} - {Timetable.EndTime:H:mm} • {Timetable.Title}";
			List<string> subtitleText = [];

			if (Timetable.Details != null)
				subtitleText.Add(Timetable.Details);

			if (Timetable.Subject != null)
				subtitleText.Add(Timetable.Subject);

			if (Timetable.Location != null)
				subtitleText.Add(Timetable.Location);

			if (subtitleText.Count != 0)
				SubtitleText.Text = string.Join(" • ", subtitleText);
			else
				TextArea.Children.Remove(SubtitleText);

			return this;
		}
	}
}