using ChatConversationControl.Contracts;
using ChatConversationControl.Messages;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Implementation;

/// <summary>
/// Manages the conversation logic for the chat application.
/// </summary>
public abstract class ConversationManager : IConversationManager
{
    /// <summary>
    /// Gets the list of conversation messages.
    /// </summary>
    public ObservableCollection<MessageItem> ConversationList { get; } = [];

    /// <summary>
    /// Saves the conversation to a file.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public virtual async Task SaveConversation()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = "Conversation.txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var filePath = saveFileDialog.FileName;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var json = JsonSerializer.Serialize(ConversationList, options);
            await File.WriteAllTextAsync(filePath, json);
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
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = ".txt"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filePath = openFileDialog.FileName;
            var json = await File.ReadAllTextAsync(filePath);
            var messages = JsonSerializer.Deserialize<ObservableCollection<MessageItem>>(json);
            if (messages != null)
            {
                ConversationList.Clear();
                foreach (var message in messages)
                {
                    ConversationList.Add(message);
                }
            }
        }
    }
}