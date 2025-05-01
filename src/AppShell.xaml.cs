using Recipes.Views;

namespace Recipes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(Constants.SettingsPageRoute, typeof(SettingsPage));
			Routing.RegisterRoute(Constants.DetailPageRoute, typeof(RecipeDetailView));
			Routing.RegisterRoute(Constants.EditPageRoute, typeof(RecipeEditView));
            Routing.RegisterRoute(Constants.IngredientListRoute, typeof(IngredientListView));
            Routing.RegisterRoute(Constants.TagListRoute, typeof(TagListView));
            Routing.RegisterRoute(Constants.ColorSelectionRoute, typeof(ColorSelectionView));

			BindingContext = this;
        }
    }
}
