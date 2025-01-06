using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text.Json;
using WPFUiDesktopApp.Settings;

namespace WPFUiDesktopApp.Services;

/// <summary>
/// Service for managing application settings.
/// </summary>
public class SettingsService
{
    private readonly IFileSystem _fileSystem;
    private readonly string _settingsFilePath;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="optionsMonitor">The options monitor for AppSettings.</param>
    public SettingsService(IFileSystem fileSystem, IOptionsMonitor<AppSettings> optionsMonitor)
    {
        Guard.IsNotNull(fileSystem);
        Guard.IsNotNull(App.AppSettingsFileName);

        _fileSystem = fileSystem;
        _settingsFilePath = App.AppSettingsFileName;

        // Subscribe to changes
        optionsMonitor.OnChange(OnAppSettingsChanged);
    }

    private void OnAppSettingsChanged(AppSettings newSettings)
    {
        // Fire and forget the async method
        _ = OnAppSettingsChangedAsync(newSettings);
    }

    private async Task OnAppSettingsChangedAsync(AppSettings newSettings)
    {
        if (string.IsNullOrEmpty(_settingsFilePath))
        {
            return;
        }

        var jsonObject = new
        {
            AppSettings = newSettings
        };

        var json = JsonSerializer.Serialize(jsonObject, JsonSerializerOptions);
        await _fileSystem.File.WriteAllTextAsync(_settingsFilePath, json);
    }

    /// <summary>
    /// Saves the application settings asynchronously.
    /// </summary>
    /// <param name="appSettings">The application settings to save.</param>
    public async Task SaveSettingsAsync(AppSettings appSettings)
    {
        if (string.IsNullOrEmpty(_settingsFilePath))
        {
            return;
        }

        var jsonObject = new
        {
            AppSettings = appSettings
        };

        var json = JsonSerializer.Serialize(jsonObject, JsonSerializerOptions);
        await _fileSystem.File.WriteAllTextAsync(_settingsFilePath, json);
    }
}
