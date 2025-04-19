using System.Windows.Input;

namespace Recipes.ViewModels;
[QueryProperty(nameof(Recipe), "Recipe")]
public class RecipeEditViewModel: BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService recipeService;

	public RecipeEditViewModel(IRecipeService recipeService)
	{
		Title = "";
		this.recipeService = recipeService;
		recipe = new();
	}

	private Recipe? recipe;
	public Recipe? Recipe
	{
		get => recipe;
		set
		{
			if (recipe == value)
				return;
			recipe = value;
			OnPropertyChanged();
		}
	}

	public ICommand SaveCommand => new Command(SaveRecipe);
	private async void SaveRecipe(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			if (string.IsNullOrEmpty(recipe.Name))
			{
				await Shell.Current.DisplayAlert(
						LocalizationManager["Warning"].ToString(),
						LocalizationManager["WrnRecipeNameEmpty"].ToString(),
						LocalizationManager["Ok"].ToString());
				return;
			}
			if (string.IsNullOrEmpty(recipe.Content))
			{
				await Shell.Current.DisplayAlert(
						LocalizationManager["Warning"].ToString(),
						LocalizationManager["WrnRecipeDirectionsEmpty"].ToString(),
						LocalizationManager["Ok"].ToString());
				return;
			}
			RequestResult result;
			if (recipe.Id == 0)
			{
				result = await recipeService.AddRecipeAsync(recipe);
			}
			else
			{
				result = await recipeService.UpdateRecipeAsync(recipe);
			}
			if (result.IsSuccess)
			{
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.DisplayAlert(
						LocalizationManager["Warning"].ToString(),
						result.ErrorMessage,
						LocalizationManager["Ok"].ToString());
			}
		}
	}

	public ICommand CancelCommand => new Command(CancelRecipe);
	private void CancelRecipe()
	{
		Shell.Current.GoToAsync("..");
	}
}
