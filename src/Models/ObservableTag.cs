using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.Models;
public class ObservableTag : RecipeTag, INotifyPropertyChanged
{
	private Color? backgroundDragColor;
	public Color? BackgroundDragColor
	{
		get => backgroundDragColor;
		set
		{
			if (backgroundDragColor != value)
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

	public override object Clone()
	{
		return new ObservableTag
		{
			Id = Id,
			Name = Name,
			Color = Color,
			IsChecked = IsChecked,
			SortOrder = SortOrder,
			BackgroundDragColor = BackgroundDragColor,
			BackgroundSelectColor = BackgroundSelectColor
		};
	}

	public override void CopyFrom(RecipeTag source)
	{
		base.CopyFrom(source);
		if (source != null && source is ObservableTag)
		{
			BackgroundDragColor = (source as ObservableTag)?.BackgroundDragColor;
			BackgroundSelectColor = (source as ObservableTag)?.BackgroundSelectColor;
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}
