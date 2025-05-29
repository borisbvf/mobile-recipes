using System.IO;
using System.IO.Compression;

namespace Recipes.Services;
public static class BackupService
{
	public static async Task<WorkResult<string>> BackupDatabase(string folderPath, IRecipeService recipeService)
	{
		try
		{
			string fileName = $"recipes_db{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.zip";
			string filePath = Path.Combine(folderPath, fileName);
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				System.IO.Compression.ZipFile.CreateFromDirectory(Constants.ImageDirectory, fileStream, CompressionLevel.Fastest, true);
				using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Update))
				{
					await recipeService.DisconnectDB();
					ZipArchiveEntry zipEntry = zipArchive.CreateEntry(Constants.DBFileName);
					using (Stream entry = zipEntry.Open())
					{
						using (FileStream dbFileStream = new FileStream(Constants.DBPath, FileMode.Open))
						{
							dbFileStream.CopyTo(entry);
						}
					}
					recipeService.ReconnectDB();
				}
			}
			return new WorkResult<string>(filePath, null);
		}
		catch (Exception e)
		{
			return new WorkResult<string>(null, e);
		}
	}

	public static async Task<WorkResult> RestoreDatabase(string backupPath, IRecipeService recipeService)
	{
		try
		{
			if (!File.Exists(backupPath))
			{
				throw new FileNotFoundException();
			}
			if (Directory.Exists(Constants.ImageDirectory))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Constants.ImageDirectory);
				directoryInfo.Delete(true);
			}
			await recipeService.DisconnectDB();
			if (File.Exists(Constants.DBPath))
			{
				File.Delete(Constants.DBPath);
			}
			ZipFile.ExtractToDirectory(backupPath, FileSystem.AppDataDirectory);
			recipeService.ReconnectDB();
			return new WorkResult(null);
		}
		catch (Exception e)
		{
			return new WorkResult(e);
		}
	}
}
