using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UniPlanner.Source.Data;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class MainWindowViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess = new();

	private WindowState currentState;
	private bool sidepanelCollapsed = false;
	private int currentPageNumber = 0;

	public WindowState CurrentState
	{
		get => currentState;
		set => SetValue(ref currentState, value);
	}
	public bool SidepanelCollapsed
	{
		get => sidepanelCollapsed;
		set => SetValue(ref sidepanelCollapsed, value);
	}
	public int CurrentPageNumber
	{
		get => currentPageNumber;
		set => SetValue(ref currentPageNumber, value);
	}

	public HomeViewModel Home { get; }
	public TimetableViewModel Timetable { get; }
	public TaskViewModel Task { get; }
	public EventViewModel Event { get; }
	public LinkViewModel Link { get; }
	public TimersViewModel Timers { get; }
	public SettingsViewModel Settings { get; }

	public RelayCommand MinimiseWindowCommand { get; }
	public RelayCommand MaximiseWindowCommand { get; }
	public RelayCommand CloseWindowCommand { get; }

	public MainWindowViewModel()
	{
		if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
		{
			Popup.MessageBox("Another instance of this application is already running.");
			Application.Current.Shutdown();
		}
		Directory.CreateDirectory(Environment.ExpandEnvironmentVariables($@"%AppData%\UniPlanner\"));
		SetScrollbars();
		currentState = dataAccess.Settings.StartupMaximised ? WindowState.Maximized : WindowState.Normal;
		Home = new(dataAccess);
		Timetable = new(dataAccess);
		Task = new(dataAccess);
		Event = new(dataAccess);
		Link = new(dataAccess);
		Timers = new(dataAccess);
		Settings = new(dataAccess);
		MinimiseWindowCommand = new(MinimiseWindow);
		MaximiseWindowCommand = new(MaximiseWindow);
		CloseWindowCommand = new(CloseWindow);
	}

	public void SetScrollbars() => Application.Current.Resources["ScrollBarVisibility"] = dataAccess.Settings.ScrollbarsEnabled ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
	public void UpdateHomeView() => Home.UpdateView();

	private void MinimiseWindow() => CurrentState = WindowState.Minimized;
	private void MaximiseWindow() => CurrentState = CurrentState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
	private void CloseWindow() => Application.Current.Shutdown();
}