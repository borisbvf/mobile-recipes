using System.Runtime.Versioning;

namespace Recipes.FolderPickUtil;
public static class FolderPicker
{
	static IFolderPicker? defaultImplementation;
	public static IFolderPicker Default => defaultImplementation ??= new FolderPickerImplementation();

	[SupportedOSPlatform("Android26.0")]
	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]
	public static Task<FolderPickerResult> PickAsync(string initialPath, CancellationToken cancellationToken = default) =>
		Default.PickAsync(initialPath, cancellationToken);

	[SupportedOSPlatform("Android26.0")]
	[SupportedOSPlatform("Windows")]
	[SupportedOSPlatform("iOS14.0")]
	[SupportedOSPlatform("MacCatalyst14.0")]
	public static Task<FolderPickerResult> PickAsync(CancellationToken cancellationToken = default) =>
		Default.PickAsync(cancellationToken);

	internal static void SetDefault(IFolderPicker implementation) =>
		defaultImplementation = implementation;
}
