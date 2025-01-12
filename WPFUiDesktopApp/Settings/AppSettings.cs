using Common.Settings;
using Wpf.Ui.Appearance;

namespace WPFUiDesktopApp.Settings;

/// <summary>
/// Represents the application settings.
/// </summary>
public partial class AppSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the settings for the Ollama service.
    /// </summary>
    [ObservableProperty]
    private OllamaSettings _ollamaSettings = new();

    /// <summary>
    /// Gets or sets the current application theme.
    /// </summary>
    [ObservableProperty]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;
}