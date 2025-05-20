using PdfSharp.Fonts;

namespace Recipes.Utils;

public class FileFontResolver : IFontResolver
{
	public byte[]? GetFont(string faceName)
	{
		using Stream filestream = Task.Run(async () => await FileSystem.Current.OpenAppPackageFileAsync(faceName)).Result;
		using MemoryStream ms = new ();
		filestream.CopyTo(ms);
		return ms.ToArray();
	}

	public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
	{
		string faceName = "OpenSans-Regular.ttf";
		if (bold & !italic)
		{
			faceName = "OpenSans-Bold.ttf";
		} else if (!bold & italic)
		{
			faceName = "OpenSans-Italic.ttf";
		} else if (bold & italic)
		{
			faceName = "OpenSans-BoldItalic.ttf";
		}
			return new FontResolverInfo(faceName);
	}
}
