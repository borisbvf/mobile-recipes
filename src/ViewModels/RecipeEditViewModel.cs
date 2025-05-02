using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;
[QueryProperty(nameof(Recipe), "Recipe")]
public class RecipeEditViewModel: BaseViewModel, IQueryAttributable
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService recipeService;

	public RecipeEditViewModel(IRecipeService recipeService)
	{
		Title = "";
		this.recipeService = recipeService;
		recipe = new();
	}

	public double BottomButtonWidth
	{
		get => DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density / 2 - 8;
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

	public ObservableCollection<RecipeTag> Tags { get; } = [];

	public ICommand SaveCommand => new Command(SaveRecipe);
	private async void SaveRecipe(object obj)
	{
		//Recipe? recipe = obj as Recipe;
		if (recipe != null)
		{
			if (string.IsNullOrEmpty(recipe.Name))
			{
				await Shell.Current.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["WmRecipeNameEmpty"].ToString(),
					LocalizationManager["Ok"].ToString());
				return;
			}
			if (string.IsNullOrEmpty(recipe.Instructions))
			{
				await Shell.Current.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["WmRecipeDirectionsEmpty"].ToString(),
					LocalizationManager["Ok"].ToString());
				return;
			}
			try
			{
				if (recipe.Id == 0)
				{
					await recipeService.AddRecipeAsync(recipe);
				}
				else
				{
					await recipeService.UpdateRecipeAsync(recipe);
				}
				await Shell.Current.GoToAsync("..");
			}
			catch (Exception ex)
			{
				await Shell.Current.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					ex.Message,
					LocalizationManager["Ok"].ToString());
			}
		}
	}

	public ICommand CancelCommand => new Command(CancelRecipe);
	private void CancelRecipe()
	{
		Shell.Current.GoToAsync("..");
	}

	public ICommand SelectTagsCommand => new Command(SelectTags);
	private async void SelectTags()
	{
		List<int> tagIds = new();
		foreach (RecipeTag tag in Tags)
		{
			tagIds.Add(tag.Id);
		}
		Dictionary<string, object> navParam = new Dictionary<string, object>
		{
			{ Constants.CheckedTagsParameter , tagIds }
		};
		await Shell.Current.GoToAsync(Constants.TagListRoute, navParam);
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(Constants.CheckedTagsParameter))
		{
			List<int>? tagIds = query[Constants.CheckedTagsParameter] as List<int>;
			Tags.Clear();
			if (tagIds != null)
			{
				IEnumerable<RecipeTag> data = await recipeService.GetTagListAsync(tagIds);
				foreach (RecipeTag tag in data)
				{
					Tags.Add(tag);
				}
			}
		}
	}
}
