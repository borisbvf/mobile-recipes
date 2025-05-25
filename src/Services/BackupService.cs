using System.IO;
using System.IO.Compression;

namespace Recipes.Services;
public static class BackupService
{
	public static WorkResult<string> BackupDatabase(string folderPath)
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
					ZipArchiveEntry zipEntry = zipArchive.CreateEntry(Constants.DBFileName);
					using (Stream entry = zipEntry.Open())
					{
						using (FileStream dbFileStream = new FileStream(Constants.DBPath, FileMode.Open))
						{
							dbFileStream.CopyTo(entry);
						}
					}
				}
			}
			return new WorkResult<string>(filePath, null);
		}
		catch (Exception e)
		{
			return new WorkResult<string>(null, e);
		}
	}

	public static WorkResult RestoreDatabase(string backupPath)
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
			if (File.Exists(Constants.DBPath))
			{
				File.Delete(Constants.DBPath);
			}
			/*if (File.Exists(Constants.DBPath))
			{
				throw new Exception("File exists after deletion.");
			}*/
			ZipFile.ExtractToDirectory(backupPath, FileSystem.AppDataDirectory);
			return new WorkResult(null);
		}
		catch (Exception e)
		{
			return new WorkResult(e);
		}
	}
}
