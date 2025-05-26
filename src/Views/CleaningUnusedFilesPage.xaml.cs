namespace Recipes.Views;

public partial class CleaningUnusedFilesPage : ContentPage
{
	public CleaningUnusedFilesPage(CleaningUnusedFilesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}