namespace Recipes.ViewModels;
public class BackupManagementViewModel : BaseViewModel
{
	public LocalizationManager LocalizationManager => LocalizationManager.Instance;

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


}
