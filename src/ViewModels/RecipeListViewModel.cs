using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recipes.ViewModels;

public class RecipeListViewModel : BaseViewModel, IQueryAttributable
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

	protected string? FilterText { get; set; }
	public ObservableCollection<Ingredient> FilterIngredients { get; } = [];
	public ObservableCollection<RecipeTag> FilterTags { get; } = [];

	private bool isTagFilterNotEmpty;
	public bool IsTagFilterNotEmpty
	{
		get => isTagFilterNotEmpty;
		set
		{
			if (value != isTagFilterNotEmpty)
			{
				isTagFilterNotEmpty = value;
				OnPropertyChanged();
			}
		}
	}
	private bool isIngredientFilterNotEmpty;
	public bool IsIngredientFilterNotEmpty
	{
		get => isIngredientFilterNotEmpty;
		set
		{
			if (value != isIngredientFilterNotEmpty)
			{
				isIngredientFilterNotEmpty = value;
				OnPropertyChanged();
			}
		}
	}

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

	public ICommand GoToSearchCommand => new Command(GoToSearch);
	private async void GoToSearch()
	{
		await Shell.Current.GoToAsync(Constants.RecipeSearchPage);
	}

	private List<int> GetIds(IEnumerable<object> entities)
	{
		List<int> result = new();
		foreach (object item in entities)
		{
			if (item is RecipeTag)
			{
				result.Add(((RecipeTag)item).Id);
			}
			else if (item is Ingredient)
			{
				result.Add(((Ingredient)item).Id);
			}
		}
		return result;
	}

	private async Task GetFilteredData()
	{
		IsBusy = true;
		try
		{
			if (string.IsNullOrWhiteSpace(FilterText) && FilterTags.Count == 0 && FilterIngredients.Count == 0)
			{
				if (Recipes.Count > 0)
					Recipes.Clear();
				return;
			}
			IEnumerable<Recipe> result = await recipeService.GetRecipeListAsync(FilterText, GetIds(FilterTags), GetIds(FilterIngredients));
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
		}
	}

	public ICommand SearchTextChangedCommand => new Command(SearchTextChanged, (object obj) => obj != null);
	private async void SearchTextChanged(object obj)
	{
		TextChangedEventArgs args = (TextChangedEventArgs)obj;
		if (args != null)
		{
			FilterText = args.NewTextValue;
			await GetFilteredData();
		}
	}

	public ICommand SelectIngredientsCommand => new Command(SelectIngredients);
	private async void SelectIngredients()
	{
		Dictionary<string, object> navParam = new();
		List<int> ids = GetIds(FilterIngredients);
		navParam.Add(Constants.CheckedIngredientsParameter, ids);
		await Shell.Current.GoToAsync(Constants.IngredientSelectionRoute, navParam);
	}

	private async Task LoadTagsByIds(List<int>? ids)
	{
		IsBusy = true;
		FilterTags.Clear();
		try
		{
			if (ids != null && ids.Count > 0)
			{
				IEnumerable<RecipeTag> data = await recipeService.GetTagListAsync(ids);
				foreach (RecipeTag tag in data)
				{
					FilterTags.Add(tag);
				}
			}
			IsTagFilterNotEmpty = FilterTags.Count > 0;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error while getting tags by ids: {ex.Message}");
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

	public ICommand SelectTagsCommand => new Command(SelectTags);
	private async void SelectTags()
	{
		Dictionary<string, object> navParam = new Dictionary<string, object>
		{
			{ Constants.CheckedTagsParameter , GetIds(FilterTags) }
		};
		await Shell.Current.GoToAsync(Constants.TagSelectionRoute, navParam);
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(Constants.CheckedTagsParameter))
		{
			List<int>? paramTagIds = query[Constants.CheckedTagsParameter] as List<int>;
			await LoadTagsByIds(paramTagIds);
			await GetFilteredData();
			query.Remove(Constants.CheckedTagsParameter);
		}

		if (query.ContainsKey(Constants.SelectedIngredientsParameter))
		{
			List<Ingredient> selected = (List<Ingredient>)query[Constants.SelectedIngredientsParameter];
			FilterIngredients.Clear();
			foreach (Ingredient item in selected)
			{
				FilterIngredients.Add(new ObservableIngredient(item));
			}
			IsIngredientFilterNotEmpty = FilterIngredients.Count > 0;
			await GetFilteredData();
			query.Remove(Constants.SelectedIngredientsParameter);
		}
	}
}