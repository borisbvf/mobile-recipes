using System.Runtime.Versioning;

namespace Recipes.FolderPickUtil;
public interface IFolderPicker
{
	[SupportedOSPlatform("Android26.0")]
	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]
	Task<FolderPickerResult> PickAsync(string initialPath, CancellationToken cancellationToken = default);

	[SupportedOSPlatform("Android26.0")]
	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]
	Task<FolderPickerResult> PickAsync(CancellationToken cancellationToken = default);
}
