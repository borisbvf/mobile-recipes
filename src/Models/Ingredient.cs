using SQLite;

namespace Recipes.Models;
public class Ingredient
{
	[PrimaryKey]
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Comment { get; set; }
	public int SortOrder { get; set; }

	public Ingredient()
	{

	}
	public Ingredient(Ingredient source)
	{
		CopyFrom(source);
	}

	public virtual void CopyFrom(Ingredient source)
	{
		Id = source.Id;
		Name = source.Name;
		Comment = source.Comment;
		SortOrder = source.SortOrder;
	}
}
