using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ObjectiveC;
using System.Web;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class TagListViewModel : BaseViewModel, IQueryAttributable
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	public ObservableCollection<ObservableTag> Tags { get; } = [];

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

	public bool IsCheckAvailable
	{
		get => checkedIds != null;
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
				foreach (RecipeTag tag in Tags)
				{
					tag.IsChecked = (checkedIds != null) && checkedIds.IndexOf(tag.Id) >= 0;
				}
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCheckAvailable));
			}
		}
	}

	private ObservableTag? draggedTag;
	public ObservableTag? DraggedTag
	{
		get => draggedTag;
		set
		{
			if (draggedTag != value)
			{
				draggedTag = value;
				OnPropertyChanged();
			}
		}
	}

	private IRecipeService recipeService;

	public TagListViewModel(IRecipeService recipeService)
	{
		this.recipeService = recipeService;
		CheckedIds = null;
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
			{
				tag.IsChecked = (checkedIds != null) && (checkedIds.IndexOf(tag.Id) >= 0);
				ObservableTag extendedTag = new();
				extendedTag.CopyFrom(tag);
				extendedTag.BackgroundDragColor = GetBaseBackgroundDragColor();
				extendedTag.BackgroundSelectColor = GetBackgroundSelectionColor(extendedTag.IsChecked);
				Tags.Add(extendedTag);
			}
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
			keyboard: Keyboard.Create(KeyboardFlags.CapitalizeSentence));
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
	private bool CanEditTag(object obj)
	{
		return obj != null;
	}
	private async void EditTag(object obj)
	{
		if (obj == null || obj is not ObservableTag)
			return;
		ObservableTag? editedTag = obj as ObservableTag;
		if (editedTag != null)
		{
			string name = await Shell.Current.DisplayPromptAsync(
			"",
			$"{LocalizationManager["MsgAddingTag"]}",
			$"{LocalizationManager["Ok"]}",
			$"{LocalizationManager["Cancel"]}",
			keyboard: Keyboard.Create(KeyboardFlags.CapitalizeSentence),
			initialValue: $"{editedTag.Name}");
			if (name != null)
			{
				foreach (RecipeTag tag in Tags)
				{
					if (tag != editedTag && string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase))
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
					editedTag.Name = name;
					await recipeService.UpdateTagAsync(editedTag);
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
	}

	public ICommand DeleteTagCommand => new Command(DeleteTag, (object obj) => obj != null);
	private async void DeleteTag(object obj)
	{
		if (obj == null || obj is not ObservableTag)
			return;
		ObservableTag? deletedTag = obj as ObservableTag;
		if (deletedTag != null)
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
					await recipeService.DeleteTagAsync(deletedTag);
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

	public async void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(Constants.CheckedTagsParameter))
		{
			CheckedIds = new();
			List<int>? paramIds = query[Constants.CheckedTagsParameter] as List<int>;
			if (paramIds != null)
			{
				foreach (int id in paramIds)
				{
					CheckedIds.Add(id);
				}
			}
		}

		if (query.ContainsKey(Constants.SelectedColorParameter) && query.ContainsKey(Constants.TagIdParameter))
		{
			string? selectedColor = query[Constants.SelectedColorParameter].ToString();
			int tagId = Convert.ToInt32(query[Constants.TagIdParameter]);
			ObservableTag? currentTag = Tags.FirstOrDefault(x => x.Id == tagId);

			if (currentTag != null && !string.IsNullOrEmpty(selectedColor))
			{
				currentTag.Color = selectedColor;
				try
				{
					await recipeService.UpdateTagAsync(currentTag);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Error while updating tag's color. {ex.Message}");
					await Shell.Current.DisplayAlert(
						$"{LocalizationManager["Error"]}",
						$"{ex.Message}",
						$"{LocalizationManager["Ok"]}");
				}
			}
		}
	}

	public ICommand SaveCheckedCommand => new Command(SaveChecked);
	public async void SaveChecked()
	{
		try
		{
			Dictionary<string, object> navParam = new Dictionary<string, object>();
			if (checkedIds != null)
			{
				List<int> paramIds = new();
				foreach (RecipeTag tag in Tags)
				{
					if (tag.IsChecked)
					{
						paramIds.Add(tag.Id);
					}
				}
				navParam.Add(Constants.CheckedTagsParameter, paramIds);
			}
			await Shell.Current.GoToAsync("..", navParam);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Exception while returning selected tags. {ex.Message}");
		}
	}

	public ICommand SelectColorCommand => new Command(SelectColor, (object obj) => obj != null);
	public async void SelectColor(object obj)
	{
		if (obj == null || obj is not ObservableTag)
			return;
		ObservableTag? currentTag = obj as ObservableTag;
		Dictionary<string, object> navParam = new();
		if (currentTag != null)
			navParam.Add(Constants.TagIdParameter, currentTag.Id);
		await Shell.Current.GoToAsync(Constants.ColorSelectionRoute, navParam);
	}

	private Color GetBaseBackgroundDragColor()
	{
		Color result = (App.Current!.RequestedTheme == AppTheme.Light)
			? (Color)App.Current!.Resources["White"]
			: (Color)App.Current!.Resources["Black"];
		return result;
	}

	private Color GetBackgroundSelectionColor(bool isChecked)
	{
		Color result = (App.Current!.RequestedTheme == AppTheme.Light)
			? (Color)App.Current!.Resources["White"]
			: (Color)App.Current!.Resources["Black"];
		if (isChecked)
		{
			result = Colors.Aqua;
		}
		return result;
	}

	public ICommand DragTagStartingCommand => new Command(DragTagStarting);
	public void DragTagStarting(object obj)
	{
		DraggedTag = obj as ObservableTag;
	}

	public ICommand DragTagOverCommand => new Command(DragTagOver);
	public void DragTagOver(object obj)
	{
		ObservableTag? currentTag = obj as ObservableTag;
		if (currentTag != null && draggedTag != null && currentTag != draggedTag)
		{
			currentTag.BackgroundDragColor = Colors.LightSkyBlue;
		}
	}

	public ICommand DragTagLeaveCommand => new Command(DragTagLeave);
	public void DragTagLeave(object obj)
	{
		ObservableTag? currentTag = obj as ObservableTag;
		if (currentTag != null)
		{
			currentTag.BackgroundDragColor = GetBaseBackgroundDragColor();
		}
	}

	public ICommand DropTagCommand => new Command(DropTag);
	public async void DropTag(object obj)
	{
		ObservableTag? currentTag = obj as ObservableTag;
		if (currentTag != null && draggedTag != null)
		{
			currentTag.BackgroundDragColor = GetBaseBackgroundDragColor();
			draggedTag.BackgroundDragColor = GetBaseBackgroundDragColor();
			if (currentTag != draggedTag)
			{
				int oldIndex = Tags.IndexOf(draggedTag);
				int newIndex = Tags.IndexOf(currentTag);
				int minIndex = Math.Min(oldIndex, newIndex);
				int maxIndex = Math.Max(oldIndex, newIndex);
				Tags.Remove(draggedTag);
				Tags.Insert(Tags.IndexOf(currentTag), draggedTag);
				for (int i = minIndex; i <= maxIndex; i++)
				{
					Tags[i].SortOrder = i;
					await recipeService.UpdateTagAsync(Tags[i]);
				}
			}
			DraggedTag = null;
		}
	}

	public ICommand TapTagCommand => new Command(TapTag);
	public void TapTag(object obj)
	{
		if (checkedIds == null)
			return;

		ObservableTag? currentTag = obj as ObservableTag;
		if (currentTag != null)
		{
			currentTag.IsChecked = !currentTag.IsChecked;
			currentTag.BackgroundSelectColor = GetBackgroundSelectionColor(currentTag.IsChecked);
		}
	}
}
