using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Models;

namespace WPFUiDesktopApp.ViewModels;

/// <summary>
/// ViewModel for the Ollama memory page.
/// </summary>
public partial class OllamaMemoryViewModel : ObservableObject, INavigationAware
{
    private readonly OllamaMemoryModel _ollamaMemoryModel;
    private readonly ILogger<OllamaMemoryViewModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaMemoryViewModel"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="ollamaKernelMemory">The Ollama memory model.</param>
    /// <param name="conversationControlViewModel"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="ollamaKernelMemory" /> is <see langword="null" />.
    /// </exception>
    public OllamaMemoryViewModel(
        ILoggerFactory? loggerFactory,
        UserControls.MemoryConversationControlViewModel conversationControlViewModel,
        OllamaMemoryModel ollamaKernelMemory)
    {
        Guard.IsNotNull(conversationControlViewModel);
        Guard.IsNotNull(ollamaKernelMemory);

        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<OllamaMemoryViewModel>();

        _ollamaMemoryModel = ollamaKernelMemory;

        ConversationControlViewModel = conversationControlViewModel;
    }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        ConversationControlViewModel.OnNavigatedTo();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
    }

    #endregion

    /// <summary>
    /// Gets the conversation control view model.
    /// </summary>
    public UserControls.MemoryConversationControlViewModel ConversationControlViewModel { get; }
}
