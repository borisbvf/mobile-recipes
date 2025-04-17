using Recipes.Services;
using System.ComponentModel;

namespace Recipes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            ThemeService.Instance.LoadFromPreferences();
            SetAppTheme();
            ThemeService.Instance.PropertyChanged += OnSettingsPropertyChanged;

            if (Preferences.Default.ContainsKey(Constants.LanguageKey))
            {
                string lang = Preferences.Default.Get(Constants.LanguageKey, Constants.EnglishLang);
				LocalizationManager.Instance.SetLanguage(lang);
            }
        }

        private void SetAppTheme()
        {
            UserAppTheme = ThemeService.Instance?.Theme ?? AppTheme.Unspecified;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeService.Theme))
            {
                SetAppTheme();
            }
        }
    }
}
