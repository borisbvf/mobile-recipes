using System.Globalization;

namespace Recipes.Converters;
public class FilterConditionToTextConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null)
		{
			if (value is FilterCondition)
			{
				if ((FilterCondition)value == FilterCondition.All)
					return $"{LocalizationManager.Instance["FilterConditionAll"]}";
				else if ((FilterCondition)value == FilterCondition.Any)
					return $"{LocalizationManager.Instance["FilterConditionAny"]}";
				else if ((FilterCondition)value == FilterCondition.None)
					return $"{LocalizationManager.Instance["FilterConditionNone"]}";
			}
			throw new ArgumentException("Invalid argument, should be FilterCondition value.");
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
			if ((string)value == $"{LocalizationManager.Instance["FilterConditionAll"]}")
				return FilterCondition.All;
			else if ((string)value == $"{LocalizationManager.Instance["FilterConditionAny"]}")
				return FilterCondition.Any;
			else if ((string)value == $"{LocalizationManager.Instance["FilterConditionNone"]}")
				return FilterCondition.None;
			else
				throw new ArgumentException("Invalid argument, should be FilterCondition value.");
		}
		else
		{
			return null;
		}
	}
}
