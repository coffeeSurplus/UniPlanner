using System.Diagnostics;
using System.Media;
using System.Windows.Threading;
using UniPlanner.Source.Data;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class TimersViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;

	private int currentPageNumber = 0;

	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}

	public TimersViewModel(DataAccess data)
	{
		dataAccess = data;
		TimerViewModel = new(dataAccess);
		PomodoroViewModel = new(dataAccess);
	}

	public StopwatchViewModel StopwatchViewModel { get; } = new();
	public TimerViewModel TimerViewModel { get; }
	public PomodoroViewModel PomodoroViewModel { get; }
}

internal class StopwatchViewModel : ViewModelBase
{
	private readonly DispatcherTimer dispatcherTimer = new() { Interval = new(0, 0, 0, 0, 1) };
	private readonly Stopwatch stopwatch = new();

	private bool isRunning = false;
	private TimeSpan currentTime = TimeSpan.Zero;

	public bool IsRunning
	{
		get => isRunning;
		set => SetValue(ref isRunning, value);
	}
	public TimeSpan CurrentTime
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
		dispatcherTimer.Tick += TimerElapsed;
	}

	private void TimerElapsed(object? sender, EventArgs e) => CurrentTime = stopwatch.Elapsed;

	private void StartStopwatch()
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
	private void ResetStopwatch()
	{
		if (IsRunning)
		{
			stopwatch.Restart();
		}
		else
		{
			stopwatch.Reset();
			CurrentTime = TimeSpan.Zero;
		}
	}
}

internal class TimerViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
	private readonly DispatcherTimer dispatcherTimer = new() { Interval = new(0, 0, 0, 0, 1) };
	private readonly Stopwatch stopwatch = new();
	private SoundPlayer soundPlayer = new();

	private bool isRunning = false;
	private bool isFinished = false;
	private int timerLength = 600;
	private TimeSpan currentTime = TimeSpan.FromMinutes(10);

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
	public TimeSpan CurrentTime
	{
		get => currentTime;
		set => SetValue(ref currentTime, value);
	}

	public RelayCommand UpdateStartTimeCommand { get; }
	public RelayCommand IncreaseHourCommand { get; }
	public RelayCommand DecreaseHourCommand { get; }
	public RelayCommand IncreaseMinuteCommand { get; }
	public RelayCommand DecreaseMinuteCommand { get; }
	public RelayCommand IncreaseSecondCommand { get; }
	public RelayCommand DecreaseSecondCommand { get; }
	public RelayCommand StartTimerCommand { get; }
	public RelayCommand ResetTimerCommand { get; }
	public RelayCommand EndTimerCommand { get; }

	public TimerViewModel(DataAccess data)
	{
		dataAccess = data;
		dispatcherTimer.Tick += TimerElapsed;
		UpdateStartTimeCommand = new(UpdateStartTime);
		IncreaseHourCommand = new(IncreaseHour);
		DecreaseHourCommand = new(DecreaseHour);
		IncreaseMinuteCommand = new(IncreaseMinute);
		DecreaseMinuteCommand = new(DecreaseMinute);
		IncreaseSecondCommand = new(IncreaseSecond);
		DecreaseSecondCommand = new(DecreaseSecond);
		StartTimerCommand = new(StartTimer);
		ResetTimerCommand = new(ResetTimer);
		EndTimerCommand = new(EndTimer);
	}

	private void TimerElapsed(object? sender, EventArgs e)
	{
		if (stopwatch.Elapsed.TotalSeconds >= TimerLength)
		{
			IsFinished = true;
			dispatcherTimer.Stop();
			stopwatch.Reset();
			soundPlayer = dataAccess.Settings.ReturnAlarm();
			soundPlayer.PlayLooping();
			CurrentTime = TimeSpan.Zero;
		}
		else
		{
			CurrentTime = TimeSpan.FromSeconds(TimerLength + 1) - stopwatch.Elapsed;
		}
	}

	private void UpdateStartTime() => UpdateStartTime(0);
	private void IncreaseHour() => UpdateStartTime(3600);
	private void DecreaseHour() => UpdateStartTime(-3600);
	private void IncreaseMinute() => UpdateStartTime(60);
	private void DecreaseMinute() => UpdateStartTime(-60);
	private void IncreaseSecond() => UpdateStartTime(1);
	private void DecreaseSecond() => UpdateStartTime(-1);
	private void StartTimer()
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
	private void ResetTimer()
	{
		if (IsRunning)
		{
			stopwatch.Restart();
		}
		else
		{
			stopwatch.Reset();
			UpdateStartTime(0);
		}
	}
	private void EndTimer()
	{
		IsFinished = false;
		IsRunning = false;
		soundPlayer.Stop();
		soundPlayer.Dispose();
		UpdateStartTime(0);
	}

	private void UpdateStartTime(int timerChange)
	{
		TimerLength -= (int)stopwatch.Elapsed.TotalSeconds;
		stopwatch.Reset();
		if (TimerLength + timerChange is > 0 and < 86400)
		{
			TimerLength += timerChange;
		}
		CurrentTime = TimeSpan.FromSeconds(TimerLength);
	}
}

