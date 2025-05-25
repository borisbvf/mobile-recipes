using Foundation;
using UIKit;
using UniformTypeIdentifiers;

namespace Recipes.FolderPickUtil;

public sealed partial class FolderPickMacIosImplementation : IDisposable
{
	readonly UIDocumentPickerViewController documentPickerViewController = new([UTTypes.Folder])
	{
		AllowsMultipleSelection = false
	};

	TaskCompletionSource<string>? taskCompletedSource;

	public FolderPickMacIosImplementation()
	{
		documentPickerViewController.DidPickDocumentAtUrls += DocumentPickerViewControllerOnDidPickDocumentAtUris;
		documentPickerViewController.WasCancelled += DocumentPickerViewControllerOnWasCancelled;
	}

	public void Dispose()
	{
		documentPickerViewController.DidPickDocumentAtUrls -= DocumentPickerViewControllerOnDidPickDocumentAtUris;
		documentPickerViewController.WasCancelled -= DocumentPickerViewControllerOnWasCancelled;
		documentPickerViewController.Dispose();
	}

	public async Task<string> InternalPickAsync(string initialPath, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		documentPickerViewController.DirectoryUrl = NSUrl.FromString(initialPath);
		var currentViewController = Platform.GetCurrentUIViewController();

		taskCompletedSource?.TrySetCanceled(CancellationToken.None);
		var tcs = taskCompletedSource = new();
		if (currentViewController is not null)
		{
			currentViewController.PresentViewController(documentPickerViewController, true, null);
		}
		else
		{
			throw new FolderPickerException("Unable to get a window where to present the folder picker UI.");
		}

		return await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task<string> InternalPickAsync(CancellationToken cancellationToken)
	{
		return await InternalPickAsync("/", cancellationToken);
	}

	void DocumentPickerViewControllerOnWasCancelled(object? sender, EventArgs e)
	{
		taskCompletedSource?.TrySetException(new FolderPickerException("Operation cancelled."));
	}

	void DocumentPickerViewControllerOnDidPickDocumentAtUris(object? sender, UIDocumentPickedAtUrlsEventArgs e)
	{
		var path = e.Urls[0].Path ?? throw new FolderPickerException("Path cannot be null.");
		taskCompletedSource?.TrySetResult(path);
	}
}
