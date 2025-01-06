using Microsoft.Extensions.Options;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Settings;
using WPFUiDesktopApp.ViewModels.Windows;

namespace WPFUiDesktopApp.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INavigationWindow
{
    /// <summary>
    /// Gets the ViewModel for the MainWindow.
    /// </summary>
    public MainWindowViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The ViewModel for the MainWindow.</param>
    /// <param name="pageService">The service for managing pages.</param>
    /// <param name="navigationService">The service for managing navigation.</param>
    /// <param name="options">The application settings.</param>
    public MainWindow(
        MainWindowViewModel viewModel,
        IPageService pageService,
        INavigationService navigationService,
        IOptions<AppSettings> options
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        SetPageService(pageService);

        var appSettings = options.Value;

        navigationService.SetNavigationControl(RootNavigation);

        ApplicationThemeManager.Apply(appSettings.CurrentTheme);
    }

    #region INavigationWindow methods

    /// <summary>
    /// Gets the navigation view.
    /// </summary>
    /// <returns>The navigation view.</returns>
    public INavigationView GetNavigation() => RootNavigation;

    /// <summary>
    /// Navigates to the specified page type.
    /// </summary>
    /// <param name="pageType">The type of the page to navigate to.</param>
    /// <returns>True if navigation was successful; otherwise, false.</returns>
    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    /// <summary>
    /// Sets the page service.
    /// </summary>
    /// <param name="pageService">The page service to set.</param>
    public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

    /// <summary>
    /// Shows the window.
    /// </summary>
    public void ShowWindow() => Show();

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void CloseWindow() => Close();

    #endregion INavigationWindow methods

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Gets the navigation view.
    /// </summary>
    /// <returns>The navigation view.</returns>
    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to set.</param>
    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }
}
