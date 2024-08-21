using UniPlanner.Source.CollectionViews;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.ViewModels;

internal class HomeViewModel : ViewModelBase
{
	private readonly DataAccess dataAccess;

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

	public RelayCommand<string> OpenLinkCommand { get; }

	public HomeTaskCollectionView TaskCollectionView { get; }
	public HomeTimetableCollectionView TimetableCollectionView { get; }
	public HomeEventCollectionView EventCollectionView { get; }
	public HomeLinkCollectionView LinkCollectionView { get; }

	public HomeViewModel(DataAccess data)
	{
		dataAccess = data;
		TaskCollectionView = new(dataAccess.TaskList);
		TimetableCollectionView = new(dataAccess.TimetableList);
		EventCollectionView = new(dataAccess.EventList);
		LinkCollectionView = new(dataAccess.LinkList);
		OpenLinkCommand = new(OpenLink);
		UpdateView();
	}

	public void UpdateView()
	{
		Username = dataAccess.Settings.Username;
		TaskCollectionView.UpdateView();
		TimetableCollectionView.UpdateView();
		EventCollectionView.UpdateView();
		LinkCollectionView.UpdateView();
		TaskCount = dataAccess.TaskList.Count(x => !x.Completed);
		TimetableCount = dataAccess.TimetableList.Count(x => x.Day == DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek());
		EventCount = dataAccess.EventList.Count(x => x.Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber is >= 0 and < 7);
	}

	private void OpenLink(string parameter) => LinkModel.OpenUrl(parameter, dataAccess.Settings.ReturnBrowser(), dataAccess.Settings.ReturnArguments());
}