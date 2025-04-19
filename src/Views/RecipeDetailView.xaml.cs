namespace Recipes.Views;

public partial class RecipeDetailView : ContentPage
{
	public RecipeDetailView(RecipeDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}