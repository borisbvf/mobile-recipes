namespace Recipes;
public static class Constants
{
	public const string AppThemeKey = "AppTheme";
	public const string LanguageKey = "Language";


	public const string RussianLang = "Русский";
	public const string RussianAbrv = "ru";
	public const string EnglishLang = "English";
	public const string EnglishAbrv = "en-US";


	public const string MainPageRoute = "MainPage";
	public const string SettingsPageRoute = "SettingsPage";
	public const string LoadingPageRoute = "LoadingPage";
	public const string DetailPageRoute = "DetailPage";
	public const string EditPageRoute = "EditPage";
	public const string IngredientListRoute = "IngredientListPage";
	public const string IngredientSelectionRoute = "IngredientSelectionPage";
	public const string TagListRoute = "TagListView";
	public const string ColorSelectionRoute = "ColorSelectionView";
	public const string ImageStoreRoute = "ImageStoreView";


	public const int LoadingAnimationInterval = 3000;


	public const string DBFileName = "recipes.db3";
	public const SQLite.SQLiteOpenFlags DBOpenFlags =
		SQLite.SQLiteOpenFlags.ReadWrite |
		SQLite.SQLiteOpenFlags.Create |
		SQLite.SQLiteOpenFlags.SharedCache;
	public static string DBPath = Path.Combine(FileSystem.AppDataDirectory, DBFileName);
	public const string ImageDirectoryName = "DatabaseImages";
	public static string ImageDirectory = Path.Combine(FileSystem.AppDataDirectory, ImageDirectoryName);

	public const string CheckedTagsParameter = "CheckedTags";
	public const string SelectedColorParameter = "SelectedColor";
	public const string RecipeIdParameter = "RecipeId";
	public const string TagIdParameter = "TagId";
	public const string IngredientIdsParameter = "IngredientIds";
	public const string SelectedIngredientsParameter = "SelectedIngredients";
}
