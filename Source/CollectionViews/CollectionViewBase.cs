using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.Models;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.CollectionViews;

internal abstract class CollectionViewBase<T> : ViewModelBase
{
	public ICollectionView CollectionView { get; }

	public CollectionViewBase(List<T> list) => CollectionView = new CollectionViewSource() { Source = list }.View;

	public virtual void UpdateView() => CollectionView.Refresh();
}

internal abstract class EventCollectionViewBase : ViewModelBase
{
	private ICollectionView eventCollectionView;

	public ICollectionView EventCollectionView
	{
		get => eventCollectionView;
		set => SetValue(ref eventCollectionView, value);
	}

	public EventCollectionViewBase(List<EventModel> eventList, DateOnly currentDate)
	{
		eventCollectionView = new CollectionViewSource() { Source = eventList }.View;
		EventCollectionView.SortDescriptions.Add(new("AllDay", ListSortDirection.Descending));
		EventCollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
		EventCollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
		UpdateView(currentDate);
	}

	public abstract void UpdateView(DateOnly currentDate);
}