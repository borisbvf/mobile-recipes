using MauiCommonTools.Alerts;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModels;

public class CleaningUnusedFilesViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;
	
	private IRecipeService _recipeService;

	public CleaningUnusedFilesViewModel(IRecipeService recipeService)
	{
		_recipeService = recipeService;
	}

	private int imageCount;
	public int ImageCount
	{
		get => imageCount;
		set
		{
			if (value != imageCount)
			{
				imageCount = value;
				OnPropertyChanged();
			}
		}
	}

	private long imageSize;
	public long ImageSize
	{
		get => imageSize;
		set
		{
			if (value != imageSize)
			{
				imageSize = value;
				OnPropertyChanged();
			}
		}
	}

	private int pdfCount;
	public int PdfCount
	{
		get => pdfCount;
		set
		{
			if (value != pdfCount)
			{
				pdfCount = value;
				OnPropertyChanged();
			}
		}
	}

	private long pdfSize;
	public long PdfSize
	{
		get => pdfSize;
		set
		{
			if (pdfSize != value)
			{
				pdfSize = value;
				OnPropertyChanged();
			}
		}
	}

	private readonly List<string> pdfFilePathList = [];
	private readonly List<string> imageFilePathList = [];

	public ICommand ReloadInfoCommand => new Command(ReloadInfo, () => !IsBusy);
	private async void ReloadInfo()
	{
		IsBusy = true;
		try
		{
			if (!Directory.Exists(Constants.PdfDirectory))
				Directory.CreateDirectory(Constants.PdfDirectory);
			DirectoryInfo pdfDirInfo = new DirectoryInfo(Constants.PdfDirectory);
			long pdfFileSize = 0;
			pdfFilePathList.Clear();
			foreach (FileInfo file in pdfDirInfo.GetFiles())
			{
				pdfFilePathList.Add(file.FullName);
				pdfFileSize += file.Length;
			}
			PdfCount = pdfFilePathList.Count;
			PdfSize = pdfFileSize;

			if (!Directory.Exists(Constants.ImageDirectory))
				Directory.CreateDirectory(Constants.ImageDirectory);
			DirectoryInfo imageDirInfo = new DirectoryInfo(Constants.ImageDirectory);
			List<string> imageDbData = await _recipeService.GetImageFilenamesAsync();
			long imageFileSize = 0;
			imageFilePathList.Clear();
			foreach (FileInfo file in imageDirInfo.GetFiles())
			{
				if (imageDbData.IndexOf(file.FullName) < 0)
				{
					imageFilePathList.Add(file.FullName);
					imageFileSize += file.Length;
				}
			}
			ImageCount = imageFilePathList.Count;
			ImageSize = imageFileSize;
		}
		catch (Exception e)
		{
			await Shell.Current.DisplayAlert(
				$"{LocalizationManager["Error"]}",
				$"{e.Message}",
				$"{LocalizationManager["Ok"]}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public ICommand CleanFilesCommand => new Command(CleanFiles, () => !IsBusy && (ImageCount > 0 || PdfCount > 0));
	private async void CleanFiles()
	{
		IsBusy = true;
		try
		{
			foreach (string file in pdfFilePathList)
			{
				File.Delete(file);
			}
			foreach (string file in imageFilePathList)
			{
				File.Delete(file);
			}
			IToast toast = Toast.Make($"{LocalizationManager["CleaningSuccessMessage"]}");
			await toast.Show();
		}
		catch (Exception e)
		{
			await Shell.Current.DisplayAlert(
				$"{LocalizationManager["Error"]}",
				$"{e.Message}",
				$"{LocalizationManager["Ok"]}");
		}
		finally
		{
			IsBusy = false;
		}
		ReloadInfo();
	}
}
