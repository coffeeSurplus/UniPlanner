using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UniPlanner.Source.Converters;

internal class LinkExpanderBindingConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values[3] == null || ((bool)values[0] ? (bool)values[1] : (bool)values[2]);
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [false, Binding.DoNothing, null!, Binding.DoNothing];
}

internal class LinkExpanderVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string?)value != null ? Visibility.Visible : Visibility.Collapsed;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}

internal class LinkMarginConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new Thickness((string?)values[0] != null ? (string?)values[1] != null ? 85 : 45 : 5, 0, 5, 0);
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing, Binding.DoNothing];
}