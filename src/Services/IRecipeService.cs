namespace Recipes.Services;
public interface IRecipeService
{
	public Task<RequestResult<IEnumerable<Recipe>>> GetRecipesAsync();
	public Task<RequestResult> AddRecipeAsync(Recipe recipe);
	public Task<RequestResult> UpdateRecipeAsync(Recipe recipe);
	public Task<RequestResult> DeleteRecipeAsync(int recipeId);
}
