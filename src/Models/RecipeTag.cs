using SQLite;

namespace Recipes.Models;
public class RecipeTag
{
	[PrimaryKey]
	public int Id { get; set; }
	public string? Name { get; set; }
	public Color? Color { get; set; }
}
