using Recipes.ViewModels;

namespace Recipes.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();

		BindingContext = settingsViewModel;
	}
}