internal class PomodoroViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;
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
	private TimeSpan currentTime = TimeSpan.FromMinutes(25);

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
	public TimeSpan CurrentTime
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

	public PomodoroViewModel(DataAccess data)
	{
		dataAccess = data;
		dispatcherTimer.Tick += TimerElapsed;
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
	}

	private void TimerElapsed(object? sender, EventArgs e)
	{
		if (stopwatch.Elapsed.TotalMinutes >= (OnBreak ? BreakLength : PomodoroLength))
		{
			IsFinished = true;
			dispatcherTimer.Stop();
			stopwatch.Reset();
			soundPlayer = dataAccess.Settings.ReturnAlarm();
			soundPlayer.PlayLooping();
			CurrentTime = TimeSpan.Zero;
		}
		else
		{
			CurrentTime = TimeSpan.FromSeconds((OnBreak ? BreakLength : PomodoroLength) * 60 + 1) - stopwatch.Elapsed;
		}
	}

	private void IncreasePomodoro()
	{
		PomodoroLength = PomodoroLength switch { 5 => 10, 10 => 15, 15 => 20, 20 => 25, 25 => 30, 30 => 45, 45 => 60, _ => 120 };
		UpdateStartTime(!OnBreak);
	}
	private void DecreasePomodoro()
	{
		PomodoroLength = PomodoroLength switch { 120 => 60, 60 => 45, 45 => 30, 30 => 25, 25 => 20, 20 => 15, 15 => 10, _ => 5 };
		UpdateStartTime(!OnBreak);
	}
	private void IncreaseBreak()
	{
		BreakLength = BreakLength switch { 1 => 2, 2 => 3, 3 => 4, 4 => 5, 5 => 10, _ => 15 };
		UpdateStartTime(OnBreak);
	}
	private void DecreaseBreak()
	{
		BreakLength = BreakLength switch { 15 => 10, 10 => 5, 5 => 4, 4 => 3, 3 => 2, _ => 1 };
		UpdateStartTime(OnBreak);
	}
	private void IncreaseRepeat()
	{
		RepeatLength += repeatLength < 10 ? 1 : 0;
		UpdateStartTime(false);
	}
	private void DecreaseRepeat()
	{
		RepeatLength -= RepeatLength > 1 ? 1 : 0;
		UpdateStartTime(false);
	}
	private void ResetAll() => (OnBreak, PomodoroLength, BreakLength, RepeatLength, CurrentRepeat, CurrentTime) = (false, 25, 5, 1, 1, TimeSpan.FromMinutes(25));
	private void StartTimer()
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
	private void ResetTimer()
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
	private void EndTimer()
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

	private void UpdateStartTime(bool resetStopwatch)
	{
		if (resetStopwatch)
		{
			stopwatch.Reset();
			CurrentTime = TimeSpan.FromMinutes(OnBreak ? BreakLength : PomodoroLength);
		}
		if (RepeatLength < CurrentRepeat)
		{
			CurrentRepeat = RepeatLength;
		}
	}
}