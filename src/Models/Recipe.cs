using SQLite;
using System.Collections.ObjectModel;

namespace Recipes.Models;
public class Recipe
{
	[PrimaryKey]
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Instructions { get; set; }
	public int? PreparationTime { get; set; }
	public int? CookingTime { get; set; }
	public ObservableCollection<RecipeTag> Tags { get; }
	public ObservableCollection<Ingredient> Ingredients { get; }
	public ObservableCollection<Image> Images { get; }
	public Recipe()
	{
		Tags = new();
		Ingredients = new();
		Images = new();
	}
}
