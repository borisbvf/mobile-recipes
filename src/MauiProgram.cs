using Recipes.ViewModels;
using Recipes.Views;
using Microsoft.Extensions.Logging;

namespace Recipes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
			builder.Services.AddSingleton<IRecipeService, RecipeDatabase>();
			builder.Services.AddTransient<RecipeListViewModel>();
			builder.Services.AddTransient<RecipeListPage>();
			builder.Services.AddTransient<RecipeDetailViewModel>();
			builder.Services.AddTransient<RecipeDetailPage>();
			builder.Services.AddTransient<RecipeEditViewModel>();
			builder.Services.AddTransient<RecipeEditPage>();
			builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<IngredientListViewModel>();
            builder.Services.AddTransient<IngredientListPage>();
            builder.Services.AddTransient<IngredientSelectionPage>();
            builder.Services.AddTransient<TagListViewModel>();
            builder.Services.AddTransient<TagListPage>();
            builder.Services.AddTransient<ColorSelectionViewModel>();
            builder.Services.AddTransient<ColorSelectionView>();
            builder.Services.AddTransient<ImageStoreView>();
            return builder.Build();
        }
    }
}
