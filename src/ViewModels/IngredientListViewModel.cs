using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class IngredientListViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<Ingredient> Ingredients { get; } = new();

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

	private IRecipeService recipeService;

	public IngredientListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	public ICommand GetIngredientsCommand => new Command(GetIngredientsAsync);
	public async void GetIngredientsAsync()
	{
		if (IsBusy)
			return;
		IsBusy = true;
		try
		{
			IEnumerable<Ingredient> result = await recipeService.GetIngredientListAsync();
			if (Ingredients.Count > 0)
				Ingredients.Clear();
			foreach (Ingredient ingredient in result)
				Ingredients.Add(ingredient);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get ingredients: {ex.Message}");
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
		Ingredient? ingredient = obj as Ingredient;
		if (ingredient != null)
		{
			var navParameter = new Dictionary<string, object> { { nameof(Ingredient), ingredient } };
			Shell.Current.GoToAsync(Constants.DetailPageRoute, navParameter);
		}
	}

	public ICommand AddIngredientCommand => new Command(AddIngredient);
	private async void AddIngredient()
	{
		string name = await Shell.Current.DisplayPromptAsync(
			"",
			$"{LocalizationManager["MsgAddingIngredient"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}",
			keyboard:Keyboard.Create(KeyboardFlags.CapitalizeSentence));
		if (name != null)
		{
			foreach (Ingredient ingredient in Ingredients)
			{
				if (string.Equals(ingredient.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{LocalizationManager["ErrorIngredientExists"]}",
						$"{LocalizationManager["Ok"]}");
					return;
				}
			}
			try
			{
				await recipeService.AddIngredientAsync(new Ingredient { Name = name });
				GetIngredientsAsync();
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"Unable to add ingredient: {ex.Message}");
				await Shell.Current.DisplayAlert(
					LocalizationManager["Error"].ToString(),
					ex.Message,
					LocalizationManager["Ok"].ToString());
			}
		}
	}

	public ICommand EditIngredientCommand => new Command(EditIngredient, CanEditIngredient);
	private bool CanEditIngredient()
	{
		return selectedIngredient != null;
	}
	private async void EditIngredient()
	{
		if (selectedIngredient != null)
		{
			string name = await Shell.Current.DisplayPromptAsync(
			"",
			$"{LocalizationManager["MsgAddingIngredient"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}",
			keyboard:Keyboard.Create(KeyboardFlags.CapitalizeSentence),
			initialValue:$"{selectedIngredient.Name}");
			if (name != null)
			{
				foreach (Ingredient ingredient in Ingredients)
				{
					if (ingredient != selectedIngredient && string.Equals(ingredient.Name, name, StringComparison.OrdinalIgnoreCase))
					{
						await Shell.Current.DisplayAlert(
							$"{LocalizationManager["Error"]}",
							$"{LocalizationManager["ErrorIngredientExists"]}",
							$"{LocalizationManager["Ok"]}");
						return;
					}
				}
				try
				{
					selectedIngredient.Name = name;
					await recipeService.UpdateIngredientAsync(selectedIngredient);
					GetIngredientsAsync();
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Unable to update ingredient: {ex.Message}");
					await Shell.Current.DisplayAlert(
						LocalizationManager["Error"].ToString(),
						ex.Message,
						LocalizationManager["Ok"].ToString());
				}
			}
		}
	}

	public ICommand DeleteIngredientCommand => new Command(DeleteIngredient, () => selectedIngredient != null);
	private async void DeleteIngredient()
	{
		if (selectedIngredient != null)
		{
			bool confirmed = await Shell.Current.DisplayAlert(
			"",
			$"{LocalizationManager["MsgDeletingIngredient"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}");
			if (confirmed)
			{
				try
				{
					await recipeService.DeleteIngredientAsync(selectedIngredient);
					GetIngredientsAsync();
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Unable to delete ingredient: {ex.Message}");
					await Shell.Current.DisplayAlert(
						LocalizationManager["Error"].ToString(),
						ex.Message,
						LocalizationManager["Ok"].ToString());
				}
			}
		}
	}

	public ICommand FinishSelectionCommand => new Command(FinishSelection);
	public async void FinishSelection()
	{
		Dictionary<string, object> navParam = new();
		if (selectedIngredient != null)
		{
			navParam.Add(nameof(Ingredient), selectedIngredient);
		}
		await Shell.Current.GoToAsync("..", navParam);
	}
}
