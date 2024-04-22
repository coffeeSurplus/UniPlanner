using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.EventControls
{
	public partial class MonthEventViewer : UserControl
	{
		public required EventsPage EventsPage { get; init; }
		public required Event Event { get; init; }

		public MonthEventViewer() => InitializeComponent();
		public MonthEventViewer SetDisplay()
		{
			Background = (Brush)FindResource($"{Event.Colour}");
			TitleText.Text = Event.Title;
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