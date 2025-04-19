using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Recipes.ViewModels;

public class RecipeListViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<Recipe> Recipes { get; } = new();
	
	private IRecipeService recipeService;

	public RecipeListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	public ICommand GetRecipesCommand => new Command(GetRecipesAsync);
	public async void GetRecipesAsync()
	{
		if (IsBusy)
			return;
		IsBusy = true;
		try
		{
			RequestResult<IEnumerable<Recipe>> result = await recipeService.GetRecipesAsync();
			if (result.IsSuccess)
			{
				if (Recipes.Count > 0)
					Recipes.Clear();
				foreach (Recipe recipe in result.Data!)
					Recipes.Add(recipe);
			}
			else
			{
				await Shell.Current.DisplayAlert(
					LocalizationManager["Error"].ToString(),
					result.ErrorMessage,
					LocalizationManager["Ok"].ToString());
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get recipes: {ex.Message}");
			await Shell.Current.DisplayAlert(
				LocalizationManager["Error"].ToString(),
				ex.Message,
				LocalizationManager["Ok"].ToString());
		}
		finally
		{
			IsBusy = false;
		}
	}

	public ICommand GoToDetailsCommand => new Command(GoToDetails);
	private void GoToDetails(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			var navParameter = new Dictionary<string, object> { { nameof(Recipe), recipe } };
			Shell.Current.GoToAsync(Constants.DetailPageRoute, navParameter);
		}
	}

	public ICommand AddRecipeCommand => new Command(AddRecipe);
	private void AddRecipe()
	{
		var navParameter = new Dictionary<string, object> { { nameof(Recipe), new Recipe() } };
		Shell.Current.GoToAsync(Constants.EditPageRoute, navParameter);
	}

	public ICommand SettingsCommand => new Command(ShowSettings);
	private async void ShowSettings()
	{
		await Shell.Current.GoToAsync($"{Constants.SettingsPageRoute}");
	}
}