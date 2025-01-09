using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Messages;
using WPFUiDesktopApp.Models;
using WPFUiDesktopApp.ViewModels.Pages;

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
    /// <param name="webpageImportDialogViewModel">The webpage import dialog view model.</param>
    /// <param name="conversationControlViewModel"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="ollamaKernelMemory" /> or <paramref name="webpageImportDialogViewModel" /> is <see langword="null" />.
    /// </exception>
    public OllamaMemoryViewModel(
        ILoggerFactory? loggerFactory,
        UserControls.MemoryConversationControlViewModel conversationControlViewModel,
        OllamaMemoryModel ollamaKernelMemory,
        WebpageImportDialogViewModel webpageImportDialogViewModel)
    {
        Guard.IsNotNull(conversationControlViewModel);
        Guard.IsNotNull(ollamaKernelMemory);
        Guard.IsNotNull(webpageImportDialogViewModel);

        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<OllamaMemoryViewModel>();

        _ollamaMemoryModel = ollamaKernelMemory;
        WebpageImportDialogViewModel = webpageImportDialogViewModel;

        ConversationControlViewModel = conversationControlViewModel;
    }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        WeakReferenceMessenger.Default.Register<ImportWebPageMessage>(this, ImportWebPageMessageHandler);
        ConversationControlViewModel.OnNavigatedTo();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ImportWebPageMessage>(this);
    }

    #endregion

    /// <summary>
    /// Gets the conversation control view model.
    /// </summary>
    public UserControls.MemoryConversationControlViewModel ConversationControlViewModel { get; }

    /// <summary>
    /// Gets the webpage import dialog view model.
    /// </summary>
    public WebpageImportDialogViewModel WebpageImportDialogViewModel { get; }

    // Handler for ImportWebPageMessage
    private async void ImportWebPageMessageHandler(object recipient, ImportWebPageMessage message)
    {
        await _ollamaMemoryModel.UploadWebpageAsync(message.UrlString, "ToDo_Set_Storage_Index");
    }
}
