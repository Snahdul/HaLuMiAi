using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;
using Wpf.Ui.Controls;

namespace WPFUiDesktopApp.ViewModels;

/// <summary>
/// ViewModel for the Ollama memory page.
/// </summary>
public partial class OllamaMemoryViewModel : ObservableObject, INavigationAware
{
    private readonly ILogger<OllamaMemoryViewModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaMemoryViewModel"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="conversationControlViewModel"></param>
    public OllamaMemoryViewModel(
        ILoggerFactory? loggerFactory,
        UserControls.MemoryConversationControlViewModel conversationControlViewModel)
    {
        Guard.IsNotNull(conversationControlViewModel);

        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<OllamaMemoryViewModel>();

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
