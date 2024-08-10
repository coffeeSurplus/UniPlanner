using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UniPlanner.Source.MVVM;

internal abstract class ViewModelBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? name = null)
	{
		field = value;
		PropertyChanged?.Invoke(this, new(name));
	}
}