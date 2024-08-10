using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.ViewModels;

namespace UniPlanner.Source;

public partial class App : Application
{
	internal DataManager<List<TaskModel>> TaskManager { get; } = new("TaskList");
	internal DataManager<List<TimetableModel>> TimetableManager { get; } = new("TimetableList");
	internal DataManager<List<EventModel>> EventManager { get; } = new("EventList");
	internal DataManager<List<LinkModel>> LinkManager { get; } = new("LinkList");
	internal DataManager<SettingsModel> SettingsManager { get; } = new("Settings");

	public App()
	{
		if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
		{
			MessageBox.Show("Another instance of this application is already running.", "UniPlanner");
			Shutdown();
		}
		Directory.CreateDirectory(Environment.ExpandEnvironmentVariables($@"%AppData%\UniPlanner\"));
		SetScrollbars();
	}

	public void SetScrollbars() => Resources["ScrollBarVisibility"] = SettingsManager.Data.ScrollbarsEnabled ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
	public void UpdateHomeView() => ((MainWindowViewModel)MainWindow.DataContext).UpdateHomeView();
}