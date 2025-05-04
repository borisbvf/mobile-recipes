using System.Collections.ObjectModel;

namespace Recipes.Views;

public partial class ImageStoreView : ContentPage
{
	public ImageStoreView()
	{
		InitializeComponent();
		BindingContext = this;
		GetAllFiles();
	}

	public ObservableCollection<string> FileNames { get; } = [];

	private void GetAllFiles()
	{
		string[] files = Directory.GetFiles(Constants.ImageDirectory);
		foreach (string file in files)
		{
			FileNames.Add(file);
		}
	}
}