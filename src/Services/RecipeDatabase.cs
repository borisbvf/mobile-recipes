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
			"color TEXT" +
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
		private const string DBRecipeImageLinkScript = "CREATE TABLE recipe_image (" +
			"id INTEGER PRIMARY KEY," +
			"recipe_id INTEGER," +
			"filename TEXT NOT NULL," +
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
				await database!.ExecuteAsync(script);
			}
		}

		public async Task<IEnumerable<Recipe>> GetRecipeListAsync()
		{
			return await database!.QueryAsync<Recipe>("SELECT id, name, description, instructions, " +
				"prep_time as preparationtime, cook_time as cookingtime FROM recipes");
		}

		public async Task<Recipe?> GetRecipeAsync(int recipeId)
		{
			var data = await database!.QueryAsync<Recipe>("SELECT id, name, description, instructions, " +
				$"prep_time as preparationtime, cook_time as cookingtime FROM recipes WHERE id = {recipeId}");
			Recipe? recipe = data.FirstOrDefault();
			if (recipe != null)
			{
				List<RecipeTag> tags = await database!.QueryAsync<RecipeTag>("SELECT t.id, t.name, t.color FROM tags t " +
					$"INNER JOIN recipe_tag rt ON rt.tag_id = t.id WHERE rt.recipe_id = {recipeId}");
				foreach (RecipeTag tag in tags)
				{
					recipe.Tags.Add(tag);
				}
				List<Ingredient> ingredients = await database!.QueryAsync<Ingredient>("SELECT t.id, t.name, link.comment FROM ingredients t " +
					$"INNER JOIN recipe_ingredient link ON link.ingredient_id = t.id WHERE link.recipe_id = {recipeId}");
				foreach (Ingredient ingredient in ingredients)
				{
					recipe.Ingredients.Add(ingredient);
				}
				List<RecipeImage> images = await database!.QueryAsync<RecipeImage>(
					$"SELECT t.id, t.description, t.filename FROM recipe_image t WHERE t.recipe_id = {recipeId}");
				foreach (RecipeImage image in images)
				{
					recipe.Images.Add(image);
				}
			}
			return recipe;
		}

		public async Task AddRecipeAsync(Recipe recipe)
		{
			int rows = await database!.ExecuteAsync(
				"INSERT INTO recipes (name, description, instructions, prep_time, cook_time) VALUES (?, ?, ?, ?, ?)",
				recipe.Name,
				recipe.Description,
				recipe.Instructions,
				recipe.PreparationTime,
				recipe.CookingTime);
			if (recipe.Tags?.Count > 0)
			{
				int lastRecipeId = await database!.ExecuteScalarAsync<int>("select last_insert_rowid()");
				foreach (RecipeTag tag in recipe.Tags)
				{
					await database!.ExecuteAsync($"INSERT INTO recipe_tag (recipe_id, tag_id) VALUES ({lastRecipeId}, {tag.Id})");
				}
				foreach (Ingredient ingredient in recipe.Ingredients)
				{
					await database!.ExecuteAsync(
						$"INSERT INTO recipe_ingredient (recipe_id, ingredient_id, comment) VALUES ({lastRecipeId}, {ingredient.Id}, ?)",
						ingredient.Comment);
				}
				foreach (RecipeImage image in recipe.Images)
				{
					await database!.ExecuteAsync(
						$"INSERT INTO recipe_image (recipe_id, description, filename VALUES ({lastRecipeId}, ?, ?)",
						image.Description,
						image.FileName);
				}
			}
		}
		public async Task UpdateRecipeAsync(Recipe recipe)
		{
			int rows = await database!.ExecuteAsync(
				$"UPDATE recipes SET name = ?, description = ?, instructions = ?, prep_time = ?, cook_time = ? WHERE id = {recipe.Id}",
				recipe.Name,
				recipe.Description,
				recipe.Instructions,
				recipe.PreparationTime,
				recipe.CookingTime);
			await database!.ExecuteAsync($"DELETE FROM recipe_tag WHERE recipe_id = {recipe.Id}");
			foreach (RecipeTag tag in recipe.Tags)
			{
				await database!.ExecuteAsync($"INSERT INTO recipe_tag (recipe_id, tag_id) VALUES ({recipe.Id}, {tag.Id})");
			}
			await database!.ExecuteAsync($"DELETE FROM recipe_ingredient WHERE recipe_id = {recipe.Id}");
			foreach (Ingredient ingredient in recipe.Ingredients)
			{
				await database!.ExecuteAsync(
					$"INSERT INTO recipe_ingredient (recipe_id, ingredient_id, comment) VALUES ({recipe.Id}, {ingredient.Id}, ?)", 
					ingredient.Comment);
			}
			await database!.ExecuteAsync($"DELETE FROM recipe_image WHERE recipe_id = {recipe.Id}");
			foreach (RecipeImage image in recipe.Images)
			{
				await database!.ExecuteAsync(
					$"INSERT INTO recipe_image (recipe_id, description, filename) VALUES ({recipe.Id}, ?, ?)",
					image.Description,
					image.FileName);
			}
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

		public async Task<IEnumerable<RecipeTag>> GetTagListAsync()
		{
			return await database!.QueryAsync<RecipeTag>("SELECT id, name, color FROM tags");
		}
		public async Task<IEnumerable<RecipeTag>> GetTagListAsync(List<int> ids)
		{
			return await database!.QueryAsync<RecipeTag>($"SELECT id, name, color FROM tags WHERE id IN ({string.Join(',', ids)})");
		}
		public async Task AddTagAsync(RecipeTag recipeTag)
		{
			int rows = await database!.ExecuteAsync(
				"INSERT INTO tags (name, color) VALUES (?, ?)",
				recipeTag.Name,
				recipeTag.Color);
		}
		public async Task UpdateTagAsync(RecipeTag recipeTag)
		{
			int rows = await database!.ExecuteAsync(
				$"UPDATE tags SET name = ?, color = ? WHERE id = {recipeTag.Id}",
				recipeTag.Name,
				recipeTag.Color);
		}
		public async Task DeleteTagAsync(RecipeTag recipeTag)
		{
			int rows = await database!.ExecuteAsync($"DELETE FROM tags WHERE id = {recipeTag.Id}");
		}
	}
}
