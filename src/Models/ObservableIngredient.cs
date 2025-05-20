using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.Models;
public class ObservableIngredient : Ingredient, INotifyPropertyChanged
{
	public ObservableIngredient()
	{

	}

	public ObservableIngredient(Ingredient source) : base(source)
	{
	}

	private Color? backgroundDragColor;
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

	private Color? backgroundSelectColor;
	public Color? BackgroundSelectColor
	{
		get => backgroundSelectColor;
		set
		{
			if (backgroundSelectColor != value)
			{
				backgroundSelectColor = value;
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
			BackgroundSelectColor = ((ObservableIngredient)source).BackgroundSelectColor;
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}