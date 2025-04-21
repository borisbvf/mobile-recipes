using SQLite;
using System.ComponentModel;

namespace Recipes.Services
{
    public class RecipeDatabase : IRecipeService
    {
		SQLiteAsyncConnection? database;
		private async Task Init()
		{
			if (database is null)
			{
				database = new SQLiteAsyncConnection(Constants.DBPath, Constants.DBOpenFlags);
				var result = await database.CreateTableAsync<Recipe>();
			}
		}
		public async Task<RequestResult<IEnumerable<Recipe>>> GetRecipesAsync()
		{
			await Init();
			RequestResult<IEnumerable<Recipe>> result = new();
			try
			{
				result.Data = await database!.Table<Recipe>().ToListAsync();
				result.IsSuccess = true;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = ex.Message;
				result.IsSuccess = false;
			}
			return result;
		}
		public async Task<RequestResult> AddRecipeAsync(Recipe recipe)
		{
			await Init();
			RequestResult result = new();
			try
			{
				int rows = await database!.InsertAsync(recipe);
				result.IsSuccess = rows > 0;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = ex.Message;
				result.IsSuccess = false;
			}
			return result;
		}
		public async Task<RequestResult> UpdateRecipeAsync(Recipe recipe)
		{
			await Init();
			RequestResult result = new();
			try
			{
				int rows = await database!.UpdateAsync(recipe);
				result.IsSuccess = rows > 0;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = ex.Message;
				result.IsSuccess = false;
			}
			return result;
		}
		public async Task<RequestResult> DeleteRecipeAsync(Recipe recipe)
		{
			await Init();
			RequestResult result = new();
			try
			{
				int rows = await database!.DeleteAsync(recipe);
				result.IsSuccess = rows > 0;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = ex.Message;
				result.IsSuccess = false;
			}
			return result;
		}
	}
}
