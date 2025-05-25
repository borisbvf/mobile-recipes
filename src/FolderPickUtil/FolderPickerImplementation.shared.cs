namespace Recipes.FolderPickUtil;
public sealed partial class FolderPickerImplementation : IFolderPicker
{
	public async Task<FolderPickerResult> PickAsync(string initialPath, CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			var folder = await InternalPickAsync(initialPath, cancellationToken);
			return new FolderPickerResult(folder, null);
		}
		catch (Exception e)
		{
			return new FolderPickerResult(null, e);
		}
	}

	public async Task<FolderPickerResult> PickAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			var folder = await InternalPickAsync(cancellationToken);
			return new FolderPickerResult(folder, null);
		}
		catch (Exception e)
		{
			return new FolderPickerResult(null, e);
		}
	}
}
