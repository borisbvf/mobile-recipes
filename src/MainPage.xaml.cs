using System.Windows.Input;
using Recipes.Services;

namespace Recipes
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

		public LocalizationManager LocalizationManager => LocalizationManager.Instance;

		public ICommand SettingsCommand => new Command(GoToSettings);
        private async void GoToSettings()
        {
            await Shell.Current.GoToAsync($"{Constants.SettingsPageRoute}");
        }
    }

}
