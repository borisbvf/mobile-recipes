using Recipes.FolderPickUtil;
using Recipes.ToastUtil;
using System.Globalization;
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

	public string? BackupLastTime
	{
		get => Preferences.Default.Get(Constants.LastBackupDatetimeKey, $"{LocalizationManager["BackupNoInformation"]}");
		set
		{
			if (value != null && value != Preferences.Default.Get(Constants.LastBackupDatetimeKey, string.Empty))
			{
				Preferences.Default.Set(Constants.LastBackupDatetimeKey, value);
				OnPropertyChanged();
			}
		}
	}

	public string? BackupFolderName
	{
		get => Preferences.Default.Get(Constants.LastBackupFolderKey, $"{LocalizationManager["BackupNoInformation"]}");
		set
		{
			if (value != null && value != Preferences.Default.Get(Constants.LastBackupFolderKey, string.Empty))
			{
				Preferences.Default.Set(Constants.LastBackupFolderKey, value);
				OnPropertyChanged();
			}
		}
	}

	public string? BackupFileName
	{
		get => Preferences.Default.Get(Constants.LastBackupFilenameKey, $"{LocalizationManager["BackupNoInformation"]}");
		set
		{
			if (value != null && value != Preferences.Default.Get(Constants.LastBackupFilenameKey, string.Empty))
			{
				Preferences.Default.Set(Constants.LastBackupFilenameKey, value);
				OnPropertyChanged();
			}
		}
	}

	public ICommand ReloadInfoCommand => new Command(ReloadInfo, () => !IsBusy);
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

		IsBusy = true;
		try
		{
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
		}
		finally
		{
			IsBusy = false;
		}
	}

	internal async Task<WorkResult<string>> ProcessBackupSaving(string path)
	{
		IsBusy = true;
		try
		{
			WorkResult<string> backupResult = BackupService.BackupDatabase(path);
			if (backupResult.IsSuccess)
			{
				BackupLastTime = DateTime.Now.ToString("yyyy.MM.dd hh:mm");
				BackupFolderName = path;
				BackupFileName = Path.GetFileName(backupResult.Data);
			}
			else
			{
				await Shell.Current.DisplayAlert(
					$"{LocalizationManager["Error"]}",
					$"{backupResult.Exception?.Message}",
					$"{LocalizationManager["Ok"]}");
			}
			return backupResult;
		}
		finally
		{
			IsBusy = false;
		}
	}

	internal async Task<WorkResult> ProcessBackupRestoring(string filePath)
	{
		IsBusy = true;
		try
		{
			WorkResult backupResult = BackupService.RestoreDatabase(filePath);
			if (!backupResult.IsSuccess)
			{
				await Shell.Current.DisplayAlert(
					$"{LocalizationManager["Error"]}",
					$"{backupResult.Exception?.Message}",
					$"{LocalizationManager["Ok"]}");
			}
			return backupResult;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public ICommand SaveBackupCommand => new Command(SaveBackup, () => !IsBusy);
	private async void SaveBackup()
	{
		FolderPickerResult folderPickResult = await FolderPicker.Default.PickAsync();
		if (folderPickResult.IsSuccess)
		{
			WorkResult<string> backupResult = await ProcessBackupSaving(folderPickResult.FolderPath!);
			if (backupResult.IsSuccess)
			{
				IToast toast = Toast.Make($"{LocalizationManager["BackupSavingSuccess"]}");
				await toast.Show();
			}
		}
		else
		{
			if (folderPickResult.Exception is not TaskCanceledException)
			{
				await Shell.Current.DisplayAlert(
					$"{LocalizationManager["Error"]}",
					$"{folderPickResult.Exception?.Message}",
					$"{LocalizationManager["Ok"]}");
			}
		}
	}

	public ICommand RestoreBackupCommand => new Command(RestoreBackup, () => !IsBusy);
	private async void RestoreBackup()
	{
		bool confirmation = await Shell.Current.DisplayAlert(
			"",
			$"{LocalizationManager["BackupRestoreWarning"]}",
			$"{LocalizationManager["Continue"]}",
			$"{LocalizationManager["Cancel"]}");
		if (!confirmation)
			return;

		FileResult? fileResult = await FilePicker.Default.PickAsync();
		if (fileResult != null)
		{
			WorkResult backupResult = await ProcessBackupRestoring(fileResult.FullPath);
			if (backupResult.IsSuccess)
			{
				IToast toast = Toast.Make($"{LocalizationManager["BackupRestoringSuccess"]}");
				await toast.Show();
				_recipeService.ReconnectDB();
				ReloadInfo();
			}
		}
	}
}
