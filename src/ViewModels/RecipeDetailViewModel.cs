using System.Windows.Input;

namespace Recipes.ViewModels;

public class RecipeDetailViewModel : BaseViewModel, IQueryAttributable
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService recipeService;
	public RecipeDetailViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(Constants.RecipeId))
		{
			int recipeId = (int)query[Constants.RecipeId];
			Recipe = await recipeService.GetRecipeAsync(recipeId);
		}
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
			if (await Shell.Current.DisplayAlert("",
				$"{LocalizationManager["DeleteMsg"]}",
				$"{LocalizationManager["Ok"]}",
				$"{LocalizationManager["Cancel"]}"))
			{
				try
				{
					await recipeService.DeleteRecipeAsync(recipe);
					await Shell.Current.GoToAsync("..");
				}
				catch (Exception ex)
				{
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{LocalizationManager["ErrorDeletingRecipe"]}: {ex.Message}",
						$"{LocalizationManager["Ok"]}");
				}
			}
		}
	}
}