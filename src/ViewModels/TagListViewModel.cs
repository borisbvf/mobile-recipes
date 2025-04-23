using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class TagListViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<RecipeTag> Tags { get; } = [];

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

	private RecipeTag selectedTag;
	public RecipeTag SelectedTag
	{
		get => selectedTag;
		set
		{
			if (selectedTag != value)
			{
				selectedTag = value;
				OnPropertyChanged();
			}
		}
	}

	private IRecipeService recipeService;

	public TagListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
	}

	public ICommand GetTagsCommand => new Command(GetTagListAsync);
	public async void GetTagListAsync()
	{
		if (IsBusy)
			return;
		IsBusy = true;
		try
		{
			IEnumerable<RecipeTag> result = await recipeService.GetTagListAsync();
			if (Tags.Count > 0)
				Tags.Clear();
			foreach (RecipeTag tag in result)
				Tags.Add(tag);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Unable to get tags: {ex.Message}");
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

	public ICommand AddTagCommand => new Command(AddTag);
	private async void AddTag()
	{
		string name = await Shell.Current.DisplayPromptAsync(
			"",
			$"{LocalizationManager["MsgAddingTag"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}",
			"");
		if (name != null)
		{
			foreach (RecipeTag tag in Tags)
			{
				if (string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{LocalizationManager["ErrorTagExists"]}",
						$"{LocalizationManager["Ok"]}");
					return;
				}
			}
			try
			{
				await recipeService.AddTagAsync(new RecipeTag { Name = name });
				GetTagListAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to add tag: {ex.Message}");
				await Shell.Current.DisplayAlert(
					LocalizationManager["Error"].ToString(),
					ex.Message,
					LocalizationManager["Ok"].ToString());
			}
		}
	}

	public ICommand EditTagCommand => new Command(EditTag, CanEditTag);
	private bool CanEditTag()
	{
		return selectedTag != null;
	}
	private async void EditTag()
	{
		string name = await Shell.Current.DisplayPromptAsync(
			"",
			$"{LocalizationManager["MsgAddingTag"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}",
			$"{selectedTag.Name}");
		if (name != null)
		{
			foreach (RecipeTag tag in Tags)
			{
				if (tag != selectedTag && string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{LocalizationManager["ErrorTagExists"]}",
						$"{LocalizationManager["Ok"]}");
					return;
				}
			}
			try
			{
				selectedTag.Name = name;
				await recipeService.UpdateTagAsync(selectedTag);
				GetTagListAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to update tag: {ex.Message}");
				await Shell.Current.DisplayAlert(
					LocalizationManager["Error"].ToString(),
					ex.Message,
					LocalizationManager["Ok"].ToString());
			}
		}
	}

	public ICommand DeleteTagCommand => new Command(DeleteTag, () => selectedTag != null);
	private async void DeleteTag()
	{
		bool confirmed = await Shell.Current.DisplayAlert(
			"",
			$"{LocalizationManager["MsgDeletingTag"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}");
		if (confirmed)
		{
			try
			{
				await recipeService.DeleteTagAsync(selectedTag);
				GetTagListAsync();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to delete tag: {ex.Message}");
				await Shell.Current.DisplayAlert(
					LocalizationManager["Error"].ToString(),
					ex.Message,
					LocalizationManager["Ok"].ToString());
			}
		}
	}
}
