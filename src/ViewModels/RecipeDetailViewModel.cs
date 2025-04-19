using System.Windows.Input;

namespace Recipes.ViewModels;

[QueryProperty(nameof(Recipe), "Recipe")]
public class RecipeDetailViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService recipeService;
	public RecipeDetailViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}


	private Recipe? recipe;
	public Recipe? Recipe
	{
		get => recipe;
		set
		{
			recipe = value;
			OnPropertyChanged();
		}
	}

	public ICommand EditRecipeCommand => new Command(EditRecipe);
	private async void EditRecipe(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			var navParameter = new Dictionary<string, object> { { nameof(Recipe), recipe } };
			await Shell.Current.GoToAsync(Constants.EditPageRoute, navParameter);
		}
	}

	public ICommand DeleteRecipeCommand => new Command(DeleteRecipe);
	private async void DeleteRecipe(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			if (await Shell.Current.DisplayPromptAsync("", "Are you sure you want to delete this recipe?") == "Ok")
			{
				await recipeService.DeleteRecipeAsync(recipe.Id);
			}
		}
	}
}