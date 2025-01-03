using ChatConversationControl.Contracts;
using ChatConversationControl.Messages;
using CommunityToolkit.Diagnostics;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Implementation;

/// <summary>
/// Manages the conversation logic for the chat application.
/// </summary>
public abstract class ConversationManager : IConversationManager
{
    private const string FileDialogFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
    private const string DefaultFileName = "Conversation.json";
    private const string DefaultFileExtension = ".json";
    private readonly IFileSystem _fileSystem;
    private readonly IFileDialogService _fileDialogService;

    protected ConversationManager(IFileSystem fileSystem, IFileDialogService fileDialogService)
    {
        Guard.IsNotNull(fileSystem);
        Guard.IsNotNull(fileDialogService);

        _fileSystem = fileSystem;
        _fileDialogService = fileDialogService;
    }

    /// <summary>
    /// Gets the list of conversation messages.
    /// </summary>
    public ObservableCollection<MessageItem> ConversationList { get; } = new();

    /// <summary>
    /// Saves the conversation to a file.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public virtual async Task SaveConversation()
    {
        var saveFileDialog = _fileDialogService.CreateSaveFileDialog();
        if (saveFileDialog.ShowDialog() == true)
        {
            var filePath = saveFileDialog.FileName;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var jsonContent = JsonSerializer.Serialize(ConversationList, options);
            await _fileSystem.File.WriteAllTextAsync(filePath, jsonContent);
        }
    }

    /// <summary>
    /// Loads a conversation from a file.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    /// <exception cref="JsonException">An error occurred while deserializing the JSON content.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="NotSupportedException">There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter" /> for <see cref="ObservableCollection{MessageItem}" /> or its serializable members.</exception>
    public virtual async Task LoadConversation()
    {
        var openFileDialog = _fileDialogService.CreateOpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            var filePath = openFileDialog.FileName;
            var jsonContent = await _fileSystem.File.ReadAllTextAsync(filePath);

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve // Include this
            };
            var conversationList = JsonSerializer.Deserialize<ObservableCollection<MessageItem>>(jsonContent, options);

            ConversationList.Clear();
            foreach (var message in conversationList)
            {
                ConversationList.Add(message);
            }
        }
    }
}
