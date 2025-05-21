namespace Recipes.Views;

public partial class BackupManagementPage : ContentPage
{
	public BackupManagementPage(BackupManagementViewModel backupManagementViewModel)
	{
		InitializeComponent();
		BindingContext = backupManagementViewModel;
	}
}