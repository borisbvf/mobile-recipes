using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Recipes.ViewModels;

public class RecipeListViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<Recipe> Recipes { get; } = new();

	private bool isRefreshing;
	public bool IsRefreshing
	{
		get => isRefreshing;
		set
		{
			if (isRefreshing != value)
			{
				isRefreshing = value;
				OnPropertyChanged();
			}
		}
	}
	
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
			if (!recipeService.CheckIfDBExists())
			{
				await Shell.Current.DisplayAlert("", "Database does not exists, will be created.", "Ok");
				await recipeService.CreateTables();
			}

			IEnumerable<Recipe> result = await recipeService.GetRecipeListAsync();
			if (Recipes.Count > 0)
				Recipes.Clear();
			foreach (Recipe recipe in result)
				Recipes.Add(recipe);
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
			IsRefreshing = false;
		}
	}

	public ICommand GoToDetailsCommand => new Command(GoToDetails);
	private void GoToDetails(object obj)
	{
		Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			var navParameter = new Dictionary<string, object> { { Constants.RecipeIdParameter, recipe.Id } };
			Shell.Current.GoToAsync(Constants.DetailPageRoute, navParameter);
		}
	}

	public ICommand AddRecipeCommand => new Command(AddRecipe);
	private void AddRecipe()
	{
		var navParameter = new Dictionary<string, object> { { nameof(Recipe), new Recipe() } };
		Shell.Current.GoToAsync(Constants.EditPageRoute, navParameter);
	}

	public ICommand ShowIngredientListCommand => new Command(ShowIngredientList);
	private async void ShowIngredientList()
	{
		await Shell.Current.GoToAsync(Constants.IngredientListRoute);
	}

	public ICommand ShowTagListCommand => new Command(ShowTagList);
	private async void ShowTagList()
	{
		await Shell.Current.GoToAsync(Constants.TagListRoute);
	}

	public ICommand SettingsCommand => new Command(ShowSettings);
	private async void ShowSettings()
	{
		await Shell.Current.GoToAsync($"{Constants.SettingsPageRoute}");
	}

	public ICommand ImageStoreCommand => new Command(ShowImageStore);
	public async void ShowImageStore()
	{
		await Shell.Current.GoToAsync($"{Constants.ImageStoreRoute}");
	}
}