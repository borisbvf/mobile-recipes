namespace Recipes.Views;

public partial class TagListView : ContentPage
{
	public TagListView(TagListViewModel tagListViewModel)
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