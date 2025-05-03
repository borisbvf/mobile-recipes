using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;
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

	private Ingredient? selectedIngredient;
	public Ingredient? SelectedIngredient
	{
		get => selectedIngredient;
		set
		{
			if (selectedIngredient != value)
			{
				selectedIngredient = value;
				OnPropertyChanged();
			}
		}
	}

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
		foreach (RecipeTag tag in recipe.Tags)
		{
			tagIds.Add(tag.Id);
		}
		Dictionary<string, object> navParam = new Dictionary<string, object>
		{
			{ Constants.CheckedTagsParameter , tagIds }
		};
		await Shell.Current.GoToAsync(Constants.TagListRoute, navParam);
	}

	public ICommand AddIngredientCommand => new Command(AddIngredient);
	public async void AddIngredient()
	{
		await Shell.Current.GoToAsync(Constants.IngredientListRoute);
	}

	public ICommand DeleteIngredientCommand => new Command(DeleteIngredient, () => selectedIngredient != null);
	public void DeleteIngredient()
	{
		if (selectedIngredient != null)
		{
			recipe?.Ingredients.Remove(selectedIngredient);
		}
	}

	public ICommand PickImageCommand => new Command(PickImage);
	public async void PickImage()
	{
		await MediaPicker.Default.PickPhotoAsync();
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(nameof(Recipe)))
		{
			Recipe = query[nameof(Recipe)] as Recipe;
		}

		if (query.ContainsKey(Constants.CheckedTagsParameter))
		{
			List<int>? tagIds = query[Constants.CheckedTagsParameter] as List<int>;
			if (tagIds != null)
			{
				recipe?.Tags.Clear();
				IEnumerable<RecipeTag> data = await recipeService.GetTagListAsync(tagIds);
				foreach (RecipeTag tag in data)
				{
					recipe?.Tags.Add(tag);
				}
			}
		}

		if (query.ContainsKey(nameof(Ingredient)))
		{
			Ingredient? added = query[nameof(Ingredient)] as Ingredient;
			if (added != null && recipe != null)
			{
				bool alreadyAdded = false;
				foreach (Ingredient ingredient in recipe.Ingredients)
				{
					if (ingredient.Id == added.Id)
					{
						alreadyAdded = true;
						break;
					}
				}
				if (!alreadyAdded)
				{
					recipe?.Ingredients.Add(added);
				}
			}
		}
	}
}
