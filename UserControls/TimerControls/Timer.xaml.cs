using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.TimerControls
{
	public partial class Timer : UserControl
	{
		public required DataManager DataManager { get; set; }

		private readonly System.Diagnostics.Stopwatch stopwatch = new();
		private readonly DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };
		private TimeSpan length = new();

		private SoundPlayer? alarm;

		public Timer() => InitializeComponent();
		public Timer SetDisplay()
		{
			SetStopStateButtons();
			SetLength(600);
			timer.Tick += DisplayTime;
			return this;
		}

		private void DisplayTime(object? sender, EventArgs e) => DisplayTime();
		private void DisplayTime()
		{
			TimeDisplay.Text = $"{length.Subtract(stopwatch.Elapsed):hh':'mm':'ss}";

			if (TimeDisplay.Text == "00:00:00")
			{
				timer.Stop();
				TimerFinished();
			}
		}

		private void TimerFinished()
		{
			alarm = new(Assembly.GetExecutingAssembly()
				.GetManifestResourceStream($"UniPlanner.Resources.Alarms.{DataManager.Settings.AlarmSound}.wav"));
			alarm.PlayLooping();
			EndTimerButton.Visibility = Visibility.Visible;
		}
		private void EndTimer()
		{
			alarm!.Stop();
			alarm.Dispose();
			stopwatch.Stop();
			stopwatch.Reset();
			SetStopStateButtons();
			DisplayTime();
		}
		private void EndTimerButtonClick(object sender, RoutedEventArgs e) => EndTimer();

		private void SetStartStateButtons()
		{
			EndTimerButton.Visibility = Visibility.Collapsed;
			StartButton.Visibility = Visibility.Collapsed;
			StopButton.Visibility = Visibility.Visible;
			ResetButton.Visibility = Visibility.Collapsed;
			RestartButton.Visibility = Visibility.Visible;
			TimerOptions.Visibility = Visibility.Collapsed;

			foreach (Button arrowbutton in ArrowUpButtons.Children)
				arrowbutton.Visibility = Visibility.Collapsed;

			foreach (Button arrowbutton in ArrowDownButtons.Children)
				arrowbutton.Visibility = Visibility.Collapsed;
		}
		private void SetStopStateButtons()
		{
			EndTimerButton.Visibility = Visibility.Collapsed;
			StartButton.Visibility = Visibility.Visible;
			StopButton.Visibility = Visibility.Collapsed;
			ResetButton.Visibility = Visibility.Visible;
			RestartButton.Visibility = Visibility.Collapsed;
			TimerOptions.Visibility = Visibility.Visible;

			foreach (Button arrowbutton in ArrowUpButtons.Children)
				arrowbutton.Visibility = Visibility.Visible;

			foreach (Button arrowbutton in ArrowDownButtons.Children)
				arrowbutton.Visibility = Visibility.Visible;
		}
		private void SetLengthButtons()
		{
			foreach (Button button in Buttons.Children)
				button.Style = (Style)Application.Current
					.FindResource(int.Parse(button.Uid) == length.TotalSeconds ? "SelectedButton" : "DeselectedButton");
		}

		private void StartTimer()
		{
			stopwatch.Start();
			timer.Start();
			SetStartStateButtons();
		}
		private void StopTimer()
		{
			stopwatch.Stop();
			timer.Stop();
			SetStopStateButtons();
		}
		private void ResetTimer()
		{
			stopwatch.Reset();
			DisplayTime();
		}
		private void RestartTimer() => stopwatch.Restart();
		private void StartButtonClick(object sender, RoutedEventArgs e) => StartTimer();
		private void StopButtonClick(object sender, RoutedEventArgs e) => StopTimer();
		private void ResetButtonClick(object sender, RoutedEventArgs e) => ResetTimer();
		private void RestartButtonClick(object sender, RoutedEventArgs e) => RestartTimer();

		private void SetLength(int seconds)
		{
			length = TimeSpan.FromSeconds(seconds);
			stopwatch.Reset();
			UpdateDisplay();
		}
		private void UpdateDisplay()
		{
			DisplayTime();
			SetLengthButtons();
		}
		private void UpdateLength()
		{
			length -= TimeSpan.FromSeconds(Math.Ceiling(stopwatch.Elapsed.TotalSeconds));
			stopwatch.Reset();
			DisplayTime();
		}
		private void LengthButtonClick(object sender, RoutedEventArgs e) => SetLength(int.Parse(((Button)sender).Uid));

		private void IncreaseHour()
		{
			UpdateLength();

			if (length.TotalHours != 23)
				length += length.Hours < 23 ? TimeSpan.FromHours(1) : TimeSpan.FromHours(-23);

			UpdateDisplay();
		}
		private void DecreaseHour()
		{
			UpdateLength();

			if (length.TotalHours != 1)
				length -= length.Hours > 0 ? TimeSpan.FromHours(1) : TimeSpan.FromHours(-23);

			UpdateDisplay();

		}
		private void IncreaseHourButtonClick(object sender, RoutedEventArgs e) => IncreaseHour();
		private void DecreaseHourButtonClick(object sender, RoutedEventArgs e) => DecreaseHour();

		private void IncreaseMinute()
		{
			UpdateLength();

			if (length.TotalMinutes != 59)
				length += length.Minutes < 59 ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(-59);

			UpdateDisplay();
		}
		private void DecreaseMinute()
		{
			UpdateLength();

			if (length.TotalMinutes != 1)
				length -= length.Minutes > 0 ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(-59);

			UpdateDisplay();
		}
		private void IncreaseMinuteButtonClick(object sender, RoutedEventArgs e) => IncreaseMinute();
		private void DecreaseMinuteButtonClick(object sender, RoutedEventArgs e) => DecreaseMinute();

		private void IncreaseSecond()
		{
			UpdateLength();

			if (length.TotalSeconds != 59)
				length += length.Seconds < 59 ? TimeSpan.FromSeconds(1) : TimeSpan.FromSeconds(-59);

			UpdateDisplay();
		}
		private void DecreaseSecond()
		{
			UpdateLength();

			if (length.TotalSeconds != 1)
				length -= length.Seconds > 0 ? TimeSpan.FromSeconds(1) : TimeSpan.FromSeconds(-59);

			UpdateDisplay();
		}
		private void IncreaseSecondButtonClick(object sender, RoutedEventArgs e) => IncreaseSecond();
		private void DecreaseSecondButtonClick(object sender, RoutedEventArgs e) => DecreaseSecond();
	}
}