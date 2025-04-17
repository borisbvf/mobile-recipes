using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.Services;
public class ThemeService : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	private static ThemeService? _instance = new ThemeService();
	public static ThemeService Instance = _instance ?? new ThemeService();

	private ThemeService()
	{
		theme = AppTheme.Unspecified;
	}

	public void LoadFromPreferences()
	{
		if (Preferences.Default.ContainsKey(Constants.AppThemeKey))
		{
			theme = (AppTheme)Preferences.Default.Get(Constants.AppThemeKey, (int)AppTheme.Unspecified);
		}
	}

	private AppTheme theme;

	public AppTheme Theme
	{
		get => theme;
		set
		{
			if (theme != value)
			{
				theme = value;
				OnPropertyChanged();
				Preferences.Default.Set(Constants.AppThemeKey, (int)theme);
			}
		}
	}
}
