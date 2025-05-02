namespace Recipes.Views;

public partial class RecipeListView : ContentPage
{
	public RecipeListView(RecipeListViewModel viewModel)
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