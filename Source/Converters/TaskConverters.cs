using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.Converters;

internal class TaskColourConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new SolidColorBrush((Color)ColorConverter.ConvertFromString((bool)value ? "#666666" : (string)parameter));
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TaskPriorityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value switch { 1 => "!!! ​", 2 => "!! ​", 3 => "! ​", _ => string.Empty };
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TaskStrikethroughConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? TextDecorations.Strikethrough : null;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TaskSubheaderConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		TaskModel[] tasks = [.. ((ReadOnlyObservableCollection<Object>)value).Cast<TaskModel>()];
		int taskCount = tasks.Count(x => !x.Completed);
		int subtaskCount = tasks.Select(x => x.Subtasks.Count(x => !x.Completed)).Sum();
		List<string> subheader = [];
		if (taskCount != 0)
		{
			subheader.Add($"{taskCount} {(taskCount != 1 ? "tasks" : "task")}");
		}
		if (subtaskCount != 0)
		{
			subheader.Add($"{subtaskCount} {(subtaskCount != 1 ? "subtasks" : "subtask")}");
		}
		return subheader.Count != 0 ? string.Join(" • ", subheader) : "completed";
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TaskSubtitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string subject = (string)values[0];
		DateOnly date = (DateOnly)values[1];
		string details = (string)values[2];
		List<string> subtitle = [];
		if (subject != null)
		{
			subtitle.Add(subject);
		}
		if (date != DateOnly.MaxValue)
		{
			subtitle.Add(date.ToString("dddd d MMMM").ToLower());
		}
		if (details != null)
		{
			subtitle.Add(details);
		}
		return string.Join(" • ", subtitle);

	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class TaskSubtitleVisibilityConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[0] != null || (DateOnly)values[1] != DateOnly.MaxValue || values[2] != null ? Visibility.Visible : Visibility.Collapsed;
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class SubtaskCollectionViewConverter : IMultiValueConverter
{
	public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[0] != null)
		{
			ICollectionView subtaskCollectionView = new CollectionViewSource() { Source = (List<SubtaskModel>)values[0] }.View;
			subtaskCollectionView.SortDescriptions.Add(new("Completed", ListSortDirection.Ascending));
			subtaskCollectionView.SortDescriptions.Add(new("Title", (bool)values[2] ? ListSortDirection.Ascending : ListSortDirection.Descending));
			subtaskCollectionView.Filter = (bool)values[1] ? null : (object parameter) => !((SubtaskModel)parameter).Completed;
			return subtaskCollectionView;
		}
		return null;
	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class SubtaskRemoveConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Tuple<TaskModel, SubtaskModel>((TaskModel)values[0], (SubtaskModel)values[1]);
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}