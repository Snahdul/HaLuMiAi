using ChatConversationControl.Contracts;
using ChatConversationControl.ViewModels;
using Microsoft.Extensions.AI;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// ViewModel for managing conversation control.
/// </summary>
public partial class ConversationControlViewModel : BaseConversationControlViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> is <see langword="null" />.
    /// </exception>
    public ConversationControlViewModel(
        IConversationManager conversationManager,
        IChatClient chatClient) : base(conversationManager, chatClient)
    {
    }
}