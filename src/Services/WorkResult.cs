namespace Recipes.Services;

public class WorkResult
{
	public Exception? Exception { get; set; }
	public bool IsSuccess => Exception is null;
	public WorkResult(Exception? exception)
	{
		Exception = exception;
	}
}
public class WorkResult<T>
{
	public Exception? Exception { get; set; }
	public T? Data { get; set; }
	public bool IsSuccess => Data != null && Exception is null;
	public WorkResult(T? data, Exception? exception)
	{
		Data = data;
		Exception = exception;
	}
}
