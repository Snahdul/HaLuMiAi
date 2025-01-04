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

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversationManager"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileDialogService">The file dialog service.</param>
    protected ConversationManager(IFileSystem fileSystem, IFileDialogService fileDialogService)
    {
        Guard.IsNotNull(fileSystem);
        Guard.IsNotNull(fileDialogService);

        _fileSystem = fileSystem;
        _fileDialogService = fileDialogService;
    }

    /// <inheritdoc />
    public ObservableCollection<MessageItem> ConversationList { get; } = [];

    /// <inheritdoc />
    public virtual async Task SaveConversation()
    {
        ISaveFileDialog saveFileDialog = _fileDialogService.CreateSaveFileDialog();
        saveFileDialog.FileDialogFilter = FileDialogFilter;
        saveFileDialog.DefaultFileExtension = DefaultFileExtension;
        saveFileDialog.FileName = DefaultFileName;

        if (saveFileDialog.ShowDialog())
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

    /// <inheritdoc />
    public virtual async Task LoadConversation()
    {
        var openFileDialog = _fileDialogService.CreateOpenFileDialog();
        openFileDialog.FileDialogFilter = FileDialogFilter;
        openFileDialog.DefaultFileExtension = DefaultFileExtension;
        openFileDialog.FileName = DefaultFileName;

        if (openFileDialog.ShowDialog())
        {
            var filePath = openFileDialog.FileName;
            var jsonContent = await _fileSystem.File.ReadAllTextAsync(filePath);

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var conversationList = JsonSerializer.Deserialize<ObservableCollection<MessageItem>>(jsonContent, options);

            if (conversationList != null)
            {
                ConversationList.Clear();
                foreach (var message in conversationList)
                {
                    ConversationList.Add(message);
                }
            }
        }
    }
}
