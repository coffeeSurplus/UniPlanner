using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class HomeViewModel : ViewModelBase
{
	private readonly SettingsModel settings = MainProgram.SettingsManager.Data;
	private readonly List<TaskModel> taskList = MainProgram.TaskManager.Data;
	private readonly List<TimetableModel> timetableList = MainProgram.TimetableManager.Data;
	private readonly List<EventModel> eventList = MainProgram.EventManager.Data;

	private string username = string.Empty;
	private int taskCount = 0;
	private int timetableCount = 0;
	private int eventCount = 0;

	public string Username
	{
		get => username;
		set => SetValue(ref username, value);
	}
	public int TaskCount
	{
		get => taskCount;
		set => SetValue(ref taskCount, value);
	}
	public int TimetableCount
	{
		get => timetableCount;
		set => SetValue(ref timetableCount, value);
	}
	public int EventCount
	{
		get => eventCount;
		set => SetValue(ref eventCount, value);
	}

	public RelayCommand OpenLinkCommand { get; }

	public HomeTaskCollectionView TaskCollectionView { get; } = new(MainProgram.TaskManager.Data);
	public HomeTimetableCollectionView TimetableCollectionView { get; } = new(MainProgram.TimetableManager.Data);
	public HomeEventCollectionView EventCollectionView { get; } = new(MainProgram.EventManager.Data);
	public HomeLinkCollectionView LinkCollectionView { get; } = new(MainProgram.LinkManager.Data);

	public HomeViewModel()
	{
		OpenLinkCommand = new(OpenLink);
		UpdateView();
	}

	public void UpdateView()
	{
		Username = settings.Username;
		TaskCollectionView.UpdateView();
		TimetableCollectionView.UpdateView();
		EventCollectionView.UpdateView();
		LinkCollectionView.UpdateView();
		TaskCount = taskList.Count(x => !x.Completed);
		TimetableCount = timetableList.Count(x => x.Day == DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek());
		EventCount = eventList.Count(x => x.Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber is >= 0 and < 7);
	}

	private void OpenLink(object parameter) => LinkModel.OpenUrl((string)parameter, settings.ReturnBrowser(), settings.ReturnArguments());
}