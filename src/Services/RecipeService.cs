using System.Diagnostics;
using System.Net.Http.Headers;
namespace Recipes.Services;

public class RecipeService : IRecipeService
{
	public async Task<RequestResult<IEnumerable<Recipe>>> GetRecipesAsync()
	{
		RequestResult<IEnumerable<Recipe>> result = new();
		try
		{
			//
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["ErrorGettingRecipesFailed"]} [{ex.Message}]";
		}
		return result;
	}

	public async Task<RequestResult> AddRecipeAsync(Recipe recipe)
	{
		RequestResult result = new();
		try
		{
			//
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["ErrorAddingRecipe"]} [{ex.Message}]";
		}
		return result;
	}

	public async Task<RequestResult> UpdateRecipeAsync(Recipe recipe)
	{
		RequestResult result = new();
		try
		{
			//
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["ErrorUpdatingRecipe"]} [{ex.Message}]";
		}
		return result;
	}

	public async Task<RequestResult> DeleteRecipeAsync(int recipeId)
	{
		RequestResult result = new();
		try
		{
			//
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			result.IsSuccess = false;
			result.ErrorMessage = $"{LocalizationManager.Instance["ErrorDeletingRecipe"]} [{ex.Message}]";
		}
		return result;
	}
}
