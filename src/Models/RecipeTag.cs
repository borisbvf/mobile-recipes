using SQLite;

namespace Recipes.Models;
public class RecipeTag : ICloneable
{
	[PrimaryKey]
	public int Id { get; set; }
	public bool IsChecked { get; set; }
	public string? Name { get; set; }
	public string? Color { get; set; }
	public object Clone()
	{
		return new RecipeTag()
		{
			Id = this.Id,
			Name = this.Name,
			Color = this.Color,
			IsChecked = this.IsChecked
		};
	}
}
