using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Recipes.Models;
public class RecipeTag : ICloneable
{
	[PrimaryKey]
	public int Id { get; set; }
	public bool IsChecked { get; set; }
	public string? Name { get; set; }
	public string? Color { get; set; }
	public int SortOrder { get; set; }

	public virtual object Clone()
	{
		return new RecipeTag()
		{
			Id = this.Id,
			Name = this.Name,
			Color = this.Color,
			IsChecked = this.IsChecked,
			SortOrder = this.SortOrder
		};
	}

	public virtual void CopyFrom(RecipeTag source)
	{
		Id = source.Id;
		Name = source.Name;
		Color = source.Color;
		IsChecked = source.IsChecked;
		SortOrder = source.SortOrder;
	}
}
