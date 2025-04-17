using BaseMobile.Resources.Localization;
using System.ComponentModel;
using System.Globalization;

namespace BaseMobile.Services;
public class LocalizationManager : INotifyPropertyChanged
{
	private LocalizationManager()
	{
		AppResources.Culture = CultureInfo.CurrentCulture;
	}

	public static LocalizationManager Instance = new LocalizationManager();

	public object this[string resourceKey] => AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetCulture(CultureInfo culture)
	{
		AppResources.Culture = culture;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
	}

	public void SetLanguage(string language)
	{
		CultureInfo culture;
		if (language == Constants.RussianLang)
		{
			culture = new CultureInfo(Constants.RussianAbrv);
		} else if (language == Constants.EnglishLang)
		{
			culture = new CultureInfo(Constants.EnglishAbrv);
		} else
		{
			culture = new CultureInfo(Constants.EnglishAbrv);
		}
		SetCulture(culture);
	}
}
