using System.ComponentModel;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class HomeEventCollectionView : CollectionViewBase<EventModel>
{
	public HomeEventCollectionView(List<EventModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("AllDay", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
		CollectionView.Filter = (object parameter) => ((EventModel)parameter).Date.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber is >= 0 and < 7;
	}
}

internal class HomeLinkCollectionView : CollectionViewBase<LinkModel>
{
	public HomeLinkCollectionView(List<LinkModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("Group", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Subgroup", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
		CollectionView.Filter = (object parameter) => ((LinkModel)parameter).Favourite;
	}
}

internal class HomeTaskCollectionView : CollectionViewBase<TaskModel>
{
	public HomeTaskCollectionView(List<TaskModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("Subject", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
		CollectionView.Filter = (object parameter) => !((TaskModel)parameter).Completed;
	}
}

internal class HomeTimetableCollectionView : CollectionViewBase<TimetableModel>
{
	public HomeTimetableCollectionView(List<TimetableModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		CollectionView.Filter = (object parameter) => ((TimetableModel)parameter).Day == DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek();
	}
}
