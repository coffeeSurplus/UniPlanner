using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using UniPlanner.Classes;
using UniPlanner.UserControls.EventControls;

namespace UniPlanner.Pages
{
	public partial class EventsPage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");
		private readonly Style horizontalGridlineStyle = (Style)Application.Current.FindResource("HorizontalGridline");

		private List<Event> eventList = [];

		private DateOnly DayDate = DefaultDate;
		private DateOnly WeekDate => DayDate.AddDays($"{DayDate:ddd}" switch
		{
			"Mon" => 0,
			"Tue" => -1,
			"Wed" => -2,
			"Thu" => -3,
			"Fri" => -4,
			"Sat" => -5,
			_ => -6
		});

		private DateOnly MonthDate => DayDate.AddDays(1 - DayDate.Day);

		private static DateOnly DefaultDate => DateOnly.FromDateTime(DateTime.Now);

		public EventsPage() => InitializeComponent();
		public EventsPage SetDisplay()
		{
			eventList = DataManager.EventList;
			EventEditorPopup.Child = new EventEditor() { EventsPage = this };
			UpdateView();
			return this;
		}
		public void UpdateEventList()
		{
			DataManager.UpdateEventList();
			eventList = DataManager.EventList;
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}
		public void UpdateTimeLine() => TimeLine.Margin = new Thickness(0, (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalMinutes + 10, 0, 0);

		private void UpdateView()
		{
			eventList = [.. eventList.OrderBy(x => x.Date).ThenBy(x => x.StartTime)];
			TodayButton.Style = DayDate == DefaultDate ? selectedButtonStyle : deselectedButtonStyle;
			UpdateMonthView();
			UpdateWeekView();
			UpdateDayView();
		}

		private void UpdateMonthView()
		{
			MonthHeader.Text = $"{MonthDate:MMMM yyyy}".ToLower();
			MonthArea.Children.RemoveRange(57, MonthArea.Children.Count - 57);
			SetMonthDateTexts();
			AddMonthEvents();
		}
		private void SetMonthDateTexts()
		{
			int dayAdjust = MonthDate.ToString("ddd") switch
			{
				"Mon" => 0,
				"Tue" => 1,
				"Wed" => 2,
				"Thu" => 3,
				"Fri" => 4,
				"Sat" => 5,
				_ => 6
			};

			int maximumDay = MonthDate.AddMonths(1).AddDays(-1).Day;

			for (int cell = 0; cell <= 41; cell++)
			{
				int date = MonthDate.Day + cell - dayAdjust;
				((TextBlock)MonthArea.Children[cell + 15]).Text = date > 0 && date <= maximumDay ? $"{date}" : string.Empty;

				((TextBlock)MonthArea.Children[cell + 15]).FontWeight = DateOnly.FromDateTime(DateTime.Now.AddDays(1 - DateTime.Now.Day)) == MonthDate && date == DateTime.Now.Day
					? FontWeights.SemiBold
					: FontWeights.Normal;
			}
		}
		private void AddMonthEvents()
		{
			for (int date = 0; date < MonthDate.AddMonths(1).AddDays(-1).Day; date++)
			{
				if (eventList.Any(x => x.Date == MonthDate.AddDays(date)))
				{
					StackPanel stackPanel = new() { Margin = new Thickness(0, 20, 0, 0) };
					Panel.SetZIndex(stackPanel, -1);

					foreach (Event @event in eventList.Where(x => x.Date == MonthDate.AddDays(date)))
						stackPanel.Children.Add(new MonthEventViewer() { EventsPage = this, Event = @event }.SetDisplay());

					Grid.SetColumn(stackPanel, $"{MonthDate.AddDays(date):ddd}" switch
					{
						"Mon" => 0,
						"Tue" => 1,
						"Wed" => 2,
						"Thu" => 3,
						"Fri" => 4,
						"Sat" => 5,
						_ => 6
					});

					Grid.SetRow(stackPanel, (MonthDate.AddDays(date).DayNumber - MonthDate.AddDays($"{MonthDate:ddd}" switch
					{
						"Mon" => 0,
						"Tue" => -1,
						"Wed" => -2,
						"Thu" => -3,
						"Fri" => -4,
						"Sat" => -5,
						_ => -6
					}).DayNumber) / 7);

					MonthArea.Children.Add(stackPanel);
				}
			}
		}

		private void UpdateWeekView()
		{
			WeekHeader.Text = $"{WeekDate:d MMMM} - {WeekDate.AddDays(6):d MMMM}".ToLower();
			WeekAllDayArea.Children.RemoveRange(1, WeekAllDayArea.Children.Count - 1);
			WeekArea.Children.RemoveRange(34, WeekArea.Children.Count - 34);
			SetWeekDayTextArea();
			SetWeekAllDayArea();
			AddWeekEvents();
			AddWeekHorizontalGridlines();
			UpdateTimeLine();
			WeekScrollViewer.ScrollToVerticalOffset(540);
		}
		private void SetWeekDayTextArea()
		{
			Grid.SetColumnSpan(WeekDayTextArea, DataManager.Settings.ScrollBars ? 1 : 2);

			foreach (TextBlock dayText in WeekDayTextArea.Children)
				dayText.FontWeight = DefaultDate.DayNumber - WeekDate.DayNumber is >= 0 and < 7 && dayText.Name == $"{DefaultDate:ddd}"
					? FontWeights.SemiBold
					: FontWeights.Normal;

			for (int day = 0; day < 7; day++)
				((TextBlock)WeekDayTextArea.Children[day]).Text = $"{WeekDate.AddDays(day):ddd d/M}".ToLower();
		}
		private void SetWeekAllDayArea()
		{
			if (eventList.Any(x => x.StartTime == null && x.Date.DayNumber - WeekDate.DayNumber is >= 0 and < 7))
			{
				WeekAllDayArea.Visibility = Visibility.Visible;
				Grid.SetRow(WeekScrollViewer, 3);
				Grid.SetColumnSpan(WeekAllDayArea, DataManager.Settings.ScrollBars ? 1 : 2);
			}
			else
			{
				WeekAllDayArea.Visibility = Visibility.Collapsed;
				Grid.SetRow(WeekScrollViewer, 2);
			}
		}
		private void AddWeekEvents()
		{
			foreach (Event @event in eventList.Where(x => x.Date.DayNumber - WeekDate.DayNumber is >= 0 and < 7))
			{
				WeekEventViewer eventViewer = new WeekEventViewer() { EventsPage = this, Event = @event }.SetDisplay();
				Grid.SetColumn(eventViewer, @event.Day());
				Panel.SetZIndex(eventViewer, -1);

				if (@event.StartTime != null)
					WeekArea.Children.Add(eventViewer);
				else
					WeekAllDayArea.Children.Add(eventViewer);
			}
		}
		private void AddWeekHorizontalGridlines()
		{
			for (int row = 10; row <= 1450; row += 60)
			{
				for (int column = 1; column <= 7; column++)
				{
					if (!eventList.Any(x => x.Date.DayNumber - WeekDate.DayNumber is >= 0 and < 7
							 && x.Day() == column
							 && x.StartTime != null
							 && ((TimeOnly)x.StartTime).ToTimeSpan().TotalMinutes + 10 < row
							 && ((TimeOnly)x.EndTime!).ToTimeSpan().TotalMinutes + 10 > row))
					{
						Rectangle horizontalGridline = new() { Style = horizontalGridlineStyle, Margin = new Thickness(0, row, 0, 0) };
						Grid.SetColumn(horizontalGridline, column);
						WeekArea.Children.Add(horizontalGridline);
					}
				}
			}
		}

		private void UpdateDayView()
		{
			DayHeader.Text = $"{DayDate:dddd d MMMM} - {eventList.Count(x => x.Date == DayDate)} {(eventList.Count(x => x.Date == DayDate) == 1 ? "event" : "events")}".ToLower();
			DayArea.Children.RemoveRange(0, DayArea.Children.Count);

			foreach (Event @event in eventList.Where(x => x.Date == DayDate))
				DayArea.Children.Add(new DayEventViewer() { EventsPage = this, Event = @event }.SetDisplay());
		}

		private void ViewToday()
		{
			DayDate = DefaultDate;
			UpdateView();
		}
		private void TodayButtonClick(object sender, RoutedEventArgs e) => ViewToday();

		private void ViewMonthPanel()
		{
			MonthButton.Style = selectedButtonStyle;
			WeekButton.Style = deselectedButtonStyle;
			DayButton.Style = deselectedButtonStyle;
			MonthPanel.Visibility = Visibility.Visible;
			WeekPanel.Visibility = Visibility.Collapsed;
			DayPanel.Visibility = Visibility.Collapsed;
		}
		private void ViewWeekPanel()
		{
			MonthButton.Style = deselectedButtonStyle;
			WeekButton.Style = selectedButtonStyle;
			DayButton.Style = deselectedButtonStyle;
			MonthPanel.Visibility = Visibility.Collapsed;
			WeekPanel.Visibility = Visibility.Visible;
			DayPanel.Visibility = Visibility.Collapsed;
		}
		private void ViewDayPanel()
		{
			MonthButton.Style = deselectedButtonStyle;
			WeekButton.Style = deselectedButtonStyle;
			DayButton.Style = selectedButtonStyle;
			MonthPanel.Visibility = Visibility.Collapsed;
			WeekPanel.Visibility = Visibility.Collapsed;
			DayPanel.Visibility = Visibility.Visible;
		}
		private void MonthButtonClick(object sender, RoutedEventArgs e) => ViewMonthPanel();
		private void WeekButtonClick(object sender, RoutedEventArgs e) => ViewWeekPanel();
		private void DayButtonClick(object sender, RoutedEventArgs e) => ViewDayPanel();

		private void AddEvent()
		{
			((EventEditor)EventEditorPopup.Child).SetDefaultDisplay();
			EventEditorPopup.IsOpen = true;
			((EventEditor)EventEditorPopup.Child).FocusKeyboard();
		}
		private void AddButtonClick(object sender, RoutedEventArgs e) => AddEvent();

		private void MonthBack()
		{
			DayDate = MonthDate.AddMonths(-1);
			UpdateView();
		}
		private void MonthForward()
		{
			DayDate = MonthDate.AddMonths(1);
			UpdateView();
		}
		private void MonthBackButtonClick(object sender, RoutedEventArgs e) => MonthBack();
		private void MonthForwardButtonClick(object sender, RoutedEventArgs e) => MonthForward();

		private void WeekBack()
		{
			DayDate = WeekDate.AddDays(-7);
			UpdateView();
		}
		private void WeekForward()
		{
			DayDate = WeekDate.AddDays(7);
			UpdateView();
		}
		private void WeekBackButtonClick(object sender, RoutedEventArgs e) => WeekBack();
		private void WeekForwardButtonClick(object sender, RoutedEventArgs e) => WeekForward();

		private void DayBack()
		{
			DayDate = DayDate.AddDays(-1);
			UpdateView();
		}
		private void DayForward()
		{
			DayDate = DayDate.AddDays(1);
			UpdateView();
		}
		private void DayBackButtonClick(object sender, RoutedEventArgs e) => DayBack();
		private void DayForwardButtonClick(object sender, RoutedEventArgs e) => DayForward();
	}
}