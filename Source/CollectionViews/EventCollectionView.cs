using System.Windows.Data;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class EventDayCollectionView(List<EventModel> eventList, DateOnly currentDate) : EventCollectionViewBase(eventList, currentDate)
{
	public override void UpdateView(DateOnly currentDate) => EventCollectionView.Filter = (parameter) => ((EventModel)parameter).Date == currentDate;
}

internal class EventMonthCollectionView : EventCollectionViewBase
{
	public EventMonthCollectionView(List<EventModel> eventList, DateOnly currentDate) : base(eventList, currentDate)
	{
		EventCollectionView = new CollectionViewSource() { Source = eventList.GroupBy(x => x.Date).Select(x => new EventModelGroup(x.Key, new(x))) }.View;
		UpdateView(eventList, currentDate);
	}

	public override void UpdateView(DateOnly currentDate) => EventCollectionView.Filter = (parameter) => (((EventModel)parameter).Date.Year, ((EventModel)parameter).Date.Month) == (currentDate.Year, currentDate.Month);
	public void UpdateView(List<EventModel> eventList, DateOnly currentDate)
	{
		EventCollectionView = new CollectionViewSource() { Source = eventList.GroupBy(x => x.Date).Select(x => new EventModelGroup(x.Key, new(x))) }.View;
		EventCollectionView.Filter = (parameter) => (((EventModelGroup)parameter).Date.Year, ((EventModelGroup)parameter).Date.Month) == (currentDate.Year, currentDate.Month);
	}
}

internal class EventWeekCollectionView(List<EventModel> eventList, DateOnly currentDate) : EventCollectionViewBase(eventList, currentDate)
{
	public override void UpdateView(DateOnly currentDate) => EventCollectionView.Filter = (parameter) => ((EventModel)parameter).Date.FirstDayOfWeek() == currentDate.FirstDayOfWeek();
}