namespace Recipes.Views;

public partial class IngredientListPage : ContentPage
{
	public IngredientListPage(IngredientListViewModel ingredientViewModel)
	{
		InitializeComponent();
		BindingContext = ingredientViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as IngredientListViewModel)?.GetIngredientsCommand.Execute(this);
	}
}