using System.Windows;
using System.Windows.Controls;
using UniPlanner.Classes;
using UniPlanner.UserControls.TimerControls;

namespace UniPlanner.Pages
{
	public partial class TimersPage : Page
	{
		public required DataManager DataManager { get; init; }

		private readonly Style selectedButtonStyle = (Style)Application.Current.FindResource("SelectedButton");
		private readonly Style deselectedButtonStyle = (Style)Application.Current.FindResource("DeselectedButton");

		public TimersPage() => InitializeComponent();
		public TimersPage SetDisplay()
		{
			StopwatchPanel.Child = new Stopwatch().SetDisplay();
			TimerPanel.Child = new UserControls.TimerControls.Timer() { DataManager = DataManager }.SetDisplay();
			PomodoroPanel.Child = new Pomodoro() { DataManager = DataManager }.SetDisplay();
			return this;
		}

		private void ShowStopwatchPanel()
		{
			StopwatchButton.Style = selectedButtonStyle;
			TimerButton.Style = deselectedButtonStyle;
			PomodoroButton.Style = deselectedButtonStyle;
			StopwatchPanel.Visibility = Visibility.Visible;
			TimerPanel.Visibility = Visibility.Collapsed;
			PomodoroPanel.Visibility = Visibility.Collapsed;
		}
		private void ShowTimerPanel()
		{
			StopwatchButton.Style = deselectedButtonStyle;
			TimerButton.Style = selectedButtonStyle;
			PomodoroButton.Style = deselectedButtonStyle;
			StopwatchPanel.Visibility = Visibility.Collapsed;
			TimerPanel.Visibility = Visibility.Visible;
			PomodoroPanel.Visibility = Visibility.Collapsed;
		}
		private void ShowPomodoroPanel()
		{
			StopwatchButton.Style = deselectedButtonStyle;
			TimerButton.Style = deselectedButtonStyle;
			PomodoroButton.Style = selectedButtonStyle;
			StopwatchPanel.Visibility = Visibility.Collapsed;
			TimerPanel.Visibility = Visibility.Collapsed;
			PomodoroPanel.Visibility = Visibility.Visible;
		}
		private void StopwatchButtonClick(object sender, RoutedEventArgs e) => ShowStopwatchPanel();
		private void TimerButtonClick(object sender, RoutedEventArgs e) => ShowTimerPanel();
		private void PomodoroButtonClick(object sender, RoutedEventArgs e) => ShowPomodoroPanel();
	}
}