using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Messages;

/// <summary>
/// Represents a message item in the conversation.
/// </summary>
public partial class MessageItem : ObservableObject
{
    private readonly StringBuilder _textBuilder = new();

    /// <summary>
    /// Gets or sets the color string for the message.
    /// </summary>
    [JsonPropertyName("colorString")]
    [ObservableProperty]
    private string _colorString = string.Empty;

    /// <summary>
    /// Gets or sets the text for the message.
    /// </summary>
    [JsonPropertyName("chatMessageContent")]
    [ObservableProperty]
    private ChatMessageContent _chatMessageContent;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageItem"/> class.
    /// </summary>
    public MessageItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageItem"/> class.
    /// </summary>
    /// <param name="chatMessageContent"></param>
    public MessageItem(ChatMessageContent chatMessageContent)
    {
        ChatMessageContent = chatMessageContent;
    }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendText(string additionalText)
    {
        _textBuilder.Append(additionalText);
        ChatMessageContent.Content = _textBuilder.ToString();
        OnPropertyChanged(nameof(ChatMessageContent));
    }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendLineText(string additionalText)
    {
        _textBuilder.AppendLine(additionalText);
        ChatMessageContent.Content = _textBuilder.ToString();
        OnPropertyChanged(nameof(ChatMessageContent));
    }
}