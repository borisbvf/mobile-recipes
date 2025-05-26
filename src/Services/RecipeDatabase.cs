using SQLite;
using System.ComponentModel;
using static SQLite.SQLite3;

namespace Recipes.Services;

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
		"color TEXT, " +
		"sort_order INTEGER" +
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
		"sort_order INTEGER," +
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

	public bool ReconnectDB()
	{
		if (database != null)
		{
			database.CloseAsync();
			database = null;
		}
		database = new SQLiteAsyncConnection(Constants.DBPath, Constants.DBOpenFlags);
		return File.Exists(Constants.DBPath) && database != null;
	}

	private async Task ReloadRecipeTags(List<Recipe> recipes)
	{
		foreach (Recipe recipe in recipes)
		{
			List<RecipeTag> tags = await database!.QueryAsync<RecipeTag>("SELECT t.id, t.name, t.color FROM tags t " +
				$"INNER JOIN recipe_tag rt ON rt.tag_id = t.id WHERE rt.recipe_id = {recipe.Id}");
			foreach (RecipeTag tag in tags)
			{
				recipe.Tags.Add(tag);
			}
		}
	}

	public async Task<List<Recipe>> GetRecipeListAsync()
	{
		List<Recipe> result = await database!.QueryAsync<Recipe>("SELECT id, name, description, instructions, " +
			"prep_time as preparationtime, cook_time as cookingtime FROM recipes ORDER BY name");
		await ReloadRecipeTags(result);
		return result;
	}

	public async Task<List<Recipe>> GetRecipeListAsync(string? searchText, List<int>? tagIds, FilterCondition? tagCondition,
		List<int>? ingredientIds, FilterCondition? ingredientCondition)
	{
		bool isTagFilter = (tagIds?.Any() ?? false) && tagCondition != null;
		bool isIngredientFilter = (ingredientIds?.Any() ?? false) && ingredientCondition != null;

		string tagHavingSection = string.Empty;
		string ingredientHavingSection = string.Empty;

		if (isTagFilter)
		{
			if (tagCondition == FilterCondition.Any)
			{
				tagHavingSection = " AND COUNT(DISTINCT rt.tag_id) > 0 ";
			}
			else if (tagCondition == FilterCondition.All)
			{
				tagHavingSection = $" AND COUNT(DISTINCT rt.tag_id) = {tagIds!.Count()} ";
			}
			else if (tagCondition == FilterCondition.None)
			{
				tagHavingSection = " AND COUNT(DISTINCT rt.tag_id) = 0 ";
			}
		}
		if (isIngredientFilter)
		{
			if (ingredientCondition == FilterCondition.Any)
			{
				ingredientHavingSection = " AND COUNT(DISTINCT ri.ingredient_id) > 0 ";
			}
			else if (ingredientCondition == FilterCondition.All)
			{
				ingredientHavingSection = $" AND COUNT(DISTINCT ri.ingredient_id) = {ingredientIds!.Count()} ";
			}
			else if (ingredientCondition == FilterCondition.None)
			{
				ingredientHavingSection = " AND COUNT(DISTINCT ri.ingredient_id) = 0 ";
			}
		}

		string sql = "SELECT r.id, r.name, r.description, " +
			"r.prep_time as preparationtime, r.cook_time as cookingtime FROM recipes r " +
			(isTagFilter ? $"LEFT JOIN recipe_tag rt ON rt.recipe_id = r.id AND rt.tag_id IN ({string.Join(',', tagIds!)}) " : string.Empty) +
			(isIngredientFilter ? $"LEFT JOIN recipe_ingredient ri ON ri.recipe_id = r.id AND ri.ingredient_id IN ({string.Join(',', ingredientIds!)}) " : string.Empty) +
			$"WHERE (r.name LIKE '%{searchText}%' OR r.description LIKE '%{searchText}%' OR r.instructions LIKE '%{searchText}%') " +
			"GROUP BY r.id, r.name, r.description, r.prep_time, r.cook_time " +
			"HAVING 1 = 1 " + tagHavingSection + ingredientHavingSection +
			$"ORDER BY CASE WHEN r.name LIKE '%{searchText}%' THEN 0 ELSE 1 END, r.name";
		List<Recipe> result = await database!.QueryAsync<Recipe>(sql);
		await ReloadRecipeTags(result);
		return result;
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
			List<Ingredient> ingredients = await database!.QueryAsync<Ingredient>("SELECT t.id, t.name, link.comment, link.sort_order AS sortorder " +
				$"FROM ingredients t INNER JOIN recipe_ingredient link ON link.ingredient_id = t.id WHERE link.recipe_id = {recipeId} ORDER BY link.sort_order");
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

	public async Task<bool> CheckRecipeNameUnique(string name, int id)
	{
		int count = id != 0
			? await database!.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM recipes WHERE name = ? AND id <> {id}", name)
			: await database!.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM recipes WHERE name = ?", name);
		return count == 0;
	}

	public async Task<int> GetRecipeCount()
	{
		int count = await database!.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM recipes");
		return count;
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
					$"INSERT INTO recipe_ingredient (recipe_id, ingredient_id, comment, sort_order) VALUES ({lastRecipeId}, {ingredient.Id}, ?, {ingredient.SortOrder})",
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
				$"INSERT INTO recipe_ingredient (recipe_id, ingredient_id, comment, sort_order) VALUES ({recipe.Id}, {ingredient.Id}, ?, {ingredient.SortOrder})", 
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

	public async Task<List<Ingredient>> GetIngredientListAsync(List<int>? excludeIds = null)
	{
		if (excludeIds == null || excludeIds.Count == 0)
		{
			return await database!.QueryAsync<Ingredient>("SELECT id, name FROM ingredients ORDER BY name");
		}
		else
		{
			return await database!.QueryAsync<Ingredient>($"SELECT id, name FROM ingredients WHERE id NOT IN ({string.Join(',', excludeIds)}) ORDER BY name");
		}
	}
	public async Task<bool> CheckIngredientNameUnique(string name, int id)
	{
		int count = id != 0
			? await database!.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ingredients WHERE name = ? AND id <> {id}", name)
			: await database!.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ingredients WHERE name = ?", name);
		return count == 0;
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

	public async Task<List<RecipeTag>> GetTagListAsync(List<int>? ids = null)
	{
		if (ids == null || ids.Count == 0)
			return await database!.QueryAsync<RecipeTag>("SELECT id, name, color, sort_order as sortorder FROM tags ORDER BY sort_order");
		else
			return await database!.QueryAsync<RecipeTag>($"SELECT id, name, color FROM tags WHERE id IN ({string.Join(',', ids)}) ORDER BY sort_order");
	}
	public async Task AddTagAsync(RecipeTag recipeTag)
	{
		int rows = await database!.ExecuteAsync(
			"INSERT INTO tags (name, color, sort_order) VALUES (?, ?, ?)",
			recipeTag.Name,
			recipeTag.Color,
			recipeTag.SortOrder);
	}
	public async Task UpdateTagAsync(RecipeTag recipeTag)
	{
		int rows = await database!.ExecuteAsync(
			$"UPDATE tags SET name = ?, color = ?, sort_order = ? WHERE id = {recipeTag.Id}",
			recipeTag.Name,
			recipeTag.Color,
			recipeTag.SortOrder);
	}
	public async Task DeleteTagAsync(RecipeTag recipeTag)
	{
		int rows = await database!.ExecuteAsync($"DELETE FROM tags WHERE id = {recipeTag.Id}");
	}

	public async Task<List<string>> GetImageFilenamesAsync()
	{
		string sql = "SELECT ri.id, ri.filename FROM recipe_image ri INNER JOIN recipes r ON r.id = ri.recipe_id";
		List<RecipeImage> images = await database!.QueryAsync<RecipeImage>(sql);
		List<string> result = new();
		foreach (RecipeImage image in images)
		{
			result.Add(image.FileName!);
		}
		return result;
	}
}
