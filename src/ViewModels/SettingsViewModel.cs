using Recipes.Services;
using Recipes.Models;
using System.Collections.ObjectModel;
using Recipes.Resources.Localization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class SettingsViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<SettingTheme> Themes { get; set; }
	public SettingTheme? selectedTheme;
	public SettingTheme? SelectedTheme
	{
		get => selectedTheme;
		set
		{
			if (selectedTheme != value)
			{
				selectedTheme = value;
				OnPropertyChanged();

				// Update theme only if it differs from current - it could be the case when language changes.
				if (selectedTheme != null && selectedTheme.AppTheme != ThemeService.Instance.Theme)
				{
					// Preferences will be saved in ThemeService
					ThemeService.Instance!.Theme = selectedTheme.AppTheme;
				}
			}
		}
	}

	public List<string> Languages { get; set; }
	public string selectedLanguage;
	public string SelectedLanguage
	{
		get => selectedLanguage;
		set
		{
			if (selectedLanguage != value)
			{
				selectedLanguage = value;
				OnPropertyChanged();

				LocalizationManager.Instance.SetLanguage(selectedLanguage);

				// Save preferences, they are not saved in LocalizationManager
				Preferences.Default.Set(Constants.LanguageKey, selectedLanguage);

				// Update manually app theme names, because it will not happen automatically
				RefillThemes();
			}
		}
	}

	public SettingsViewModel()
	{
		Themes = new();
		RefillThemes();

		Languages = new()
		{
			Constants.EnglishLang,
			Constants.RussianLang
		};
		selectedLanguage = AppResources.Culture.TwoLetterISOLanguageName == Constants.RussianAbrv
			? Constants.RussianLang
			: Constants.EnglishLang;
	}

	private void RefillThemes()
	{
		AppTheme curTheme = ThemeService.Instance?.Theme ?? AppTheme.Unspecified;
		SettingTheme lightTheme = new SettingTheme(AppTheme.Light, LocalizationManager["LightTheme"].ToString()!);
		SettingTheme darkTheme = new SettingTheme(AppTheme.Dark, LocalizationManager["DarkTheme"].ToString()!);
		SettingTheme systemTheme = new SettingTheme(AppTheme.Unspecified, LocalizationManager["SystemTheme"].ToString()!);
		SettingTheme newTheme = systemTheme;
		if (curTheme == lightTheme.AppTheme)
		{
			newTheme = lightTheme;
		} else if (curTheme == darkTheme.AppTheme)
		{
			newTheme = darkTheme;
		}
		Themes.Add(lightTheme);
		Themes.Add(darkTheme);
		Themes.Add(systemTheme);
		selectedTheme = newTheme;
		while (Themes.Count > 3)
		{
			Themes.RemoveAt(0);
		}
	}

	public ICommand GoToBackupsCommand => new Command(GoToBackups);
	private async void GoToBackups()
	{
		await Shell.Current.GoToAsync(Constants.BackupManagementRoute);
	}

	public ICommand GoToCleaningUnusedCommand => new Command(async () => await Shell.Current.GoToAsync(Constants.CleaningUnusedFilesRoute));
}
