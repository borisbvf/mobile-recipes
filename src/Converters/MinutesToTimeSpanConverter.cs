using System.Globalization;

namespace Recipes.Converters;
public class MinutesToTimeSpanConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return TimeSpan.FromMinutes(System.Convert.ToDouble(value));
		}
		else
		{
			return null;
		}
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return ((TimeSpan)value).Hours * 60 + ((TimeSpan)value).Minutes;
		}
		else
		{
			return null;
		}
	}
}