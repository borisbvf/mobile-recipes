namespace Recipes.Views;

public partial class RecipeDetailPage : ContentPage
{
	public RecipeDetailPage(RecipeDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}