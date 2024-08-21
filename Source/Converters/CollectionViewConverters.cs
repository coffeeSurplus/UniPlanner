using System.Globalization;
using System.Windows.Data;

namespace UniPlanner.Source.Converters;

internal class CollectionViewTaskGroupDateConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (DateOnly)value != DateOnly.MaxValue ? (((DateOnly)value).DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) switch { < 0 => "overdue", 0 => "today", 1 => "tomorrow", < 7 => "this week", _ => "later" } : "no date";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class CollectionViewTaskGroupPriorityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value switch { 1 => "high priority", 2 => "medium priority", 3 => "low priority", _ => "no priority" };
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class CollectionViewTaskGroupSubjectConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string?)value ?? "other";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class CollectionViewTimetableDayConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value switch { 0 => "monday", 1 => "tuesday", 2 => "wednesday", 3 => "thursday", _ => "friday" };
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}