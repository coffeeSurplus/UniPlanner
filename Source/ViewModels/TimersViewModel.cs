using System.Diagnostics;
using System.Media;
using System.Windows.Threading;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class TimersViewModel : ViewModelBase
{
	private int currentPageNumber = 0;

	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}

	public StopwatchViewModel StopwatchViewModel { get; } = new();
	public TimerViewModel TimerViewModel { get; } = new();
	public PomodoroViewModel PomodoroViewModel { get; } = new();
}

internal class StopwatchViewModel : ViewModelBase
{
	private readonly DispatcherTimer dispatcherTimer = new() { Interval = new(0, 0, 0, 0, 1) };
	private readonly Stopwatch stopwatch = new();

	private bool isRunning;
	private string currentTime = "00:00.00";

	public bool IsRunning
	{
		get => isRunning;
		set => SetValue(ref isRunning, value);
	}
	public string CurrentTime
	{
		get => currentTime;
		set => SetValue(ref currentTime, value);
	}

	public RelayCommand StartStopwatchCommand { get; }
	public RelayCommand ResetStopwatchCommand { get; }

	public StopwatchViewModel()
	{
		StartStopwatchCommand = new(StartStopwatch);
		ResetStopwatchCommand = new(ResetStopwatch);
		dispatcherTimer.Tick += UpdateTime;
	}

	private void UpdateTime(object? sender, EventArgs e) => CurrentTime = $"{(stopwatch.Elapsed.Hours > 0 ? $"{stopwatch.Elapsed.Hours:00}:" : string.Empty)}{stopwatch.Elapsed.Minutes:00}:{stopwatch.Elapsed.Seconds:00}.{stopwatch.Elapsed.Milliseconds / 10:00}";
	private void StartStopwatch(object parameter)
	{
		if (IsRunning)
		{
			dispatcherTimer.Start();
			stopwatch.Start();
		}
		else
		{
			dispatcherTimer.Stop();
			stopwatch.Stop();
		}
	}
	private void ResetStopwatch(object parameter)
	{
		if (IsRunning)
		{
			stopwatch.Restart();
		}
		else
		{
			stopwatch.Reset();
			CurrentTime = "00:00.00";
		}
	}
}

internal class TimerViewModel : ViewModelBase
{
	private readonly SettingsModel settings = MainProgram.SettingsManager.Data;
	private readonly DispatcherTimer dispatcherTimer = new() { Interval = new(0, 0, 0, 0, 1) };
	private readonly Stopwatch stopwatch = new();
	private SoundPlayer soundPlayer = new();

	private bool isRunning = false;
	private bool isFinished = false;
	private int timerLength = 600;
	private string currentTime = "00:10:00";

	public bool IsRunning
	{
		get => isRunning;
		set => SetValue(ref isRunning, value);
	}
	public bool IsFinished
	{
		get => isFinished;
		set => SetValue(ref isFinished, value);
	}
	public int TimerLength
	{
		get => timerLength;
		set => SetValue(ref timerLength, value);
	}
	public string CurrentTime
	{
		get => currentTime;
		set => SetValue(ref currentTime, value);
	}

	public RelayCommand IncreaseHourCommand { get; }
	public RelayCommand DecreaseHourCommand { get; }
	public RelayCommand IncreaseMinuteCommand { get; }
	public RelayCommand DecreaseMinuteCommand { get; }
	public RelayCommand IncreaseSecondCommand { get; }
	public RelayCommand DecreaseSecondCommand { get; }
	public RelayCommand StartTimerCommand { get; }
	public RelayCommand ResetTimerCommand { get; }
	public RelayCommand EndTimerCommand { get; }
	public RelayCommand UpdateStartTimeCommand { get; }

	public TimerViewModel()
	{
		IncreaseHourCommand = new(IncreaseHour);
		DecreaseHourCommand = new(DecreaseHour);
		IncreaseMinuteCommand = new(IncreaseMinute);
		DecreaseMinuteCommand = new(DecreaseMinute);
		IncreaseSecondCommand = new(IncreaseSecond);
		DecreaseSecondCommand = new(DecreaseSecond);
		StartTimerCommand = new(StartTimer);
		ResetTimerCommand = new(ResetTimer);
		EndTimerCommand = new(EndTimer);
		UpdateStartTimeCommand = new(UpdateStartTime);
		dispatcherTimer.Tick += UpdateTime;
	}

