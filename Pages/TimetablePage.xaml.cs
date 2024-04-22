using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using UniPlanner.Classes;
using UniPlanner.UserControls.TimetableControls;

namespace UniPlanner.Pages
{
	public partial class TimetablePage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");
		private readonly Style horizontalGridlineStyle = (Style)Application.Current.FindResource("HorizontalGridline");

		private List<Timetable> timetableList = [];
		private int minTime;
		private int maxTime;

		public TimetablePage() => InitializeComponent();
		public TimetablePage SetDisplay()
		{
			timetableList = DataManager.TimetableList;
			TimetableEditorPopup.Child = new TimetableEditor() { TimetablePage = this };
			UpdateView();
			return this;
		}

		public void ExpandGroup(int day)
		{
			foreach (var obj in HorizontalTimetableArea.Children)
				if (obj is HorizontalTimetableViewer horizontalTimetableViewer && horizontalTimetableViewer.Timetable.Day == day)
					horizontalTimetableViewer.Visibility = Visibility.Visible;
		}
		public void CollapseGroup(int day)
		{
			foreach (var obj in HorizontalTimetableArea.Children)
				if (obj is HorizontalTimetableViewer horizontalTimetableViewer && horizontalTimetableViewer.Timetable.Day == day)
					horizontalTimetableViewer.Visibility = Visibility.Collapsed;
		}
		public void UpdateTimetableList()
		{
			DataManager.UpdateTimetableList();
			timetableList = DataManager.TimetableList;
			((MainWindow)Application.Current.MainWindow).UpdateDisplay();
		}
		public void UpdateTimeLine() => TimeLine.Margin = new Thickness(0, (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalMinutes - minTime + 10, 0, 0);

		private void UpdateView()
		{
			timetableList = [.. timetableList.OrderBy(x => x.Day).ThenBy(x => x.StartTime)];
			UpdateVerticalTimetableArea();
			UpdateHorizontalTimetableArea();
			UpdateTodayTimetableArea();
		}

		private void UpdateVerticalTimetableArea()
		{
			VerticalTimetableArea.Children.RemoveRange(7, VerticalTimetableArea.Children.Count - 7);
			minTime = timetableList.Select(x => x.StartTime.Hour * 60).Concat([540]).Min();
			maxTime = timetableList.Select(x => x.EndTime.Hour * 60 + (x.EndTime.Minute != 0 ? 60 : 0)).Concat([960]).Max();
			VerticalTimetableArea.Height = maxTime - minTime + 20;
			SetVerticalDayTextArea();
			AddVerticalimetabledEvents();
			AddVerticalTimes();
			AddVerticalHorizontalGridlines();
			UpdateTimeLine();
		}
		private void SetVerticalDayTextArea()
		{
			foreach (TextBlock dayText in VerticalDayTextArea.Children)
				dayText.FontWeight = $"{DateTime.Now:ddd}" == dayText.Name ? FontWeights.SemiBold : FontWeights.Normal;
		}
		private void AddVerticalimetabledEvents()
		{
			foreach (Timetable timetable in timetableList)
			{
				VerticalTimetableViewer timetableViewer = new VerticalTimetableViewer()
				{
					TimetablePage = this,
					Timetable = timetable
				}.SetDisplay(timetable.StartTime.ToTimeSpan().TotalMinutes - minTime + 10);

				Grid.SetColumn(timetableViewer, timetable.Day);
				Panel.SetZIndex(timetableViewer, -1);
				VerticalTimetableArea.Children.Add(timetableViewer);
			}
		}
		private void AddVerticalTimes()
		{
			for (int row = minTime; row <= maxTime; row += 60)
				VerticalTimetableArea.Children.Add(new TextBlock()
				{
					Text = $"{row / 60:00}:00",
					Margin = new Thickness(0, row - minTime, 0, 0),
					VerticalAlignment = VerticalAlignment.Top
				});
		}
		private void AddVerticalHorizontalGridlines()
		{
			for (int row = minTime; row <= maxTime; row += 60)
			{
				for (int column = 1; column <= 5; column++)
				{
					if (!timetableList.Any(x => x.Day == column
						&& x.StartTime.ToTimeSpan().TotalMinutes < row
						&& x.EndTime.ToTimeSpan().TotalMinutes > row))
					{
						Rectangle horizontalGridline = new()
						{
							Style = horizontalGridlineStyle,
							Margin = new Thickness(0, row - minTime + 10, 0, 0)
						};

						Grid.SetColumn(horizontalGridline, column);
						VerticalTimetableArea.Children.Add(horizontalGridline);
					}
				}
			}
		}

		private void UpdateHorizontalTimetableArea()
		{
			HorizontalTimetableArea.Children.RemoveRange(0, HorizontalTimetableArea.Children.Count);
			AddHorizontalDefaultHeader();
			AddHorizontalTimetabledEvents();
		}
		private void AddHorizontalDefaultHeader()
		{
			if (timetableList.Count == 0)
				HorizontalTimetableArea.Children.Add(new HorizontalTimetableHeader() { TimetablePage = this, Day = 0 }.SetDefaultDisplay());
		}
		private void AddHorizontalTimetabledEvents()
		{
			for (int day = 1; day <= 5; day++)
			{
				if (timetableList.Any(x => x.Day == day))
				{
					HorizontalTimetableArea.Children.Add(new HorizontalTimetableHeader() { TimetablePage = this, Day = day }.SetWeekDisplay());

					foreach (Timetable timetable in timetableList.Where(x => x.Day == day))
						HorizontalTimetableArea.Children.Add(new HorizontalTimetableViewer()
						{
							TimetablePage = this,
							Timetable = timetable
						}.SetDisplay());
				}
			}
		}

		private void UpdateTodayTimetableArea()
		{
			TodayTimetableArea.Children.RemoveRange(0, TodayTimetableArea.Children.Count);

			int today = $"{DateTime.Now:ddd}" switch
			{
				"Mon" => 1,
				"Tue" => 2,
				"Wed" => 3,
				"Thu" => 4,
				"Fri" => 5,
				_ => 0
			};
			AddTodayDefaultHeader(today);
			AddTodayTimetabledEvents(today);
		}
		private void AddTodayDefaultHeader(int today)
		{
			if (!timetableList.Any(x => x.Day == today))
				TodayTimetableArea.Children.Add(new HorizontalTimetableHeader() { TimetablePage = this, Day = today });
		}
		private void AddTodayTimetabledEvents(int today)
		{
			if (timetableList.Any(x => x.Day == today))
			{
				TodayTimetableArea.Children.Add(new HorizontalTimetableHeader() { TimetablePage = this, Day = today }.SetTodayDisplay());

				foreach (Timetable timetable in timetableList.Where(x => x.Day == today))
					TodayTimetableArea.Children.Add(new HorizontalTimetableViewer()
					{
						TimetablePage = this,
						Timetable = timetable
					}.SetDisplay());
			}
		}

		private void ShowVerticalPanel()
		{
			VerticalButton.Style = selectedButtonStyle;
			HorizontalButton.Style = deselectedButtonStyle;
			TodayButton.Style = deselectedButtonStyle;
			VerticalTimetablePanel.Visibility = Visibility.Visible;
			HorizontalTimetablePanel.Visibility = Visibility.Collapsed;
			TodayTimetablePanel.Visibility = Visibility.Collapsed;
		}
		private void ShowHorizontalPanel()
		{
			VerticalButton.Style = deselectedButtonStyle;
			HorizontalButton.Style = selectedButtonStyle;
			TodayButton.Style = deselectedButtonStyle;
			VerticalTimetablePanel.Visibility = Visibility.Collapsed;
			HorizontalTimetablePanel.Visibility = Visibility.Visible;
			TodayTimetablePanel.Visibility = Visibility.Collapsed;
		}
		private void ShowTodayPanel()
		{
			VerticalButton.Style = deselectedButtonStyle;
			HorizontalButton.Style = deselectedButtonStyle;
			TodayButton.Style = selectedButtonStyle;
			VerticalTimetablePanel.Visibility = Visibility.Collapsed;
			HorizontalTimetablePanel.Visibility = Visibility.Collapsed;
			TodayTimetablePanel.Visibility = Visibility.Visible;
		}
		private void VerticalButtonClick(object sender, RoutedEventArgs e) => ShowVerticalPanel();
		private void HorizontalButtonClick(object sender, RoutedEventArgs e) => ShowHorizontalPanel();
		private void TodayButtonClick(object sender, RoutedEventArgs e) => ShowTodayPanel();

		private void AddTimetable()
		{
			((TimetableEditor)TimetableEditorPopup.Child).SetDefaultDisplay();
			TimetableEditorPopup.IsOpen = true;
			((TimetableEditor)TimetableEditorPopup.Child).FocusKeyboard();
		}
		private void SaveAsPdf()
		{
			SaveFileDialog saveFileDialog = new()
			{
				Filter = "Pdf File (*.pdf)|*.pdf|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
				FileName = "UniPlanner timetable",
				DefaultExt = ".pdf"
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				File.WriteAllBytes(saveFileDialog.FileName, TimetableGenerator.GeneratePdf(timetableList));
				switch (DataManager.Settings.PdfSave)
				{
					case "default":
						Process.Start("explorer.exe", saveFileDialog.FileName);
						break;

					case "browser":
						Process.Start(new ProcessStartInfo()
						{
							UseShellExecute = true,
							FileName = DataManager.Settings.Browser,
							Arguments = DataManager.Settings.Arguments() + Uri.EscapeDataString(saveFileDialog.FileName)
						});
						break;

					case "explorer":
						Process.Start("explorer.exe", Directory.GetParent(saveFileDialog.FileName)!.FullName);
						break;
				}
			}
		}
		private void AddButtonClick(object sender, RoutedEventArgs e) => AddTimetable();
		private void SaveAsPdfButtonClick(object sender, RoutedEventArgs e) => SaveAsPdf();
	}
}