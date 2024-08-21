using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UniPlanner.Source.Data;
using UniPlanner.Source.Models;

namespace UniPlanner.Source.Converters;

internal class TimetableCurrentDayConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => int.Parse((string)parameter) == DateOnly.FromDateTime(DateTime.Now).UKDayOfWeek() ? FontWeights.SemiBold : FontWeights.Normal;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimetableLengthConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => ((TimeOnly)values[1] - (TimeOnly)values[0]).TotalMinutes;
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class TimetableMarginConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new Thickness(0, ((TimeOnly)value).ToTimeSpan().TotalMinutes - 530, 0, 0);
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimetableSubheaderConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		List<TimetableModel> timetableList = [.. ((ReadOnlyObservableCollection<object>)value).Cast<TimetableModel>()];
		return $"{timetableList.Count} {(timetableList.Count != 1 ? "events" : "event")}";
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimetableSubtitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string details = (string)values[0];
		string subject = (string)values[1];
		string location = (string)values[2];
		List<string> subtitle = [];
		if (details != null)
		{
			subtitle.Add(details);
		}
		if (subject != null)
		{
			subtitle.Add(subject);
		}
		if (location != null)
		{
			subtitle.Add(location);
		}
		return string.Join(" • ", subtitle);
	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class TimetableSubtitleVisibilityConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[0] != null || values[1] != null || values[2] != null ? Visibility.Visible : Visibility.Collapsed;
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class TimetableTitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => $"{values[0]:H:mm} - {values[1]:H:mm} • {values[2]}";
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}