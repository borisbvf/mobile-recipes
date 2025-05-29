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

	public ObservableCollection<ObservableIngredient> EditedIngredients { get; } = [];

	private ObservableIngredient? draggedIngredient;


	private RecipeImage? selectedImage;
	public RecipeImage? SelectedImage
	{
		get => selectedImage;
		set
		{
			if (value != selectedImage)
			{
				selectedImage = value;
				OnPropertyChanged();
			}
		}
	}

	private List<string> deletedImages = new();

	public ICommand SaveCommand => new Command(SaveRecipe);
	private async void SaveRecipe()
	{
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
			bool isNameUnique = await recipeService.CheckRecipeNameUnique(recipe.Name, recipe.Id);
			if (!isNameUnique)
			{
				await Shell.Current.DisplayAlert(
					LocalizationManager["Warning"].ToString(),
					LocalizationManager["WmRecipeNameExists"].ToString(),
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
				recipe.Ingredients.Clear();
				for (int index = 0; index < EditedIngredients.Count; index++)
				{
					Ingredient item = new Ingredient(EditedIngredients[index]);
					item.SortOrder = index;
					recipe.Ingredients.Add(item);
				}
				if (recipe.Id == 0)
				{
					await recipeService.AddRecipeAsync(recipe);
				}
				else
				{
					await recipeService.UpdateRecipeAsync(recipe);
				}
				foreach (string filename in deletedImages)
				{
					File.Delete(filename);
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
		if (recipe == null)
			return;
		List<int> tagIds = new();
		foreach (RecipeTag tag in recipe.Tags)
		{
			tagIds.Add(tag.Id);
		}
		Dictionary<string, object> navParam = new Dictionary<string, object>
		{
			{ Constants.CheckedTagsParameter , tagIds }
		};
		await Shell.Current.GoToAsync(Constants.TagSelectionRoute, navParam);
	}

	public ICommand AddIngredientCommand => new Command(AddIngredient);
	public async void AddIngredient()
	{
		Dictionary<string, object> navParam = new();
		List<int> ids = new();
		foreach (ObservableIngredient item in EditedIngredients)
		{
			ids.Add(item.Id);
		}
		navParam.Add(Constants.ExcludedIngredientIdsParameter, ids);
		await Shell.Current.GoToAsync(Constants.IngredientSelectionRoute, navParam);
	}

	public ICommand DeleteIngredientCommand => new Command(DeleteIngredient, (object obj) => obj != null);
	public void DeleteIngredient(object obj)
	{
		if (obj != null)
		{
			EditedIngredients.Remove((ObservableIngredient)obj);
		}
	}

	public ICommand DragIngredientStartingCommand => new Command(DragIngredientStarting);
	public void DragIngredientStarting(object obj)
	{
		draggedIngredient = (ObservableIngredient)obj;
	}

	public ICommand DragIngredientOverCommand => new Command(DragIngredientOver);
	public void DragIngredientOver(object obj)
	{
		ObservableIngredient? current = (ObservableIngredient)obj;
		if (current != null && draggedIngredient != null && current != draggedIngredient)
		{
			current.BackgroundDragColor = Colors.LightSkyBlue;
		}
	}

	public ICommand DragIngredientLeaveCommand => new Command(DragIngredientLeave);
	public void DragIngredientLeave(object obj)
	{
		ObservableIngredient? current = (ObservableIngredient)obj;
		current.BackgroundDragColor = ColorHelper.GetBaseBackgroundDragColor();
	}

	public ICommand DropIngredientCommand => new Command(DropIngredient);
	public void DropIngredient(object obj)
	{
		ObservableIngredient? current = (ObservableIngredient)obj;
		if (current != null && draggedIngredient != null)
		{
			current.BackgroundDragColor = ColorHelper.GetBaseBackgroundDragColor();
			draggedIngredient.BackgroundDragColor = ColorHelper.GetBaseBackgroundDragColor();
			if (current != draggedIngredient)
			{
				EditedIngredients.Remove(draggedIngredient);
				EditedIngredients.Insert(EditedIngredients.IndexOf(current), draggedIngredient);
			}
			draggedIngredient = null;
		}
	}

	private async void AddFile(FileResult image)
	{
		if (!Directory.Exists(Constants.ImageDirectory))
		{
			Directory.CreateDirectory(Constants.ImageDirectory);
		}
		string newFileName = Path.Combine(Constants.ImageDirectory, $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");
		using Stream sourceStream = await image.OpenReadAsync();
		using FileStream newFileStream = File.OpenWrite(newFileName);

		await sourceStream.CopyToAsync(newFileStream);

		RecipeImage recipeImage = new();
		recipeImage.FileName = Path.GetFileName(newFileName);
		recipe!.Images.Add(recipeImage);
	}

	public ICommand SelectImageCommand => new Command(SelectImage);
	public async void SelectImage()
	{
		FileResult? photo = await MediaPicker.Default.PickPhotoAsync();
		if (photo != null && recipe != null)
		{
			AddFile(photo);
		}
	}

	public ICommand TakePhotoCommand => new Command(TakePhoto);
	public async void TakePhoto()
	{
		if (MediaPicker.Default.IsCaptureSupported)
		{
			FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
			if (photo != null && recipe != null)
			{
				AddFile(photo);
			}
		}	
	}

	public ICommand DeleteImageCommand => new Command(DeleteImage, () => selectedImage != null);
	public void DeleteImage()
	{
		if (selectedImage != null && recipe != null)
		{
			deletedImages.Add(selectedImage.FileName!);
			recipe.Images.Remove(selectedImage);
		}
	}

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		IsBusy = true;
		if (query.ContainsKey(nameof(Recipe)))
		{
			Recipe = query[nameof(Recipe)] as Recipe;
			if (Recipe != null)
			{
				EditedIngredients.Clear();
				foreach (Ingredient item in Recipe.Ingredients)
				{
					EditedIngredients.Add(new ObservableIngredient(item));
				}
			}
			query.Remove(nameof(Recipe));
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
			query.Remove(Constants.CheckedTagsParameter);
		}

		if (query.ContainsKey(Constants.SelectedIngredientsParameter))
		{
			List<Ingredient> selected = (List<Ingredient>)query[Constants.SelectedIngredientsParameter];
			foreach (Ingredient item in selected)
			{
				EditedIngredients.Add(new ObservableIngredient(item));
			}
			query.Remove(Constants.SelectedIngredientsParameter);
		}

		IsBusy = false;
	}
}
