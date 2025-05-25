using Android.Content;
using Android.Provider;
using System.Runtime.Versioning;
using System.Web;
using Microsoft.Maui.ApplicationModel;

namespace Recipes.FolderPickUtil;
[SupportedOSPlatform("Android26.0")]
public sealed partial class FolderPickerImplementation : IFolderPicker
{
	public const string ExternalStorageBaseUrl = "content://com.android.externalstorage.documents/document/primary%3A";
	public const string PrimaryStorage = "primary";
	public const string Storage = "storage";
	public const int RequestCodeFolderPicker = 2000;

	static async Task<string> InternalPickAsync(string initialPath, CancellationToken cancellationToken)
	{
		string? folder = null;
		if (!OperatingSystem.IsAndroidVersionAtLeast(26) && !string.IsNullOrEmpty(initialPath))
		{
			var statusRead = await Permissions.RequestAsync<Permissions.StorageRead>().WaitAsync(cancellationToken).ConfigureAwait(false);
			if (statusRead is not PermissionStatus.Granted)
			{
				throw new PermissionException("Storage permission is not granted.");
			}
		}

		if (Android.OS.Environment.ExternalStorageDirectory is not null)
		{
			initialPath = initialPath.Replace(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, string.Empty, StringComparison.InvariantCulture);
		}

		var initialFolderUri = Android.Net.Uri.Parse(ExternalStorageBaseUrl + HttpUtility.UrlEncode(initialPath));

		var intent = new Intent(Intent.ActionOpenDocumentTree);
		intent.PutExtra(DocumentsContract.ExtraInitialUri, initialFolderUri);

		await FolderPickIntermediateActivity.StartAsync(intent, RequestCodeFolderPicker, onResult: OnResult).WaitAsync(cancellationToken);

		return folder ?? throw new FolderPickerException("Unable to get folder.");

		void OnResult(Intent resultIntent)
		{
			folder = EnsurePhysicalPath(resultIntent.Data);
		}
	}

	public static async Task<string> InternalPickAsync(CancellationToken cancellationToken)
	{
		return await InternalPickAsync(Android.OS.Environment.ExternalStorageDirectory?.Path ?? "/storage/emulated/0", cancellationToken);
	}

	private static string EnsurePhysicalPath(Android.Net.Uri? uri)
	{
		if (uri is null)
		{
			throw new FolderPickerException("Path is not selected.");
		}

		return UriToPhysicalPath(uri) ?? throw new FolderPickerException($"Unable to resolve absolute path or retrieve contents of URI '{uri}'.");
	}

	private static string? UriToPhysicalPath(Android.Net.Uri uri)
	{
		const string uriSchemeFolder = "content";
		if (uri.Scheme is null || !uri.Scheme.Equals(uriSchemeFolder, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		if (uri.PathSegments?.Count < 2)
		{
			return null;
		}

		// Example path would be /tree/primary:DCIM, or /tree/SDCare:DCIM
		var path = uri.PathSegments?[1];

		if (path is null)
		{
			return null;
		}

		var pathSplit = path.Split(':');
		if (pathSplit.Length < 2)
		{
			return null;
		}

		// Primary is the device's internal storage, and anything else is an SD card or other external storage
		if (pathSplit[0].Equals(PrimaryStorage, StringComparison.OrdinalIgnoreCase))
		{
			// Example for internal path /storage/emulated/0/DCIM
			return $"{Android.OS.Environment.ExternalStorageDirectory?.Path}/{pathSplit[1]}";
		}

		// Example for external path /storage/1B0B-0B1C/DCIM
		return $"/{Storage}/{pathSplit[0]}/{pathSplit[1]}";
	}
}
