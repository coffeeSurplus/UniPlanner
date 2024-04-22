using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TimetableControls
{
	public partial class VerticalTimetableViewer : UserControl
	{
		public required TimetablePage TimetablePage { get; init; }
		public required Timetable Timetable { get; init; }

		public VerticalTimetableViewer() => InitializeComponent();
		public VerticalTimetableViewer SetDisplay(double margin)
		{
			Background = (Brush)FindResource($"{Timetable.Colour}");
			Margin = new Thickness(0, margin, 0, 0);
			Height = Timetable.Length();
			VerticalAlignment = VerticalAlignment.Top;
			TitleText.Text = Timetable.Title;
			List<string> subtitleText = [];

			if (Timetable.Details != null)
				subtitleText.Add(Timetable.Details);

			if (Timetable.Subject != null)
				subtitleText.Add(Timetable.Subject);

			if (Timetable.Location != null)
				subtitleText.Add(Timetable.Location);

			if (subtitleText.Count != 0 && Timetable.Length() >= 30)
				SubtitleText.Text = string.Join(" • ", subtitleText);
			else
				TextArea.Children.Remove(SubtitleText);

			return this;
		}

		private void EditTimetable()
		{
			((TimetableEditor)TimetablePage.TimetableEditorPopup.Child).SetDisplay(Timetable);
			TimetablePage.TimetableEditorPopup.IsOpen = true;
			((TimetableEditor)TimetablePage.TimetableEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteTimetable()
		{
			if (MessageBox.Show("Are you sure?", "Delete event", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
			{
				TimetablePage.DataManager.TimetableList.Remove(Timetable);
				TimetablePage.UpdateTimetableList();
			}
		}
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditTimetable();
		private void DeleteButtonClick(object sender, MouseButtonEventArgs e) => DeleteTimetable();
	}
}