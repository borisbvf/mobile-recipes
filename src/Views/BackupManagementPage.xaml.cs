namespace Recipes.Views;

public partial class BackupManagementPage : ContentPage
{
	public BackupManagementPage(BackupManagementViewModel backupManagementViewModel)
	{
		InitializeComponent();
		BindingContext = backupManagementViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as BackupManagementViewModel)?.ReloadInfoCommand.Execute(this);
	}
}