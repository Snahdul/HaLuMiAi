using ChatConversationControl.Contracts;
using System.IO.Abstractions;

namespace ChatConversationControl.Implementation;

public sealed class ConversationManagerDefault : ConversationManager
{
    /// <inheritdoc />
    public ConversationManagerDefault(IFileSystem fileSystem, IFileDialogService fileDialogService) : base(fileSystem, fileDialogService)
    {
    }
}