	private void UpdateTime(object? sender, EventArgs e)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(TimerLength + 1) - stopwatch.Elapsed;
		if (timeSpan.TotalMilliseconds <= 1000)
		{
			IsFinished = true;
			dispatcherTimer.Stop();
			stopwatch.Reset();
			soundPlayer = settings.ReturnAlarm();
			soundPlayer.PlayLooping();
			CurrentTime = "00:00:00";
		}
		else
		{
			CurrentTime = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
	}
	private void IncreaseHour(object parameter) => UpdateStartTime(3600);
	private void DecreaseHour(object parameter) => UpdateStartTime(-3600);
	private void IncreaseMinute(object parameter) => UpdateStartTime(60);
	private void DecreaseMinute(object parameter) => UpdateStartTime(-60);
	private void IncreaseSecond(object parameter) => UpdateStartTime(1);
	private void DecreaseSecond(object parameter) => UpdateStartTime(-1);
	private void StartTimer(object parameter)
	{
		if (IsRunning)
		{
			dispatcherTimer.Start();
			stopwatch.Start();
		}
		else
		{
			dispatcherTimer.Stop();
			stopwatch.Stop();
		}
	}
	private void ResetTimer(object parameter)
	{
		if (IsRunning)
		{
			stopwatch.Restart();
		}
		else
		{
			stopwatch.Reset();
			UpdateStartTime();
		}
	}
	private void EndTimer(object parameter)
	{
		IsFinished = false;
		IsRunning = false;
		soundPlayer.Stop();
		soundPlayer.Dispose();
		UpdateStartTime(0);
	}
	private void UpdateStartTime(object parameter)
	{
		stopwatch.Reset();
		UpdateStartTime();
	}

	private void UpdateStartTime(int timerChange = 0)
	{
		TimerLength -= (int)stopwatch.Elapsed.TotalSeconds;
		stopwatch.Reset();
		if (TimerLength + timerChange is > 0 and < 86400)
		{
			TimerLength += timerChange;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(TimerLength);
		CurrentTime = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}
}

internal class PomodoroViewModel : ViewModelBase
{
	private readonly SettingsModel settings = MainProgram.SettingsManager.Data;
	private readonly DispatcherTimer dispatcherTimer = new() { Interval = new(0, 0, 0, 0, 1) };
	private readonly Stopwatch stopwatch = new();
	private SoundPlayer soundPlayer = new();

	private bool onBreak = false;
	private bool isRunning = false;
	private bool isFinished = false;
	private int pomodoroLength = 25;
	private int breakLength = 5;
	private int repeatLength = 1;
	private int currentRepeat = 1;
	private string currentTime = "25:00";

	public bool OnBreak
	{
		get => onBreak;
		set => SetValue(ref onBreak, value);
	}
	public bool IsRunning
	{
		get => isRunning;
		set => SetValue(ref isRunning, value);
	}
	public bool IsFinished
	{
		get => isFinished;
		set => SetValue(ref isFinished, value);
	}
	public int PomodoroLength
	{
		get => pomodoroLength;
		set => SetValue(ref pomodoroLength, value);
	}
	public int BreakLength
	{
		get => breakLength;
		set => SetValue(ref breakLength, value);
	}
	public int RepeatLength
	{
		get => repeatLength;
		set => SetValue(ref repeatLength, value);
	}
	public int CurrentRepeat
	{
		get => currentRepeat;
		set => SetValue(ref currentRepeat, value);
	}
	public string CurrentTime
	{
		get => currentTime;
		set => SetValue(ref currentTime, value);
	}

	public RelayCommand IncreasePomodoroCommand { get; }
	public RelayCommand DecreasePomodoroCommand { get; }
	public RelayCommand IncreaseBreakCommand { get; }
	public RelayCommand DecreaseBreakCommand { get; }
	public RelayCommand IncreaseRepeatCommand { get; }
	public RelayCommand DecreaseRepeatCommand { get; }
	public RelayCommand ResetAllCommand { get; }
	public RelayCommand StartTimerCommand { get; }
	public RelayCommand ResetTimerCommand { get; }
	public RelayCommand EndTimerCommand { get; }

