using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.CollectionViews;

internal class LinkCollectionView : CollectionViewBase<LinkModel>
{
	public LinkCollectionView(List<LinkModel> list) : base(list)
	{
		CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
		CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Subgroup"));
		CollectionView.SortDescriptions.Add(new("Group", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Subgroup", ListSortDirection.Ascending));
		CollectionView.SortDescriptions.Add(new("Title", ListSortDirection.Ascending));
	}

	public void UpdateView(bool showAll) => CollectionView.Filter = showAll ? null : (object parameter) => ((LinkModel)parameter).Favourite;
}