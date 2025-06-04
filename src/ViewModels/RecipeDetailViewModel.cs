using MauiCommonTools.Alerts;
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
		if (query.ContainsKey(Constants.RecipeIdParameter))
		{
			int recipeId = (int)query[Constants.RecipeIdParameter];
			Recipe = await recipeService.GetRecipeAsync(recipeId);
			IsTagsFrameVisible = Recipe?.Tags.Count > 0;
			IsImagesFrameVisible = Recipe?.Images.Count > 0;
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

	private bool isTagsFrameVisible;
	public bool IsTagsFrameVisible
	{
		get => isTagsFrameVisible;
		set
		{
			if (isTagsFrameVisible != value)
			{
				isTagsFrameVisible = value;
				OnPropertyChanged();
			}
		}
	}

	private bool isImagesFrameVisible;
	public bool IsImagesFrameVisible
	{
		get => isImagesFrameVisible;
		set
		{
			if (isImagesFrameVisible != value)
			{
				isImagesFrameVisible = value;
				OnPropertyChanged();
			}
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

	private string GetRecipeText(Recipe? source)
	{
		if (source == null)
			return string.Empty;

		string GetIngredients()
		{
			List<string> list = new();
			foreach (Ingredient item in source!.Ingredients)
			{
				list.Add(item.Name! + " " + item.Comment);
			}
			return list.Count > 0 ? string.Join(Environment.NewLine, list) + Environment.NewLine : string.Empty;
		}

		string GetTags()
		{
			List<string> list = new();
			foreach (RecipeTag item in source!.Tags)
			{
				list.Add(item.Name!);
			}
			return list.Count > 0 ? Environment.NewLine + string.Join(';', list) : string.Empty;
		}

		return source.Name + Environment.NewLine +
			source.Description + Environment.NewLine +
			GetIngredients() +
			source.Instructions +
			GetTags();
	}

	public ICommand CopyRecipeTextCommand => new Command(CopyRecipeText);
	private async void CopyRecipeText()
	{
		if (recipe == null)
			return;

		string recipeText = GetRecipeText(recipe);
		try
		{
			await Clipboard.Default.SetTextAsync(recipeText);
			IToast toast = Toast.Make($"{LocalizationManager["ClipboardCopiedMsg"]}");
			await toast.Show();
		}
		catch (Exception e)
		{
			await Shell.Current.DisplayAlert(
				$"{LocalizationManager["Error"]}",
				$"{e.Message}",
				$"{LocalizationManager["Ok"]}");
		}
	}

	public ICommand ShareRecipeCommand => new Command(ShareRecipe);
	private async void ShareRecipe()
	{
		if (recipe == null)
			return;

		try
		{
			string recipeText = GetRecipeText(recipe);
			await Share.Default.RequestAsync(new ShareTextRequest(recipeText));
		}
		catch (Exception e)
		{
			await Shell.Current.DisplayAlert(
				$"{LocalizationManager["Error"]}",
				$"{e.Message}",
				$"{LocalizationManager["Ok"]}");
		}
	}

	public ICommand ExportRecipeToPdfCommand => new Command(ExportRecipeToPdf);
	private async void ExportRecipeToPdf()
	{
		if (recipe == null)
			return;

		try
		{
			if (!Directory.Exists(Constants.PdfDirectory))
				Directory.CreateDirectory(Constants.PdfDirectory);
			string guid = $"_{Guid.NewGuid()}";
			string fileName = $"{recipe.Name}";
			int guidIndex = 1;
			while (File.Exists(Path.Combine(Constants.PdfDirectory, $"{fileName}.pdf")) && guidIndex < guid.Length)
			{
				fileName = $"{fileName}{guid[guidIndex]}";
				guidIndex++;
			}
			fileName = Path.Combine(Constants.PdfDirectory, $"{fileName}.pdf");

			RecipeExport.ExportToPdf(recipe, fileName);

			await Launcher.Default.OpenAsync(new OpenFileRequest($"{recipe.Name}", new ReadOnlyFile(fileName)));

			File.Delete(fileName);
		}
		catch (Exception e)
		{
			await Shell.Current.DisplayAlert(
				$"{LocalizationManager["Error"]}",
				$"{e.Message}",
				$"{LocalizationManager["Ok"]}");
		}
	}
}