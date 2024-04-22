using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.EventControls
{
	public partial class WeekEventViewer : UserControl
	{
		public required EventsPage EventsPage { get; init; }
		public required Event Event { get; init; }

		public WeekEventViewer() => InitializeComponent();
		public WeekEventViewer SetDisplay()
		{
			Background = (Brush)FindResource($"{Event.Colour}");
			Margin = new Thickness(0, Event.StartTime != null ? ((TimeOnly)Event.StartTime).ToTimeSpan().TotalMinutes + 10 : 0, 0, 0);
			Height = Event.Length() != 0 ? Event.Length() : 50;
			VerticalAlignment = VerticalAlignment.Top;
			TitleText.Text = Event.Title;
			List<string> subtitleText = [];

			if (Event.Details != null)
				subtitleText.Add(Event.Details);

			if (Event.Location != null)
				subtitleText.Add(Event.Location);

			return this;
		}

		private void EditEvent()
		{
			((EventEditor)EventsPage.EventEditorPopup.Child).SetDisplay(Event);
			EventsPage.EventEditorPopup.IsOpen = true;
			((EventEditor)EventsPage.EventEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteEvent()
		{
			if (MessageBox.Show("Are you sure?", "Delete event", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
			{
				EventsPage.DataManager.EventList.Remove(Event);
				EventsPage.UpdateEventList();
			}
		}
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditEvent();
		private void DeleteButtonClick(object sender, MouseButtonEventArgs e) => DeleteEvent();
	}
}