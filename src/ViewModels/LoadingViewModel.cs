namespace Recipes.ViewModels;

public class LoadingViewModel : BaseViewModel
{
	public LoadingViewModel()
	{
		Random rnd = new Random();
		animationFileName = $"anim_{rnd.Next(21, 31)}";
	}

	public async Task InitAsync()
	{
		IsBusy = true;
		try
		{
			await Task.Delay(Constants.LoadingAnimationInterval);
			await Shell.Current.GoToAsync($"//{Constants.MainPageRoute}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private string? animationFileName;
	public string? AnimationFileName
	{
		get => animationFileName;
		set
		{
			animationFileName = value;
			OnPropertyChanged();
		}
	}
}
