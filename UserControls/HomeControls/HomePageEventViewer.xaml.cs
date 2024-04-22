using System.Windows.Controls;
using System.Windows.Media;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.HomeControls
{
	public partial class HomePageEventViewer : UserControl
	{
		public required Event Event { get; init; }

		public HomePageEventViewer() => InitializeComponent();
		public HomePageEventViewer SetDisplay()
		{
			Background = (Brush)FindResource($"{Event.Colour}");
			TitleText.Text = Event.Title;
			List<string> subtitleText = [];
			subtitleText.Add(Event.ShortDateTime());

			if (Event.Details != null)
				subtitleText.Add(Event.Details);

			if (Event.Location != null)
				subtitleText.Add(Event.Location);

			SubtitleText.Text = string.Join(" • ", subtitleText);
			return this;
		}
	}
}
