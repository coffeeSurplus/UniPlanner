using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.Converters;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class TaskCollectionView(List<TaskModel> list) : CollectionViewBase<TaskModel>(list)
{
	public void UpdateView(string group, bool orderByAscending, bool showCompleted)
	{
		CollectionView.GroupDescriptions.Clear();
		CollectionView.GroupDescriptions.Add(new PropertyGroupDescription(group, group switch { "Subject" => new TaskGroupSubjectConverter(), "Date" => new TaskGroupDateConverter(), _ => new TaskGroupPriorityConverter() }));
		CollectionView.SortDescriptions.Clear();
		CollectionView.SortDescriptions.Add(new(group, orderByAscending ? ListSortDirection.Ascending : ListSortDirection.Descending));
		CollectionView.SortDescriptions.Add(new("Completed", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", orderByAscending ? ListSortDirection.Ascending : ListSortDirection.Descending));
		CollectionView.Filter = showCompleted ? null : (object parameter) => !((TaskModel)parameter).Completed || ((TaskModel)parameter).Subtasks.Any(x => !x.Completed);
	}
}