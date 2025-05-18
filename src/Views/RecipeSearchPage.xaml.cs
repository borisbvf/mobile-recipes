namespace Recipes.Views;

public partial class RecipeSearchPage : ContentPage
{
	public RecipeSearchPage(RecipeListViewModel recipeListViewModel)
	{
		InitializeComponent();
		BindingContext = recipeListViewModel;
	}

	private void SearchEntry_Loaded(object sender, EventArgs e)
	{
		SearchEntry.Focus();
	}
}