namespace Recipes.Utils;

public static class ColorHelper
{
	public static Color GetBaseBackgroundDragColor()
	{
		Color result = (App.Current!.RequestedTheme == AppTheme.Light)
			? (Color)App.Current!.Resources["White"]
			: (Color)App.Current!.Resources["Black"];
		return result;
	}

	public static Color GetBackgroundDragColor()
	{
		return Colors.LightSkyBlue;
	}

	public static Color GetBackgroundSelectionColor(bool isChecked)
	{
		Color result = (App.Current!.RequestedTheme == AppTheme.Light)
			? (Color)App.Current!.Resources["White"]
			: (Color)App.Current!.Resources["Black"];
		if (isChecked)
		{
			result = Colors.LightSkyBlue;
		}
		return result;
	}
}
