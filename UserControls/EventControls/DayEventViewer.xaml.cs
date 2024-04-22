using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.EventControls
{
	public partial class DayEventViewer : UserControl
	{
		public required EventsPage EventsPage { get; init; }
		public required Event Event { get; init; }

		public DayEventViewer() => InitializeComponent();
		public DayEventViewer SetDisplay()
		{
			Background = (Brush)FindResource($"{Event.Colour}");
			TitleText.Text = $"{(Event.StartTime != null ? $"{Event.StartTime:H:mm} - {Event.EndTime:H:mm}" : "all day")} • {Event.Title}";
			List<string> subtitleText = [];
			if (Event.Details != null)
				subtitleText.Add(Event.Details);
			if (Event.Location != null)
				subtitleText.Add(Event.Location);
			if (subtitleText.Count != 0)
				SubtitleText.Text = string.Join(" • ", subtitleText);
			else
				TextArea.Children.Remove(SubtitleText);
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
			EventsPage.DataManager.EventList.Remove(Event);
			EventsPage.UpdateEventList();
		}
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditEvent();
		private void DeleteButtonClick(object sender, RoutedEventArgs e) => DeleteEvent();

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