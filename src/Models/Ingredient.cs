using SQLite;

namespace Recipes.Models;
public class Ingredient
{
	[PrimaryKey]
	public int Id { get; set; }
	public string? Name { get; set; }
}
