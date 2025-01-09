using CommunityToolkit.Diagnostics;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.UserControls;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// ViewModel for the Ollama page, handling navigation and coordinating between chat and memory view models.
/// </summary>
public class OllamaViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaViewModel"/> class.
    /// </summary>
    /// <param name="ollamaChatViewModel">The view model for the Ollama chat page.</param>
    /// <param name="ollamaMemoryViewModel">The view model for the Ollama memory page.</param>
    /// <param name="addFileToMemoryViewModel"> The view model for adding a file to memory.</param>
    /// <param name="addWebpageToMemoryViewModel">The view model for adding a webpage to memory.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="ollamaChatViewModel" /> is <see langword="null" />.
    /// </exception>
    public OllamaViewModel(
        OllamaChatViewModel ollamaChatViewModel,
        OllamaMemoryViewModel ollamaMemoryViewModel,
        AddFileToMemoryViewModel addFileToMemoryViewModel,
        AddWebpageToMemoryViewModel addWebpageToMemoryViewModel)
    {
        Guard.IsNotNull(ollamaChatViewModel);
        Guard.IsNotNull(ollamaMemoryViewModel);

        OllamaChatViewModel = ollamaChatViewModel;
        OllamaMemoryViewModel = ollamaMemoryViewModel;
        AddFileToMemoryViewModel = addFileToMemoryViewModel;
        AddWebpageToMemoryViewModel = addWebpageToMemoryViewModel;
    }

    /// <summary>
    /// Gets the view model for the Ollama chat page.
    /// </summary>
    public OllamaChatViewModel OllamaChatViewModel { get; }

    public OllamaMemoryViewModel OllamaMemoryViewModel { get; }

    public AddFileToMemoryViewModel AddFileToMemoryViewModel { get; }

    public AddWebpageToMemoryViewModel AddWebpageToMemoryViewModel { get; }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        OllamaMemoryViewModel.OnNavigatedTo();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        OllamaMemoryViewModel.OnNavigatedFrom();
    }

    #endregion
}