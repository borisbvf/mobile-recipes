using System.Runtime.Versioning;

namespace Recipes.FolderPickUtil;

[SupportedOSPlatform("iOS14.0")]
[SupportedOSPlatform("MacCatalyst14.0")]
public sealed partial class FolderPickerImplementation : IFolderPicker
{
	static async Task<string> InternalPickAsync(string initialPath, CancellationToken cancellationToken)
	{
		using (FolderPickMacIosImplementation maciosImplementation = new FolderPickMacIosImplementation())
		{
			return await maciosImplementation.InternalPickAsync(initialPath, cancellationToken);
		}
	}

	static async Task<string> InternalPickAsync(CancellationToken cancellationToken)
	{
		using (FolderPickMacIosImplementation maciosImplementation = new FolderPickMacIosImplementation())
		{
			return await maciosImplementation.InternalPickAsync(cancellationToken);
		}
	}
}
