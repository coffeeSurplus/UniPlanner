using System.ComponentModel;
using System.Windows.Data;
using UniPlanner.Source.MVVM;

namespace UniPlanner.Source.CollectionViews;

internal abstract class CollectionViewBase<T> : ViewModelBase
{
	private CollectionViewSource collectionViewSource;

	public ICollectionView CollectionView
	{
		get => collectionViewSource.View;
		set => SetValue(ref collectionViewSource, new CollectionViewSource() { Source = value.SourceCollection });
	}

	public CollectionViewBase(List<T> list) => collectionViewSource = new CollectionViewSource() { Source = list };

	public virtual void UpdateView() => CollectionView.Refresh();
}