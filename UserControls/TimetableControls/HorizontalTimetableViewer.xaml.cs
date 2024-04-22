using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UniPlanner.Classes;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TimetableControls
{
	public partial class HorizontalTimetableViewer : UserControl
	{
		public required TimetablePage TimetablePage { get; init; }
		public required Timetable Timetable { get; init; }

		public HorizontalTimetableViewer() => InitializeComponent();
		public HorizontalTimetableViewer SetDisplay()
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

		private void EditTimetable()
		{
			((TimetableEditor)TimetablePage.TimetableEditorPopup.Child).SetDisplay(Timetable);
			TimetablePage.TimetableEditorPopup.IsOpen = true;
			((TimetableEditor)TimetablePage.TimetableEditorPopup.Child).FocusKeyboard();
		}
		private void DeleteTimetable()
		{
			TimetablePage.DataManager.TimetableList.Remove(Timetable);
			TimetablePage.UpdateTimetableList();
		}
		private void EditButtonClick(object sender, RoutedEventArgs e) => EditTimetable();
		private void DeleteButtonClick(object sender, RoutedEventArgs e) => DeleteTimetable();

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
