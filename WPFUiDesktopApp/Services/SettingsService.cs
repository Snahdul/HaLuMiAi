using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text.Json;
using WPFUiDesktopApp.Settings;

namespace WPFUiDesktopApp.Services;

/// <summary>
/// Service for managing application settings.
/// </summary>
public class SettingsService : IDisposable
{
    private readonly IFileSystem _fileSystem;
    private readonly string _settingsFilePath;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    private AppSettings _appSettings;
    private Timer _debounceTimer;
    private readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(500);
    private readonly object _debounceLock = new object();
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="optionsMonitor">The options monitor for AppSettings.</param>
    public SettingsService(IFileSystem fileSystem, IOptionsMonitor<AppSettings> optionsMonitor)
    {
        Guard.IsNotNull(fileSystem, nameof(fileSystem));
        Guard.IsNotNull(optionsMonitor, nameof(optionsMonitor));
        Guard.IsNotNullOrEmpty(App.AppSettingsFileName, nameof(App.AppSettingsFileName));

        _fileSystem = fileSystem;
        _settingsFilePath = App.AppSettingsFileName;
        _appSettings = optionsMonitor.CurrentValue;

        // Subscribe to changes with debouncing
        optionsMonitor.OnChange(OnAppSettingsChanged);
    }

    /// <summary>
    /// Saves the application settings asynchronously.
    /// </summary>
    /// <param name="appSettings">The application settings to save.</param>
    public async Task SaveSettingsAsync(AppSettings appSettings)
    {
        if (string.IsNullOrEmpty(_settingsFilePath))
        {
            Console.WriteLine("Settings file path is null or empty.");
            return;
        }

        var jsonObject = new
        {
            AppSettings = appSettings
        };

        var json = JsonSerializer.Serialize(jsonObject, JsonSerializerOptions);
        var tempFilePath = $"{_settingsFilePath}.tmp";

        try
        {
            // Write to a temporary file first
            await _fileSystem.File.WriteAllTextAsync(tempFilePath, json);

            // Replace the original file atomically
            _fileSystem.File.Copy(tempFilePath, _settingsFilePath, overwrite: true);
            _fileSystem.File.Delete(tempFilePath);

            Console.WriteLine("Settings saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            // Handle exceptions (e.g., log them)
        }
    }

    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    /// <returns>The current AppSettings instance.</returns>
    public AppSettings GetCurrentSettings()
    {
        return _appSettings;
    }

    private void OnAppSettingsChanged(AppSettings newSettings)
    {
        lock (_debounceLock)
        {
            // Dispose the existing timer
            _debounceTimer?.Dispose();

            // Create a new timer
            _debounceTimer = new Timer(DebouncedHandler, newSettings, _debounceTime, Timeout.InfiniteTimeSpan);
        }
    }

    private void DebouncedHandler(object state)
    {
        var newSettings = (AppSettings)state;
        _appSettings = newSettings;

        // Implement additional logic here (e.g., update UI, adjust service behavior)
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _debounceTimer?.Dispose();
            _disposed = true;
        }
    }
}