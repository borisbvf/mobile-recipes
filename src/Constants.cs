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
	public const string IngredientListRoute = "IngredientListView";
	public const string IngredientDetailRoute = "IngredientDetailView";
	public const string IngredientEditRoute = "IngredientEditView";
	public const string TagListRoute = "TagListView";


	public const int LoadingAnimationInterval = 3000;


	public const string DBFileName = "recipes.db3";
	public const SQLite.SQLiteOpenFlags DBOpenFlags =
		SQLite.SQLiteOpenFlags.ReadWrite |
		SQLite.SQLiteOpenFlags.Create |
		SQLite.SQLiteOpenFlags.SharedCache;
	public static string DBPath = Path.Combine(FileSystem.AppDataDirectory, DBFileName);
}
