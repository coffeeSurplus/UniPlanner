using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UniPlanner.Source.Data;

namespace UniPlanner.Source.Converters;

internal class EventCurrentDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateOnly)value).FirstDayOfWeek().AddDays(int.Parse((string)parameter)) == DateOnly.FromDateTime(DateTime.Now) ? FontWeights.SemiBold : FontWeights.Normal;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventCurrentDayConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => $"{values[0]:dddd d MMMM} • {values[1]} {((int)values[1] != 1 ? "events" : "event")}".ToLower();
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventCurrentMonthConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		DateOnly date = (DateOnly)values[0];
		return $"{date.AddDays(1 - date.Day):MMMM yyyy} • {values[1]} {((int)values[1] != 1 ? "events" : "event")}".ToLower();
	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventCurrentWeekConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		DateOnly currentDate = (DateOnly)values[0];
		return $"{currentDate.FirstDayOfWeek():d MMMM} - {currentDate.FirstDayOfWeek().AddDays(6):d MMMM} • {values[1]} {((int)values[1] != 1 ? "events" : "event")}".ToLower();
	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventLengthConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => (bool)values[0] ? 60 : ((TimeOnly)values[2] - (TimeOnly)values[1]).TotalMinutes;
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class EventMarginConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Thickness(0, (bool)values[0] ? 10 : ((TimeOnly)values[1]).ToTimeSpan().TotalMinutes + 70, 0, 0);
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventMonthColumnConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateOnly)value).UKDayOfWeek();
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventMonthCurrentDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DateOnly date = (DateOnly)value;
		return date.AddDays(1 - date.Day).FirstDayOfWeek().AddDays(int.Parse((string)parameter)) == DateOnly.FromDateTime(DateTime.Now) ? FontWeights.SemiBold : FontWeights.Normal;
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventMonthDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DateOnly date = (DateOnly)value;
		DateOnly newDate = date.AddDays(1 - date.Day).FirstDayOfWeek().AddDays(int.Parse((string)parameter));
		return newDate.Month == date.Month ? newDate.Day : string.Empty;
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventMonthRowConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DateOnly date = (DateOnly)value;
		return (date.DayNumber - date.AddDays(1 - date.Day).FirstDayOfWeek().DayNumber) / 7;
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventSubtitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string details = (string)values[0];
		string location = (string)values[1];
		List<string> subtitle = [];
		if (details != null)
		{
			subtitle.Add(details);
		}
		if (location != null)
		{
			subtitle.Add(location);
		}
		return subtitle.Count != 0 ? string.Join(" • ", subtitle) : string.Empty;
	}
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventSubtitleVisibilityConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[0] != null || values[1] != null ? Visibility.Visible : Visibility.Collapsed;
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}

internal class EventTitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => (bool)values[0] ? $"all day - {values[3]}" : $"{values[1]:H:mm} - {values[2]:H:mm} • {values[3]}";
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class EventTodayDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (DateOnly)value == DateOnly.FromDateTime(DateTime.Now);
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class EventWeekDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateOnly)value).FirstDayOfWeek().AddDays(int.Parse((string)parameter)).ToString("ddd d/M").ToLower();
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}