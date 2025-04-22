namespace Recipes.Views;

public partial class IngredientListView : ContentPage
{
	public IngredientListView(IngredientListViewModel ingredientViewModel)
	{
		InitializeComponent();
		BindingContext = ingredientViewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as IngredientListViewModel)?.GetIngredientsCommand.Execute(this);
	}
}