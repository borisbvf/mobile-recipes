using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recipes.ViewModels;
public class BackupManagementViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

	private IRecipeService _recipeService;

	public BackupManagementViewModel(IRecipeService recipeService)
	{
		_recipeService = recipeService;
	}

	private string? databaseSize;
	public string? DatabaseSize
	{
		get => databaseSize;
		set
		{
			if (value != databaseSize)
			{
				databaseSize = value;
				OnPropertyChanged();
			}
		}
	}

	private string? imagesSize;
	public string? ImagesSize
	{
		get => imagesSize;
		set
		{
			if (value != imagesSize)
			{
				imagesSize = value;
				OnPropertyChanged();
			}
		}
	}

	private string? recipesCount;
	public string? RecipesCount
	{
		get => recipesCount;
		set
		{
			if (value != recipesCount)
			{
				recipesCount = value;
				OnPropertyChanged();
			}
		}
	}

	private string? backupLastTime;
	public string? BackupLastTime
	{
		get => backupLastTime;
		set
		{
			if (value != backupLastTime)
			{
				backupLastTime = value;
				OnPropertyChanged();
			}
		}
	}

	public ICommand ReloadInfoCommand => new Command(ReloadInfo);
	private async void ReloadInfo()
	{
		string SizeToStr(long byteSize)
		{
			int sizeKB = (int)Math.Round(((double)byteSize) / 1024);
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberGroupSeparator = " ";
			nfi.NumberDecimalSeparator = ".";
			return $"{sizeKB.ToString(nfi)} {LocalizationManager["KiloByte"]}";
		}

		if (!Directory.Exists(Constants.ImageDirectory))
			Directory.CreateDirectory(Constants.ImageDirectory);
		DirectoryInfo dirInfo = new DirectoryInfo(Constants.ImageDirectory);
		FileInfo[] files = dirInfo.GetFiles();
		long size = 0;
		foreach (FileInfo file in files)
		{
			size += file.Length;
		}
		ImagesSize = SizeToStr(size);

		FileInfo dbFile = new FileInfo(Constants.DBPath);
		DatabaseSize = SizeToStr(dbFile.Length);

		int count = await _recipeService.GetRecipeCount();
		RecipesCount = $"{count}";

		DateTime datetime = Preferences.Default.Get(Constants.LastBackupDatetimeKey, DateTime.MinValue);
		BackupLastTime = datetime == DateTime.MinValue ? $"{LocalizationManager["DateTimeNever"]}" : $"{datetime}";
	}

	public ICommand SaveBackupCommand => new Command(SaveBackup);
	private void SaveBackup()
	{

	}

	public ICommand RestoreBackupCommand => new Command(RestoreBackup);
	private void RestoreBackup()
	{

	}
}
