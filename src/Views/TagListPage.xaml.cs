namespace Recipes.Views;

public partial class TagListPage : ContentPage
{
	public TagListPage(TagListViewModel tagListViewModel)
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