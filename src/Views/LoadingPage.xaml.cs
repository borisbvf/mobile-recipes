using Recipes.ViewModels;

namespace Recipes.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingViewModel loadingViewModel)
	{
		InitializeComponent();

		BindingContext = loadingViewModel;
		Appearing += async (s, e) =>
		{
			await loadingViewModel.InitAsync();
		};
	}
}