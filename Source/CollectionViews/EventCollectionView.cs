using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class EventDayCollectionView : CollectionViewBase<EventModel>
{
	public EventDayCollectionView(List<EventModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("Date", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("AllDay", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
	}

	public void UpdateView(DateOnly currentDate) => CollectionView.Filter = (object parameter) => ((EventModel)parameter).Date == currentDate;
}

internal class EventMonthCollectionView : CollectionViewBase<EventModel>
{
	public EventMonthCollectionView(List<EventModel> list) : base(list)
	{
		CollectionView = new CollectionViewSource() { Source = list.GroupBy(x => x.Date).Select(x => new EventModelGroup(x.Key, new(x))) }.View;
		CollectionView.SortDescriptions.Add(new("Date", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("AllDay", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
	}

	public void UpdateView(DateOnly currentDate) => CollectionView.Filter = (object parameter) => (((EventModelGroup)parameter).Date.Year, ((EventModelGroup)parameter).Date.Month) == (currentDate.Year, currentDate.Month);
}

internal class EventWeekCollectionView : CollectionViewBase<EventModel>
{
	public EventWeekCollectionView(List<EventModel> list) : base(list)
	{
		CollectionView.SortDescriptions.Add(new("Date", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("AllDay", ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
	}

	public void UpdateView(DateOnly currentDate) => CollectionView.Filter = (object parameter) => ((EventModel)parameter).Date.FirstDayOfWeek() == currentDate.FirstDayOfWeek();
}