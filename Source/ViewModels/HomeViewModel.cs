using System.Windows;
using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class HomeViewModel : ViewModelBase
{
	private readonly SettingsModel settings = ((App)Application.Current).SettingsManager.Data;
	private readonly List<TaskModel> taskList = ((App)Application.Current).TaskManager.Data;
	private readonly List<TimetableModel> timetableList = ((App)Application.Current).TimetableManager.Data;
	private readonly List<EventModel> eventList = ((App)Application.Current).EventManager.Data;
	private readonly List<LinkModel> linkList = ((App)Application.Current).LinkManager.Data;

	private string username;
	private int taskCount;
	private int timetableCount;
	private int eventCount;

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

	public HomeTaskCollectionView TaskCollectionView { get; }
	public HomeTimetableCollectionView TimetableCollectionView { get; }
	public HomeEventCollectionView EventCollectionView { get; }
	public HomeLinkCollectionView LinkCollectionView { get; }

	public HomeViewModel()
	{
		OpenLinkCommand = new(OpenLink);
		username = settings.Username;
		TaskCollectionView = new(taskList);
		TimetableCollectionView = new(timetableList);
		EventCollectionView = new(eventList);
		LinkCollectionView = new(linkList);
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