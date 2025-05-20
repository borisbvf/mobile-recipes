using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class IngredientListViewModel : BaseViewModel, IQueryAttributable
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<ObservableIngredient> Ingredients { get; } = new();

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

	private List<int>? exclusionIds;
	public List<int>? ExclusionIds
	{
		get => exclusionIds;
		set
		{
			if (value != exclusionIds)
			{
				exclusionIds = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCheckAvailable));
			}
		}
	}

	public bool IsCheckAvailable
	{
		get => checkedIds != null || exclusionIds != null;
	}

	private List<int>? checkedIds;
	public List<int>? CheckedIds
	{
		get => checkedIds;
		set
		{
			if (value != checkedIds)
			{
				checkedIds = value;
				foreach (Ingredient item in Ingredients)
				{
					item.IsChecked = (checkedIds != null) && checkedIds.IndexOf(item.Id) >= 0;
				}
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCheckAvailable));
			}
		}
	}

	public IList<object>? SelectedIngredients { get; set; }


	private IRecipeService recipeService;

	public IngredientListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
		exclusionIds = null;
		checkedIds = null;
	}

	public ICommand GetIngredientsCommand => new Command(GetIngredientsAsync);
	public async void GetIngredientsAsync()
	{
		if (IsBusy)
			return;
		IsBusy = true;
		try
		{
			IEnumerable<Ingredient> result = await recipeService.GetIngredientListAsync(exclusionIds);
			if (Ingredients.Count > 0)
				Ingredients.Clear();
			foreach (Ingredient ingredient in result)
			{
				ingredient.IsChecked = checkedIds != null && checkedIds.IndexOf(ingredient.Id) >= 0;

				ObservableIngredient observableIngredient = new();
				observableIngredient.CopyFrom(ingredient);
				observableIngredient.BackgroundSelectColor = ColorHelper.GetBackgroundSelectionColor(observableIngredient.IsChecked);

				Ingredients.Add(observableIngredient);
			}
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
			bool isNameUnique = await recipeService.CheckIngredientNameUnique(name, 0);
			if (!isNameUnique)
			{
				await Shell.Current.DisplayAlert(
					$"{LocalizationManager["Error"]}",
					$"{LocalizationManager["ErrorIngredientExists"]}",
					$"{LocalizationManager["Ok"]}");
				return;
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
	private bool CanEditIngredient(object obj)
	{
		return obj != null;
	}
	private async void EditIngredient(object obj)
	{
		Ingredient? item = (Ingredient)obj;
		if (item != null)
		{
			string name = await Shell.Current.DisplayPromptAsync(
				"",
				$"{LocalizationManager["MsgAddingIngredient"]}",
				$"{LocalizationManager["Ok"]}",
				$"{LocalizationManager["Cancel"]}",
				keyboard:Keyboard.Create(KeyboardFlags.CapitalizeSentence),
				initialValue:$"{item.Name}");
			if (name != null)
			{
				bool isNameUnique = await recipeService.CheckIngredientNameUnique(name, item.Id);
				if (!isNameUnique)
				{
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{LocalizationManager["ErrorIngredientExists"]}",
						$"{LocalizationManager["Ok"]}");
					return;
				}
				try
				{
					item.Name = name;
					await recipeService.UpdateIngredientAsync(item);
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

	public ICommand DeleteIngredientCommand => new Command(DeleteIngredient, (object obj) => obj != null);
	private async void DeleteIngredient(object obj)
	{
		Ingredient? item = (Ingredient)obj;
		if (item != null)
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
					await recipeService.DeleteIngredientAsync(item);
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
		if (IsCheckAvailable)
		{
			List<Ingredient> list = new();
			foreach (Ingredient item in Ingredients)
			{
				if (item.IsChecked)
				{
					list.Add(item);
				}
			}
			navParam.Add(Constants.SelectedIngredientsParameter, list);
		}
		await Shell.Current.GoToAsync("..", navParam);
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(Constants.ExcludedIngredientIdsParameter))
		{
			exclusionIds = (List<int>)query[Constants.ExcludedIngredientIdsParameter];
		}

		if (query.ContainsKey(Constants.CheckedIngredientsParameter))
		{
			CheckedIds = new();
			List<int>? paramIds = query[Constants.CheckedIngredientsParameter] as List<int>;
			if (paramIds != null)
			{
				foreach (int id in paramIds)
				{
					CheckedIds.Add(id);
				}
			}
		}
	}

	public ICommand TapIngredientCommand => new Command(TapIngredient);
	public void TapIngredient(object obj)
	{
		if (!IsCheckAvailable)
			return;

		ObservableIngredient? current = obj as ObservableIngredient;
		if (current != null)
		{
			current.IsChecked = !current.IsChecked;
			current.BackgroundSelectColor = ColorHelper.GetBackgroundSelectionColor(current.IsChecked);
		}
	}
}
