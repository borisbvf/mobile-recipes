namespace Recipes.Views;

public partial class TagSelectionPage : ContentPage
{
	public TagSelectionPage(TagListViewModel tagListViewModel)
	{
		InitializeComponent();
		BindingContext = tagListViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as TagListViewModel)?.GetTagsCommand.Execute(this);
	}
}