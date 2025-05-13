namespace Recipes.Views;

public partial class RecipeEditPage : ContentPage
{
	public RecipeEditPage(RecipeEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}