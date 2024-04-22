using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UniPlanner.UserControls.TimerControls
{
	public partial class Stopwatch : UserControl
	{
		private readonly System.Diagnostics.Stopwatch stopwatch = new();
		private readonly DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };

		public Stopwatch() => InitializeComponent();
		public Stopwatch SetDisplay()
		{
			timer.Tick += DisplayTime;
			return this;
		}

		private void DisplayTime(object? sender, EventArgs e) => TimeDisplay.Text = $"{stopwatch.Elapsed:mm':'ss'.'ff}";

		private void SetStartStateButtons()
		{
			StartButton.Visibility = Visibility.Collapsed;
			StopButton.Visibility = Visibility.Visible;
			ResetButton.Visibility = Visibility.Collapsed;
			RestartButton.Visibility = Visibility.Visible;
		}
		private void SetStopStateButtons()
		{
			StartButton.Visibility = Visibility.Visible;
			StopButton.Visibility = Visibility.Collapsed;
			ResetButton.Visibility = Visibility.Visible;
			RestartButton.Visibility = Visibility.Collapsed;
		}

		private void StartStopwatch()
		{
			stopwatch.Start();
			timer.Start();
			SetStartStateButtons();
		}
		private void StopStopwatch()
		{
			stopwatch.Stop();
			timer.Stop();
			SetStopStateButtons();
		}
		private void ResetStopwatch()
		{
			stopwatch.Reset();
			TimeDisplay.Text = "00:00.00";
		}
		private void RestartStopwatch() => stopwatch.Restart();
		private void StartButtonClick(object sender, RoutedEventArgs e) => StartStopwatch();
		private void StopButtonClick(object sender, RoutedEventArgs e) => StopStopwatch();
		private void ResetButtonClick(object sender, RoutedEventArgs e) => ResetStopwatch();
		private void RestartButtonClick(object sender, RoutedEventArgs e) => RestartStopwatch();
	}
}