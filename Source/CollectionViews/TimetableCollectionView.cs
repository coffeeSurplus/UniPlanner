using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.Converters;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class TimetableHorizontalCollectionView : CollectionViewBase<TimetableModel>
{
	public TimetableHorizontalCollectionView(List<TimetableModel> list) : base(list)
	{
		CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Day", new TimetableDayConverter()));
		CollectionView.SortDescriptions.Add(new("Day", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
	}
}

internal class TimetableTodayCollectionView : CollectionViewBase<TimetableModel>
{
	public TimetableTodayCollectionView(List<TimetableModel> list) : base(list)
	{
		CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Day", new TimetableDayConverter()));
		CollectionView.SortDescriptions.Add(new("StartTime", ListSortDirection.Ascending));
	}

	public override void UpdateView() => CollectionView.Filter = (object parameter) => ((TimetableModel)parameter).Day == DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek();
}

internal class TimetableVerticalCollectionView(List<TimetableModel> list) : CollectionViewBase<TimetableModel>(list);