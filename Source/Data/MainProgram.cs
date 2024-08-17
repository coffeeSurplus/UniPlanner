using System.Windows;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.Data;

internal static class MainProgram
{
	private readonly static App program = (App)Application.Current;

	public static DataManager<List<TaskModel>> TaskManager => program.TaskManager;
	public static DataManager<List<TimetableModel>> TimetableManager => program.TimetableManager;
	public static DataManager<List<EventModel>> EventManager => program.EventManager;
	public static DataManager<List<LinkModel>> LinkManager => program.LinkManager;
	public static DataManager<SettingsModel> SettingsManager => program.SettingsManager;

	public static void SetScrollbars() => program.SetScrollbars();
	public static void UpdateHomeView() => program.UpdateHomeView();
}