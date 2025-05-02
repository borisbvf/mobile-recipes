using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class ColorSelectionViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;
	public ObservableCollection<Color> Colors { get; } = [];

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
		Colors.Add(Color.FromArgb("#F1BCDB"));
		Colors.Add(Color.FromArgb("#F19288"));
		Colors.Add(Color.FromArgb("#FFC175"));
		Colors.Add(Color.FromArgb("#FBFF9E"));
		Colors.Add(Color.FromArgb("#F5F3AE"));
		Colors.Add(Color.FromArgb("#AFDE9C"));
		Colors.Add(Color.FromArgb("#A0FEE7"));
		Colors.Add(Color.FromArgb("#B6ECC6"));
		Colors.Add(Color.FromArgb("#9EE6EA"));
		Colors.Add(Color.FromArgb("94DFFF"));
		Colors.Add(Color.FromArgb("#92B3E3"));
		Colors.Add(Color.FromArgb("#8694F3"));
		Colors.Add(Color.FromArgb("#AB93DC"));
		Colors.Add(Color.FromArgb("#D59CE7"));
		Colors.Add(Color.FromArgb("#CBA89A"));
	}

	public ICommand FinishSelectionCommand => new Command(FinishSelection);
	public async void FinishSelection()
	{
		Dictionary<string, object> navParam = new();
		if (selectedColor != null)
			navParam.Add(Constants.SelectedColorParameter, selectedColor.ToArgbHex());
		await Shell.Current.GoToAsync("..", navParam);
	}
}
