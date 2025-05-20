using System.Globalization;

namespace Recipes.Converters;
public class TextToColorConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return Color.FromArgb((string)value);
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
			return ((Color)value).ToArgbHex();
		}
		else
		{
			return null;
		}
	}
}
