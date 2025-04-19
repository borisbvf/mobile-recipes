namespace Recipes.Services;
public class RequestResult
{
	public bool IsSuccess { get; set; }
	public string? ErrorMessage { get; set; }
}
public class RequestResult<T> : RequestResult
{
	public T? Data { get; set; }
}
