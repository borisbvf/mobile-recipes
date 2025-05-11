using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.Models;
public class ObservableIngredient : Ingredient, INotifyPropertyChanged
{
	private Color? backgroundDragColor;

	public ObservableIngredient(Ingredient source) : base(source)
	{
	}

	public Color? BackgroundDragColor
	{
		get => backgroundDragColor;
		set
		{
			if (value != backgroundDragColor)
			{
				backgroundDragColor = value;
				OnPropertyChanged();
			}
		}
	}

	public override void CopyFrom(Ingredient source)
	{
		base.CopyFrom(source);
		if (source is ObservableIngredient)
		{
			BackgroundDragColor = ((ObservableIngredient)source).BackgroundDragColor;
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}