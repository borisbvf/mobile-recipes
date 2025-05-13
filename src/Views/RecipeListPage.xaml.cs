namespace Recipes.Views;

public partial class RecipeListPage : ContentPage
{
	public RecipeListPage(RecipeListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as RecipeListViewModel)?.GetRecipesCommand.Execute(this);
	}
}