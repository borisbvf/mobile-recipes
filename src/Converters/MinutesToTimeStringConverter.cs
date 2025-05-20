using System.Globalization;

namespace Recipes.Converters;

public class MinutesToTimeStringConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return TimeSpan.FromMinutes(System.Convert.ToDouble(value)).ToString(@"hh\:mm");
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
			return 0;
		}
		else
		{
			return null;
		}
	}
}