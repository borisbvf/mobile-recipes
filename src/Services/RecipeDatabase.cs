using SQLite;
using System.ComponentModel;

namespace Recipes.Services
{
    public class RecipeDatabase : IRecipeService
    {
		SQLiteAsyncConnection? database;

		private const string DBRecipesScript = "CREATE TABLE recipes (" +
			"id INTEGER PRIMARY KEY," +
			"name TEXT NOT NULL UNIQUE," +
			"description TEXT NOT NULL," +
			"instructions TEXT NOT NULL," +
			"prep_time INTEGER," +
			"cook_time INTEGER" +
			");";
		private const string DBIngredientsScript = "CREATE TABLE ingredients (" +
			"id INTEGER PRIMARY KEY," +
			"name TEXT NOT NULL UNIQUE" +
			");";
		private const string DBRecipeTagScript = "CREATE TABLE tags (" +
			"id INTEGER PRIMARY KEY," +
			"name TEXT NOT NULL UNIQUE," +
			"color TEXT NOT NULL" +
			");";
		private const string DBRecipeTagLinkScript = "CREATE TABLE recipe_tag (" +
			"recipe_id INTEGER," +
			"tag_id INTEGER," +
			"PRIMARY KEY (recipe_id, tag_id)," +
			"FOREIGN KEY (recipe_id) REFERENCES recipes (id) ON DELETE CASCADE ON UPDATE CASCADE," +
			"FOREIGN KEY (tag_id) REFERENCES tags (id) ON DELETE CASCADE ON UPDATE CASCADE" +
			");";
		private const string DBRecipeIngredientLinkScript = "CREATE TABLE recipe_ingredient (" +
			"recipe_id INTEGER," +
			"ingredient_id INTEGER," +
			"comment TEXT," +
			"PRIMARY KEY (recipe_id, ingredient_id)," +
			"FOREIGN KEY (recipe_id) REFERENCES recipes (id) ON DELETE CASCADE ON UPDATE CASCADE," +
			"FOREIGN KEY (ingredient_id) REFERENCES ingredients (id) ON DELETE CASCADE ON UPDATE CASCADE" +
			");";
		private const string DBRecipeImageLinkScript = "CREATE TABLE recipe_images (" +
			"id INTEGER PRIMARY KEY," +
			"recipe_id INTEGER," +
			"data BLOB NOT NULL," +
			"description TEXT," +
			"FOREIGN KEY (recipe_id) REFERENCES recipes (id) ON DELETE CASCADE ON UPDATE CASCADE" +
			");";
		private static readonly string[] DBScripts = { 
			DBRecipesScript, 
			DBIngredientsScript, 
			DBRecipeTagScript, 
			DBRecipeTagLinkScript, 
			DBRecipeIngredientLinkScript, 
			DBRecipeImageLinkScript };

		public bool CheckIfDBExists()
		{
			if (database == null)
			{
				database = new SQLiteAsyncConnection(Constants.DBPath, Constants.DBOpenFlags);
			}
			return File.Exists(Constants.DBPath);
		}

		public async Task CreateTables()
		{
			foreach (string script in DBScripts)
			{
				await database.ExecuteAsync(script);
			}
		}

		public async Task<IEnumerable<Recipe>> GetRecipeListAsync()
		{
			return await database!.QueryAsync<Recipe>("SELECT id, name, description, instructions FROM recipes");
		}

		public async Task<Recipe> GetRecipeAsync(int recipeId)
		{
			var data = await database!.QueryAsync<Recipe>("SELECT id, name, description, instructions FROM recipes WHERE id = id");
			return data.FirstOrDefault();
		}

		public async Task AddRecipeAsync(Recipe recipe)
		{
			int rows = await database!.ExecuteAsync(
				"INSERT INTO recipes (name, description, instructions) VALUES (?, ?, ?)",
				recipe.Name,
				recipe.Description,
				recipe.Content);
		}
		public async Task UpdateRecipeAsync(Recipe recipe)
		{
			int rows = await database!.ExecuteAsync(
				$"UPDATE recipes SET name = ?, description = ? WHERE id = {recipe.Id}",
				recipe.Name,
				recipe.Description);
		}
		public async Task DeleteRecipeAsync(Recipe recipe)
		{
			int rows = await database!.ExecuteAsync(
				$"DELETE FROM recipes WHERE id = {recipe.Id}");
		}

		public async Task<IEnumerable<Ingredient>> GetIngredientListAsync()
		{
			return await database!.QueryAsync<Ingredient>("SELECT id, name FROM ingredients");
		}
		public async Task<Ingredient> GetIngredientAsync()
		{
			var data = await database!.QueryAsync<Ingredient>("SELECT id, name FROM ingredient WHERE 1 = 1");
			return data.FirstOrDefault();
		}
		public async Task AddIngredientAsync(Ingredient ingredient)
		{
			int rows = await database!.ExecuteAsync(
				"INSERT INTO ingredients (name) VALUES (?)",
				ingredient.Name);
		}
		public async Task UpdateIngredientAsync(Ingredient ingredient)
		{
			int rows = await database!.ExecuteAsync(
				$"UPDATE ingredients SET name = ? WHERE id = {ingredient.Id}",
				ingredient.Name);
		}
		public async Task DeleteIngredientAsync(Ingredient ingredient)
		{
			int rows = await database!.ExecuteAsync(
				$"DELETE FROM ingredients WHERE id = {ingredient.Id}");
		}
	}
}