	public PomodoroViewModel()
	{
		IncreasePomodoroCommand = new(IncreasePomodoro);
		DecreasePomodoroCommand = new(DecreasePomodoro);
		IncreaseBreakCommand = new(IncreaseBreak);
		DecreaseBreakCommand = new(DecreaseBreak);
		IncreaseRepeatCommand = new(IncreaseRepeat);
		DecreaseRepeatCommand = new(DecreaseRepeat);
		ResetAllCommand = new(ResetAll);
		StartTimerCommand = new(StartTimer);
		ResetTimerCommand = new(ResetTimer);
		EndTimerCommand = new(EndTimer);
		dispatcherTimer.Tick += UpdateTime;
	}

	private void UpdateTime(object? sender, EventArgs e)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((OnBreak ? BreakLength : PomodoroLength) * 60 + 1) - stopwatch.Elapsed;
		if (timeSpan.TotalMilliseconds < 1000)
		{
			IsFinished = true;
			dispatcherTimer.Stop();
			stopwatch.Reset();
			soundPlayer = settings.ReturnAlarm();
			soundPlayer.PlayLooping();
			CurrentTime = "00:00";
		}
		else
		{
			CurrentTime = $"{(timeSpan.Hours > 0 ? $"{timeSpan.Hours}:" : string.Empty)}{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
	}
	private void IncreasePomodoro(object parameter)
	{
		PomodoroLength = PomodoroLength switch { 5 => 10, 10 => 15, 15 => 20, 20 => 25, 25 => 30, 30 => 45, 45 => 60, _ => 120 };
		UpdateStartTime(!OnBreak);
	}
	private void DecreasePomodoro(object parameter)
	{
		PomodoroLength = PomodoroLength switch { 120 => 60, 60 => 45, 45 => 30, 30 => 25, 25 => 20, 20 => 15, 15 => 10, _ => 5 };
		UpdateStartTime(!OnBreak);
	}
	private void IncreaseBreak(object parameter)
	{
		BreakLength = BreakLength switch { 1 => 2, 2 => 3, 3 => 4, 4 => 5, 5 => 10, _ => 15 };
		UpdateStartTime(OnBreak);
	}
	private void DecreaseBreak(object parameter)
	{
		BreakLength = BreakLength switch { 15 => 10, 10 => 5, 5 => 4, 4 => 3, 3 => 2, _ => 1 };
		UpdateStartTime(OnBreak);
	}
	private void IncreaseRepeat(object parameter)
	{
		RepeatLength += repeatLength < 10 ? 1 : 0;
		UpdateStartTime();
	}
	private void DecreaseRepeat(object parameter)
	{
		RepeatLength -= RepeatLength > 1 ? 1 : 0;
		UpdateStartTime();
	}
	private void ResetAll(object parameter) => (OnBreak, PomodoroLength, BreakLength, RepeatLength, CurrentRepeat) = (false, 25, 5, 1, 1);
	private void StartTimer(object parameter)
	{
		if (IsRunning)
		{
			dispatcherTimer.Start();
			stopwatch.Start();
		}
		else
		{
			dispatcherTimer.Stop();
			stopwatch.Stop();
		}
	}
	private void ResetTimer(object parameter)
	{
		if (IsRunning)
		{
			stopwatch.Restart();
		}
		else
		{
			stopwatch.Reset();
			UpdateStartTime(true);
		}
	}
	private void EndTimer(object parameter)
	{
		OnBreak ^= true;
		if (CurrentRepeat == RepeatLength && !OnBreak)
		{
			UpdateStartTime(true);
			CurrentRepeat = 1;
			IsRunning = false;
		}
		else
		{
			if (!OnBreak)
			{
				CurrentRepeat++;
			}
			dispatcherTimer.Start();
			stopwatch.Start();
		}
		IsFinished = false;
		soundPlayer.Stop();
		soundPlayer.Dispose();
	}

	private void UpdateStartTime(bool resetStopwatch = false)
	{
		if (resetStopwatch)
		{
			stopwatch.Reset();
			TimeSpan timeSpan = TimeSpan.FromSeconds((OnBreak ? BreakLength : PomodoroLength) * 60);
			CurrentTime = $"{(timeSpan.Hours > 0 ? $"{timeSpan.Hours}:" : string.Empty)}{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		if (RepeatLength < CurrentRepeat)
		{
			CurrentRepeat = RepeatLength;
		}
	}
}