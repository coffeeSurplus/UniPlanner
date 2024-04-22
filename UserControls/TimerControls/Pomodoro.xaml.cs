using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniPlanner.Classes;

namespace UniPlanner.UserControls.TimerControls
{
	public partial class Pomodoro : UserControl
	{
		public required DataManager DataManager { get; init; }

		private readonly System.Diagnostics.Stopwatch stopwatch = new();
		private readonly DispatcherTimer timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 10) };

		private SoundPlayer? alarm = new();

		private TimeSpan length;
		private TimeSpan pomodoroLength;
		private TimeSpan breakLength;

		private int repeat;
		private int repeatCount;
		private bool onBreak;

		public Pomodoro() => InitializeComponent();
		public Pomodoro SetDisplay()
		{
			SetStopStateButtons();
			SetDefaultValues();
			SetPomodoroText();
			SetBreakText();
			SetRepeatText();
			SetModeText();
			timer.Tick += DisplayTime;
			return this;
		}
		private void SetDefaultValues()
		{
			length = TimeSpan.FromMinutes(25);
			pomodoroLength = TimeSpan.FromMinutes(25);
			breakLength = TimeSpan.FromMinutes(5);
			repeat = 1;
			repeatCount = 1;
			onBreak = false;
			stopwatch.Reset();
		}

		private void DisplayTime(object? sender, EventArgs e) => DisplayTime();
		private void DisplayTime()
		{
			TimeDisplay.Text = length.Subtract(stopwatch.Elapsed).TotalHours >= 1
				? $"{length.Subtract(stopwatch.Elapsed):hh':'mm':'ss}"
				: $"{length.Subtract(stopwatch.Elapsed):mm':'ss}";

			if (TimeDisplay.Text == "00:00")
			{
				timer.Stop();
				TimerFinished();
			}
		}
		private void TimerFinished()
		{
			alarm = new(Assembly.GetExecutingAssembly()
				.GetManifestResourceStream($"UniPlanner.Resources.Alarms.{DataManager.Settings.AlarmSound}.wav"));
			alarm!.PlayLooping();
			EndTimerButton.Visibility = Visibility.Visible;
		}

		private void EndTimer()
		{
			alarm!.Stop();
			alarm.Dispose();
			stopwatch.Stop();
			stopwatch.Reset();

			if (repeatCount == repeat && onBreak)
			{
				SetDefaultValues();
				SetStopStateButtons();
				DisplayTime();
				SetModeText();
			}
			else
			{
				onBreak ^= true;

				if (onBreak)
					length = breakLength;
				else
				{
					length = pomodoroLength;
					repeatCount++;
				}

				SetModeText();
				stopwatch.Start();
				timer.Start();
				SetStartStateButtons();
			}
		}
		private void EndTimerButtonClick(object sender, RoutedEventArgs e) => EndTimer();

		private void SetStartStateButtons()
		{
			EndTimerButton.Visibility = Visibility.Collapsed;
			StartButton.Visibility = Visibility.Collapsed;
			StopButton.Visibility = Visibility.Visible;
			ResetButton.Visibility = Visibility.Collapsed;
			RestartButton.Visibility = Visibility.Visible;
			PomodoroOptions.Visibility = Visibility.Collapsed;
		}
		private void SetStopStateButtons()
		{
			EndTimerButton.Visibility = Visibility.Collapsed;
			StartButton.Visibility = Visibility.Visible;
			StopButton.Visibility = Visibility.Collapsed;
			ResetButton.Visibility = Visibility.Visible;
			RestartButton.Visibility = Visibility.Collapsed;
			PomodoroOptions.Visibility = Visibility.Visible;
		}

		private void StartPomodoro()
		{
			stopwatch.Start();
			timer.Start();
			SetStartStateButtons();
		}
		private void StopPomodoro()
		{
			stopwatch.Stop();
			timer.Stop();
			SetStopStateButtons();
		}
		private void ResetPomodoro()
		{
			stopwatch.Reset();
			DisplayTime();
		}
		private void RestartPomodoro() => stopwatch.Restart();
		private void StartButtonClick(object sender, RoutedEventArgs e) => StartPomodoro();
		private void StopButtonClick(object sender, RoutedEventArgs e) => StopPomodoro();
		private void ResetButtonClick(object sender, RoutedEventArgs e) => ResetPomodoro();
		private void RestartButtonClick(object sender, RoutedEventArgs e) => RestartPomodoro();

		private void SetModeText() => ModeDisplay.Text = $"{(onBreak ? "break" : "pomodoro")} ({repeatCount} of {repeat})";
		private void SetPomodoroText()
		{
			PomodoroText.Text = pomodoroLength.TotalMinutes switch
			{
				5 => "5 mins",
				10 => "10 mins",
				15 => "15 mins",
				20 => "20 mins",
				25 => "25 mins",
				30 => "30 mins",
				45 => "45 mins",
				60 => "1 hour",
				120 => "2 hours",
				_ => "25 mins"
			};

			if (!onBreak)
			{
				length = pomodoroLength;
				stopwatch.Reset();
			}

			DisplayTime();
		}
		private void SetBreakText()
		{
			BreakText.Text = breakLength.TotalMinutes switch
			{
				1 => "1 min",
				2 => "2 mins",
				3 => "3 mins",
				4 => "4 mins",
				5 => "5 mins",
				10 => "10 mins",
				15 => "15 mins",
				_ => "5 mins"
			};

			if (onBreak)
			{
				length = breakLength;
				stopwatch.Reset();
			}

			DisplayTime();
		}
		private void SetRepeatText() => RepeatText.Text = $"{repeat}";

		private void IncreasePomodoro()
		{
			pomodoroLength = TimeSpan.FromMinutes(pomodoroLength.TotalMinutes switch
			{
				5 => 10,
				10 => 15,
				15 => 20,
				20 => 25,
				25 => 30,
				30 => 45,
				45 => 60,
				60 => 120,
				120 => 120,
				_ => 25
			});
			SetPomodoroText();
		}
		private void DecreasePomodoro()
		{
			pomodoroLength = TimeSpan.FromMinutes(pomodoroLength.TotalMinutes switch
			{
				120 => 60,
				60 => 45,
				45 => 30,
				30 => 25,
				25 => 20,
				20 => 15,
				15 => 10,
				10 => 5,
				5 => 5,
				_ => 25
			});
			SetPomodoroText();
		}
		private void IncreaseBreak()
		{
			breakLength = TimeSpan.FromMinutes(breakLength.TotalMinutes switch
			{
				1 => 2,
				2 => 3,
				3 => 4,
				4 => 5,
				5 => 10,
				10 => 15,
				15 => 15,
				_ => 5
			});
			SetBreakText();
		}
		private void DecreaseBreak()
		{
			breakLength = TimeSpan.FromMinutes(breakLength.TotalMinutes switch
			{
				15 => 10,
				10 => 5,
				5 => 4,
				4 => 3,
				3 => 2,
				2 => 1,
				1 => 1,
				_ => 25
			});
			SetBreakText();
		}
		private void IncreaseRepeat()
		{
			if (repeat < 10)
			{
				repeat++;
				SetRepeatText();
				SetModeText();
			}
		}
		private void DecreaseRepeat()
		{
			if (repeat > 1 && repeat > repeatCount)
			{
				repeat--;
				SetRepeatText();
				SetModeText();
			}
		}
		private void IncreasePomodoroButtonClick(object sender, RoutedEventArgs e) => IncreasePomodoro();
		private void DecreasePomodoroButtonClick(object sender, RoutedEventArgs e) => DecreasePomodoro();
		private void IncreaseBreakButtonClick(object sender, RoutedEventArgs e) => IncreaseBreak();
		private void DecreaseBreakButtonClick(object sender, RoutedEventArgs e) => DecreaseBreak();
		private void IncreaseRepeatButtonClick(object sender, RoutedEventArgs e) => IncreaseRepeat();
		private void DecreaseRepeatButtonClick(object sender, RoutedEventArgs e) => DecreaseRepeat();

		private void ResetAll()
		{
			SetDefaultValues();
			SetPomodoroText();
			SetBreakText();
			SetRepeatText();
			SetModeText();
		}
		private void ResetAllButtonClick(object sender, RoutedEventArgs e) => ResetAll();
	}
}