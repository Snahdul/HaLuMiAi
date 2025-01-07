using Common.Settings;
using Wpf.Ui.Appearance;

namespace WPFUiDesktopApp.Settings
{
    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        private OllamaSettings _ollamaSettings = new();

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;
    }
}