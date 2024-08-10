using System.Globalization;
using System.Windows.Data;

namespace UniPlanner.Source.Converters;

internal class TimersResetRestartTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? "restart" : "reset";
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimersSelectedBreakConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value switch { 1 => "1 min", 2 => "2 mins", 3 => "3 mins", 4 => "4 mins", 5 => "5 mins", 10 => "10 mins", _ => "15 mins" };
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimersSelectedPomodoroConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value switch { 5 => "5 mins", 10 => "10 mins", 15 => "15 mins", 20 => "20 mins", 25 => "25 mins", 30 => "30 mins", 45 => "45 mins", 60 => "1 hour", _ => "2 hours" };
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class TimersStatusConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => $"{((bool)values[0] ? "break" : "pomodoro")} ({(int)values[1]} of {(int)values[2]})";
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing, Binding.DoNothing];
}