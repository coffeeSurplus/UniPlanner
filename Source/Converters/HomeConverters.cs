using System.Globalization;
using System.Windows.Data;

namespace UniPlanner.Source.Converters;

internal class HomeEventCountConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{value} {((int)value != 1 ? "events" : "event")} this week";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class HomeEventTitleConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => $"{values[0]:ddd d/M} • {((bool)values[1] ? "all day" : $"{values[2]:H:mm} - {values[3]:H:mm}")} • {values[4]}".ToLower();
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}

internal class HomeTaskCountConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{value} incomplete {((int)value != 1 ? "tasks" : "task")}";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class HomeTimetableCountConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{value} timetabled {((int)value != 1 ? "events" : "event")} today";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class HomeUsernameConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"good {DateTime.Now.Hour switch { >= 6 and < 12 => "morning", >= 12 and < 18 => "afternoon", _ => "evening" }}, {value}";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}