using CommunityToolkit.Diagnostics;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Services;
using WPFUiDesktopApp.Settings;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// ViewModel for the Settings page.
/// </summary>
public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly SettingsService _settingsService;

    private bool _isInitialized = false;

    [ObservableProperty]
    private AppSettings _appSettings;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="settingsService">The settings service.</param>
    public SettingsViewModel(SettingsService settingsService)
    {
        Guard.IsNotNull(settingsService);
        _settingsService = settingsService;
        _appSettings = _settingsService.GetCurrentSettings();
    }

    /// <summary>
    /// Called when the view is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    /// <summary>
    /// Called when the view is navigated from.
    /// </summary>
    public void OnNavigatedFrom() { }

    /// <summary>
    /// Initializes the ViewModel.
    /// </summary>
    private void InitializeViewModel()
    {
        AppVersion = $"WPFUiDesktopApp - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    /// <summary>
    /// Gets the assembly version.
    /// </summary>
    /// <returns>The assembly version.</returns>
    private string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
               ?? string.Empty;
    }

    /// <summary>
    /// Saves the application settings asynchronously.
    /// </summary>
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _settingsService.SaveSettingsAsync(AppSettings);
    }

    /// <summary>
    /// Changes the application theme asynchronously.
    /// </summary>
    /// <param name="parameter">The theme parameter.</param>
    [RelayCommand]
    private async Task OnChangeThemeAsync(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (AppSettings.CurrentTheme == ApplicationTheme.Light)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                AppSettings.CurrentTheme = ApplicationTheme.Light;
                await SaveSettingsAsync();
                break;

            default:
                if (AppSettings.CurrentTheme == ApplicationTheme.Dark)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                AppSettings.CurrentTheme = ApplicationTheme.Dark;
                await SaveSettingsAsync();
                break;
        }
    }
}
