using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class ColorSelectionViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;
	public ObservableCollection<Color> Colors { get; } = [];

	private Color selectedColor;
	public Color SelectedColor
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
		//Colors.Add(Color.FromRgb(200, 200, 200));
		//Colors.Add(Color.FromRgb(240, 50, 240));
		//Colors.Add(Color.FromRgb(50, 240, 150));
		//Colors.Add(Color.FromRgb(50, 150, 240));
		Colors.Add(Color.FromArgb("#AB93DC"));
		Colors.Add(Color.FromArgb("#f5f3ae"));
		Colors.Add(Color.FromArgb("#b6ecc6"));
		Colors.Add(Color.FromArgb("#92b3e3"));
		Colors.Add(Color.FromArgb("94dfff"));
		Colors.Add(Color.FromArgb("#f1bcdb"));
		Colors.Add(Color.FromArgb("#ffc175"));
		Colors.Add(Color.FromArgb("#d59ce7"));
		Colors.Add(Color.FromArgb("#cba89a"));
		Colors.Add(Color.FromArgb("#9ee6ea"));
		Colors.Add(Color.FromArgb("#afde9c"));
		Colors.Add(Color.FromArgb("#f19288"));
		Colors.Add(Color.FromArgb("#8694f3"));
		Colors.Add(Color.FromArgb("#a0fee7"));
		Colors.Add(Color.FromArgb("#fbff9e"));
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
