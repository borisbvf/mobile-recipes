using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class ColorSelectionViewModel : BaseViewModel, IQueryAttributable
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;
	public double RectangleWidth
	{
		get => DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density / 3 - 20;
	}
	public double RectangleHeight
	{
		get => (DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density) * 0.8 / (Math.Ceiling(Colors.Count / 3.0)) - 20;
	}
	public ObservableCollection<Color> Colors { get; } = [];

	private int tagId;

	private Color? selectedColor;
	public Color? SelectedColor
	{
		get => selectedColor;
		set
		{
			if (value != selectedColor)
			{
				selectedColor = value;
				OnPropertyChanged();
			}
		}
	}

	public ColorSelectionViewModel()
	{
		Colors.Add(Color.FromArgb("#a2a2a2"));
		Colors.Add(Color.FromArgb("#d86c6c"));
		Colors.Add(Color.FromArgb("#F19288"));
		Colors.Add(Color.FromArgb("#fdb795"));

		Colors.Add(Color.FromArgb("#CBA89A"));
		Colors.Add(Color.FromArgb("#FFC175"));
		Colors.Add(Color.FromArgb("#f1e397"));
		Colors.Add(Color.FromArgb("#e4f197"));

		Colors.Add(Color.FromArgb("#97f1a3"));
		Colors.Add(Color.FromArgb("#64b27c"));
		Colors.Add(Color.FromArgb("#AFDE9C"));
		Colors.Add(Color.FromArgb("#87e1cb"));

		Colors.Add(Color.FromArgb("#68b2d1"));
		Colors.Add(Color.FromArgb("#97d3e4"));
		Colors.Add(Color.FromArgb("#94DFFF"));
		Colors.Add(Color.FromArgb("#97bef1"));

		Colors.Add(Color.FromArgb("#8694F3"));
		Colors.Add(Color.FromArgb("#A495E6"));
		Colors.Add(Color.FromArgb("#d497f1"));
		Colors.Add(Color.FromArgb("#f197ca"));
	}

	public ICommand FinishSelectionCommand => new Command(FinishSelection);
	public async void FinishSelection()
	{
		Dictionary<string, object> navParam = new();
		if (selectedColor != null)
			navParam.Add(Constants.SelectedColorParameter, selectedColor.ToArgbHex());
		navParam.Add(Constants.TagIdParameter, tagId);
		await Shell.Current.GoToAsync("..", navParam);
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey(nameof(RecipeTag)))
		{
			RecipeTag? tag = query[nameof(RecipeTag)] as RecipeTag;
			if (tag != null)
			{
				tagId = tag.Id;
			}
			query.Remove(nameof(RecipeTag));
		}
	}
}
