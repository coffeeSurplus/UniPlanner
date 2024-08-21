using System.Windows;
using UniPlanner.Source.Models;
using UniPlanner.Source.ViewModels;

namespace UniPlanner.Source.Data;

internal class DataAccess
{
	public static MainWindowViewModel MainWindow => (MainWindowViewModel)Application.Current.MainWindow.DataContext;

	private DataManager<List<TimetableModel>> TimetableManager { get; } = new("TimetableList");
	private DataManager<List<TaskModel>> TaskManager { get; } = new("TaskList");
	private DataManager<List<EventModel>> EventManager { get; } = new("EventList");
	private DataManager<List<LinkModel>> LinkManager { get; } = new("LinkList");
	private DataManager<SettingsModel> SettingsManager { get; } = new("Settings");

	public List<TimetableModel> TimetableList => TimetableManager.Data;
	public List<TaskModel> TaskList => TaskManager.Data;
	public List<EventModel> EventList => EventManager.Data;
	public List<LinkModel> LinkList => LinkManager.Data;
	public SettingsModel Settings => SettingsManager.Data;

	public void UpdateTimetableList() => TimetableManager.UpdateData();
	public void UpdateTaskList() => TaskManager.UpdateData();
	public void UpdateEventList() => EventManager.UpdateData();
	public void UpdateLinkList() => LinkManager.UpdateData();
	public void UpdateSettings() => SettingsManager.UpdateData();
}