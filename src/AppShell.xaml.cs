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

			BindingContext = this;
        }
    }
}
