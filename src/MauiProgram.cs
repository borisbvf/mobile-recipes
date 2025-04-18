﻿using Recipes.ViewModels;
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
			builder.Services.AddSingleton<IRecipeService, RecipeService>();
			builder.Services.AddTransient<RecipeListViewModel>();
			builder.Services.AddTransient<RecipeListView>();
			builder.Services.AddTransient<RecipeDetailViewModel>();
			builder.Services.AddTransient<RecipeDetailView>();
			builder.Services.AddTransient<RecipeEditViewModel>();
			builder.Services.AddTransient<RecipeEditView>();
			builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<LoadingPage>();
            return builder.Build();
        }
    }
}
