namespace Recipes.FolderPickUtil;
public class FolderPickerResult
{
	public string? FolderPath { get; set; }
	public Exception? Exception { get; set; }
	public bool IsSuccess => FolderPath != null && Exception is null;
	public FolderPickerResult(string? folderPath, Exception? exception)
	{
		FolderPath = folderPath;
		Exception = exception;
	}
}
