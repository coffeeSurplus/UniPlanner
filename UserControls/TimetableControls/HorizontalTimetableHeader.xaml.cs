using System.Windows;
using System.Windows.Controls;
using UniPlanner.Pages;

namespace UniPlanner.UserControls.TimetableControls
{
	public partial class HorizontalTimetableHeader : UserControl
	{
		public required TimetablePage TimetablePage { get; init; }
		public required int Day { get; init; }

		public HorizontalTimetableHeader() => InitializeComponent();
		public HorizontalTimetableHeader SetWeekDisplay()
		{
			HeaderText.Text = Day switch
			{
				1 => "monday",
				2 => "tuesday",
				3 => "wednesday",
				4 => "thursday",
				_ => "friday"
			};

			int timetableCount = TimetablePage.DataManager.TimetableList.Count(x => x.Day == Day);
			SubtitleText.Text = $"{timetableCount} {(timetableCount == 1 ? "event" : "events")}";
			CollapseButton.Visibility = Visibility.Visible;
			return this;
		}
		public HorizontalTimetableHeader SetDefaultDisplay()
		{
			HeaderText.Text = "you have no timetabled events added.";
			return this;
		}
		public HorizontalTimetableHeader SetTodayDisplay()
		{
			HeaderText.Text = $"{DateTime.Now:dddd d MMMM}".ToLower();
			int timetableCount = TimetablePage.DataManager.TimetableList.Count(x => x.Day == Day);
			SubtitleText.Text = $"{timetableCount} {(timetableCount == 1 ? "event" : "events")}";
			return this;
		}

		private void CollapseHeader()
		{
			CollapseButton.Visibility = Visibility.Collapsed;
			ExpandButton.Visibility = Visibility.Visible;
			TimetablePage.CollapseGroup(Day);
		}
		private void ExpandHeader()
		{
			CollapseButton.Visibility = Visibility.Visible;
			ExpandButton.Visibility = Visibility.Collapsed;
			TimetablePage.ExpandGroup(Day);
		}
		private void CollapseButtonClick(object sender, RoutedEventArgs e) => CollapseHeader();
		private void ExpandButtonClick(object sender, RoutedEventArgs e) => ExpandHeader();
	}
}