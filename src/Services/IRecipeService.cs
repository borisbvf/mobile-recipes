namespace Recipes.Services;
public interface IRecipeService
{
	public bool CheckIfDBExists();
	public Task CreateTables();

	public Task<IEnumerable<Recipe>> GetRecipeListAsync();
	public Task<Recipe?> GetRecipeAsync(int recipeId);
	public Task AddRecipeAsync(Recipe recipe);
	public Task UpdateRecipeAsync(Recipe recipe);
	public Task DeleteRecipeAsync(Recipe recipe);

	public Task<IEnumerable<Ingredient>> GetIngredientListAsync();
	public Task AddIngredientAsync(Ingredient ingredient);
	public Task UpdateIngredientAsync(Ingredient ingredient);
	public Task DeleteIngredientAsync(Ingredient ingredient);

	public Task<IEnumerable<RecipeTag>> GetTagListAsync();
	public Task<IEnumerable<RecipeTag>> GetTagListAsync(List<int> ids);
	public Task AddTagAsync(RecipeTag recipeTag);
	public Task UpdateTagAsync(RecipeTag recipeTag);
	public Task DeleteTagAsync(RecipeTag recipeTag);
}
