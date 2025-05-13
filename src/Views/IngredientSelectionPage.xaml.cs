namespace Recipes.Views;

public partial class IngredientSelectionPage : ContentPage
{
	public IngredientSelectionPage(IngredientListViewModel ingredientViewModel)
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