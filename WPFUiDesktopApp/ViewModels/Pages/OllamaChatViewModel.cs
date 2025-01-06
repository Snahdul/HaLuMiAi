using CommunityToolkit.Diagnostics;
using WPFUiDesktopApp.ViewModels.UserControls;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// The view model for the Ollama chat page.
/// </summary>
public partial class OllamaChatViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaChatViewModel"/> class.
    /// </summary>
    public OllamaChatViewModel(
        ConversationControlViewModel conversationControlViewModel)
    {
        Guard.IsNotNull(conversationControlViewModel);

        ConversationControlViewModel = conversationControlViewModel;
    }

    /// <summary>
    /// Gets the conversation control view model.
    /// </summary>
    public ConversationControlViewModel ConversationControlViewModel { get; }
}