using SQLite;

namespace Recipes.Models;
public class Recipe
{
	[PrimaryKey]
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Content { get; set; }
	public int? PreparationTime { get; set; }
	public int? CookingTime { get; set; }
}
