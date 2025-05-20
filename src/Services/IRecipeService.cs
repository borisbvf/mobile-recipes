namespace Recipes.Services;
public interface IRecipeService
{
	public bool CheckIfDBExists();
	public Task CreateTables();

	public Task<List<Recipe>> GetRecipeListAsync();
	public Task<List<Recipe>> GetRecipeListAsync(string? searchText, IEnumerable<int>? tagIds, IEnumerable<int>? ingredientIds);
	public Task<Recipe?> GetRecipeAsync(int recipeId);
	public Task<bool> CheckRecipeNameUnique(string name, int id);
	public Task AddRecipeAsync(Recipe recipe);
	public Task UpdateRecipeAsync(Recipe recipe);
	public Task DeleteRecipeAsync(Recipe recipe);

	public Task<List<Ingredient>> GetIngredientListAsync(List<int>? excludeIds = null);
	public Task<bool> CheckIngredientNameUnique(string name, int id);
	public Task AddIngredientAsync(Ingredient ingredient);
	public Task UpdateIngredientAsync(Ingredient ingredient);
	public Task DeleteIngredientAsync(Ingredient ingredient);

	public Task<List<RecipeTag>> GetTagListAsync(List<int>? ids = null);
	public Task AddTagAsync(RecipeTag recipeTag);
	public Task UpdateTagAsync(RecipeTag recipeTag);
	public Task DeleteTagAsync(RecipeTag recipeTag);
}
