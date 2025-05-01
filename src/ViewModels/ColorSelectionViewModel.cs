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
		Colors.Add(Color.FromRgb(200, 200, 200));
		Colors.Add(Color.FromRgb(240, 50, 240));
		Colors.Add(Color.FromRgb(50, 240, 150));
		Colors.Add(Color.FromRgb(50, 150, 240));
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
