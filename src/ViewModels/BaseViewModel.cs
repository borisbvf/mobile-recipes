using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.ViewModels;
public class BaseViewModel : INotifyPropertyChanged
{
	private bool isBusy;
	private string? title;

	public bool IsBusy
	{
		get => isBusy;
		set
		{
			if (isBusy != value)
			{
				isBusy = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsEnabled));
			}
		}
	}

	public bool IsEnabled { get => !isBusy; }

	public string Title
	{
		get => title ?? string.Empty;
		set
		{
			if (title != value)
			{
				title = value;
				OnPropertyChanged();
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}
}
