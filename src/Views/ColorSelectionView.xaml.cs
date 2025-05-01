namespace Recipes.Views;

public partial class ColorSelectionView : ContentPage
{
	public ColorSelectionView(ColorSelectionViewModel colorSelectionViewModel)
	{
		InitializeComponent();
		BindingContext = colorSelectionViewModel;
	}
}