using BaseMobile.ViewModels;

namespace BaseMobile.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();

		BindingContext = settingsViewModel;
	}
}