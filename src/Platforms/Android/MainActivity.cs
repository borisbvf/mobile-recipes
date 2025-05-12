using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Views;
using System.ComponentModel;

namespace Recipes
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RefreshStatusBarColor();
			ThemeService.Instance.PropertyChanged += OnSettingsPropertyChanged;
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			if (Build.VERSION.SdkInt < BuildVersionCodes.R)
				return;
			if (ThemeService.Instance?.Theme != AppTheme.Unspecified)
				return;
			if (newConfig.IsNightModeActive)
			{
				Window?.SetStatusBarColor(GetColorFromRes("MainDark"));
				Window?.InsetsController?.SetSystemBarsAppearance(0, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
			else
			{
				Window?.SetStatusBarColor(GetColorFromRes("MainLight"));
				Window?.InsetsController?.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
		}

		private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ThemeService.Theme))
			{
				RefreshStatusBarColor();
			}
		}

		private void RefreshStatusBarColor()
		{
			if (Build.VERSION.SdkInt < BuildVersionCodes.R)
				return;

			AppTheme? currentTheme = ThemeService.Instance?.Theme;
			if (currentTheme == AppTheme.Light || currentTheme == AppTheme.Unspecified && !Resources.Configuration.IsNightModeActive)
			{
				Window?.SetStatusBarColor(GetColorFromRes("MainLight"));
				Window?.InsetsController?.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
			else if (currentTheme == AppTheme.Dark || currentTheme == AppTheme.Unspecified && Resources.Configuration.IsNightModeActive)
			{
				Window?.SetStatusBarColor(GetColorFromRes("MainDark"));
				Window?.InsetsController?.SetSystemBarsAppearance(0, (int)WindowInsetsControllerAppearance.LightStatusBars);
			}
		}

		private Android.Graphics.Color GetColorFromRes(string name)
		{
			// Get value from Resources/Styles/Colors.xaml
			return ((Color)App.Current!.Resources[name]).ToAndroid();
		}
	}
}
