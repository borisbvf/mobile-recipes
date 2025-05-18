using Recipes.Views;

namespace Recipes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(Constants.SettingsPageRoute, typeof(SettingsPage));
            Routing.RegisterRoute(Constants.RecipeSearchPage, typeof(RecipeSearchPage));
			Routing.RegisterRoute(Constants.DetailPageRoute, typeof(RecipeDetailPage));
			Routing.RegisterRoute(Constants.EditPageRoute, typeof(RecipeEditPage));
            Routing.RegisterRoute(Constants.IngredientListRoute, typeof(IngredientListPage));
			Routing.RegisterRoute(Constants.IngredientSelectionRoute, typeof(IngredientSelectionPage));
			Routing.RegisterRoute(Constants.TagListRoute, typeof(TagListPage));
            Routing.RegisterRoute(Constants.TagSelectionRoute, typeof(TagSelectionPage));
            Routing.RegisterRoute(Constants.ColorSelectionRoute, typeof(ColorSelectionView));
            Routing.RegisterRoute(Constants.ImageStoreRoute, typeof(ImageStoreView));

			BindingContext = this;
        }

		public LocalizationManager LocalizationManager => LocalizationManager.Instance;
	}
}
