namespace Recipes.Models;
public class SettingTheme
{
	public AppTheme AppTheme { get; }
	public string DisplayName { get; }
	public SettingTheme(AppTheme appTheme, string displayName)
	{
		AppTheme = appTheme;
		DisplayName = displayName;
	}
}
