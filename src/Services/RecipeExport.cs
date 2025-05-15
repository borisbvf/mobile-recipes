using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Net;
using System.Numerics;

namespace Recipes.Services;
public class RecipeExport
{
	public static LocalizationManager LocalizationManager => LocalizationManager.Instance;
	public static MigraDoc.DocumentObjectModel.Color GetSelectionColor()
	{
		return MigraDoc.DocumentObjectModel.Color.Parse(((Microsoft.Maui.Graphics.Color)App.Current!.Resources["MainDark"]).ToArgbHex());
	}
	private static readonly bool useColors = false;
	public static void ExportToPdf(Recipe recipe, string filename)
	{
		void AddSpanBetweenParagraphs(Section targetSection)
		{
			var spanParagraph = targetSection.AddParagraph("", StyleNames.Normal);
			spanParagraph.Format.Font.Size = Unit.FromPoint(6);
		}

		var document = new Document();

		var style = document.Styles[StyleNames.Heading1];
		style!.Font.Size = Unit.FromPoint(20);
		style.Font.Bold = true;
		style.Font.Italic = false;
		style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

		style = document.Styles[StyleNames.Heading2];
		style!.Font.Size = Unit.FromPoint(16);
		style.Font.Bold = false;
		style.Font.Italic = true;
		style.ParagraphFormat.Alignment = ParagraphAlignment.Left;
		style.ParagraphFormat.SpaceBefore = Unit.FromPoint(3);

		style = document.Styles[StyleNames.Normal];
		style!.Font.Size = Unit.FromPoint(16);
		style.Font.Bold = false;
		style.Font.Italic = false;


		var section = document.AddSection();
		section.PageSetup.PageFormat = PageFormat.A4;
		section.PageSetup.Orientation = Orientation.Portrait;
		PageSetup.GetPageSize(PageFormat.A4, out Unit pageWidth, out Unit pageHeight);

		var paragraph = section.AddParagraph(recipe.Name!, StyleNames.Heading1);

		paragraph = section.AddParagraph(recipe.Description!, StyleNames.Heading2);

		AddSpanBetweenParagraphs(section);

		string recipeTime = $"{LocalizationManager["PrepTime"]} " +
			$"{TimeSpan.FromMinutes(Convert.ToDouble(recipe.PreparationTime)).ToString(@"hh\:mm")}" +
			$"{Environment.NewLine}" +
			$"{LocalizationManager["CookTime"]} " +
			$"{TimeSpan.FromMinutes(Convert.ToDouble(recipe.CookingTime)).ToString(@"hh\:mm")}";
		paragraph = section.AddParagraph();
		paragraph.Style = StyleNames.Normal;
		var text = paragraph.AddFormattedText($"{LocalizationManager["PrepTime"]}");
		if (useColors)
			text.Color = GetSelectionColor();
		text = paragraph.AddFormattedText($"    {TimeSpan.FromMinutes(Convert.ToDouble(recipe.PreparationTime)).ToString(@"hh\:mm")}");
		paragraph.AddLineBreak();
		text = paragraph.AddFormattedText($"{LocalizationManager["CookTime"]}");
		if (useColors)
			text.Color = GetSelectionColor();
		text = paragraph.AddFormattedText($"    {TimeSpan.FromMinutes(Convert.ToDouble(recipe.CookingTime)).ToString(@"hh\:mm")}");

		AddSpanBetweenParagraphs(section);

		paragraph = section.AddParagraph();
		paragraph.Style = StyleNames.Normal;
		text = paragraph.AddFormattedText($"{LocalizationManager["IngredientsCaption"]}");
		if (useColors)
			text.Color = GetSelectionColor();

		List<string> recipeIngredients = new();
		foreach (Ingredient item in recipe.Ingredients)
			recipeIngredients.Add($"{item.Name} {item.Comment}");
		paragraph = section.AddParagraph($"{string.Join(Environment.NewLine, recipeIngredients)}");

		AddSpanBetweenParagraphs(section);

		paragraph = section.AddParagraph(recipe.Instructions!, StyleNames.Normal);

		AddSpanBetweenParagraphs(section);

		if (recipe.Tags.Count > 0)
		{
			paragraph = section.AddParagraph();
			paragraph.Style = StyleNames.Normal;
			text = paragraph.AddFormattedText($"{LocalizationManager["TagsCaption"]} ");
			foreach (RecipeTag tag in recipe.Tags)
			{
				paragraph.AddFormattedText($"{tag.Name}    ");
			}
		}

		if (recipe.Images.Count > 0)
		{
			document.ImagePath = Constants.ImageDirectory;
			foreach (RecipeImage recipeImage in recipe.Images)
			{
				XImage xImage = XImage.FromFile(recipeImage.FileName!);
				var docImage = section.AddImage(recipeImage.FileName!);
				docImage.LockAspectRatio = true;
				docImage.Left = ShapePosition.Center;

				Unit maxSize = pageWidth * 2 / 3;
				double maxSide = Math.Max(xImage.PointWidth, xImage.PointHeight);
				if (maxSide > maxSize.Point)
				{
					if (xImage.PixelWidth > xImage.PixelHeight)
						docImage.Width = maxSize;
					else
						docImage.Height = maxSize;
				}

				if (!string.IsNullOrEmpty(recipeImage.Description))
				{
					paragraph = section.AddParagraph(recipeImage.Description, StyleNames.Normal);
					paragraph.Format.Alignment = ParagraphAlignment.Center;
				}
			}
		}


		var pdfRenderer = new PdfDocumentRenderer
		{
			Document = document,
			PdfDocument =
			{
				PageLayout = PdfPageLayout.SinglePage,
				ViewerPreferences =
				{
					FitWindow = true
				}
			}
		};
		pdfRenderer.RenderDocument();

		pdfRenderer.Save(filename);
	}
}
