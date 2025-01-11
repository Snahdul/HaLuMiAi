using ChatConversationControl.Contracts;
using System.IO.Abstractions;

namespace ChatConversationControl.Implementation;

/// <summary>
/// Manages the conversation logic for the chat application.
/// </summary>
public sealed class ConversationManagerDefault : ConversationManager
{
    /// <inheritdoc />
    public ConversationManagerDefault(IFileSystem fileSystem, IFileDialogService fileDialogService) : base(fileSystem, fileDialogService)
    {
    }
}