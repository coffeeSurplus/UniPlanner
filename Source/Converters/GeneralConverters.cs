using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UniPlanner.Source.Data;

namespace UniPlanner.Source.Converters;

internal class GeneralBoolToBoolInvertedConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
}

internal class GeneralBoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Visible : Visibility.Collapsed;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class GeneralBoolToVisibilityInvertedConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Collapsed : Visibility.Visible;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class GeneralDateOnlyToIntConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateOnly)value).UKDayOfWeek();
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class GeneralIntToColourConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (SolidColorBrush)new BrushConverter().ConvertFromString((int)value switch { 0 => "#F2F2F2", 1 => "#FFADAD", 2 => "#FFD6A5", 3 => "#FDFFB6", 4 => "#CAFFBF", 5 => "#9BF6FF", 6 => "#A0C4FF", 7 => "#BDB2FF", _ => "#FFC6FF" })!;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class GeneralIntToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == int.Parse((string)parameter) ? Visibility.Visible : Visibility.Collapsed;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class GeneralRadioToIntConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == int.Parse((string)parameter);
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? (string)parameter : Binding.DoNothing;
}

internal class GeneralRadioToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == (string)parameter;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? (string)parameter : Binding.DoNothing;
}

internal class GeneralTodayConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => DateTime.Now.ToString("dddd d MMMM").ToLower();
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}