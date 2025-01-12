using System.Windows.Controls;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.Views.Pages;

/// <summary>
/// Interaction logic for OllamaPage.xaml
/// </summary>
public partial class OllamaPage : Page, INavigationAware
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model for the Ollama page.</param>
    public OllamaPage(OllamaViewModel viewModel)
    {
        InitializeComponent();

        this.ViewModel = viewModel;
        this.DataContext = this;
    }

    /// <summary>
    /// Gets the view model for the Ollama page.
    /// </summary>
    public OllamaViewModel ViewModel { get; }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        ViewModel.OllamaMemoryViewModel.OnNavigatedTo();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        ViewModel.OllamaMemoryViewModel.OnNavigatedFrom();
    }

    #endregion
}