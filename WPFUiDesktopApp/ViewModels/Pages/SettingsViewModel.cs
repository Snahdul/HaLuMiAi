using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Services;
using WPFUiDesktopApp.Settings;

namespace WPFUiDesktopApp.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private readonly SettingsService _settingsService;

        private bool _isInitialized = false;

        [ObservableProperty]
        private AppSettings _appSettings;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        public SettingsViewModel(IOptions<AppSettings> options, SettingsService settingsService)
        {
            Guard.IsNotNull(options);
            _appSettings = options.Value;
            _settingsService = settingsService;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            AppVersion = $"WPFUiDesktopApp - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? string.Empty;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            await _settingsService.SaveSettingsAsync(AppSettings);
        }

        [RelayCommand]
        private async Task OnChangeThemeAsync(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    AppSettings.CurrentTheme = ApplicationTheme.Light;
                    break;

                default:
                    if (AppSettings.CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    AppSettings.CurrentTheme = ApplicationTheme.Dark;
                    break;
            }

            await SaveSettingsAsync();
        }
    }
}
