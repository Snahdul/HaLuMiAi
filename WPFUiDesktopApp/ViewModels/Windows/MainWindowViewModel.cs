using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace WPFUiDesktopApp.ViewModels.Windows
{
    /// <summary>
    /// ViewModel for the Main Window of the application.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets the title of the application.
        /// </summary>
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - UiDesktopApp";

        /// <summary>
        /// Gets or sets the collection of menu items for the navigation view.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<object> _menuItems =
        [
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Ollama",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Chat24 },
                TargetPageType = typeof(Views.Pages.OllamaPage)
            }
        ];

        /// <summary>
        /// Gets or sets the collection of footer menu items for the navigation view.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems =
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        ];

        /// <summary>
        /// Gets or sets the collection of tray menu items.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = [new MenuItem { Header = "Home", Tag = "tray_home" }];
    }
}
