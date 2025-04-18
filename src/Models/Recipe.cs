﻿namespace Recipes.Models;
public class Recipe
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Content { get; set; }
	public int? Duration { get; set; }
}
