using CommunityToolkit.Mvvm.ComponentModel;
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
    [JsonPropertyName("text")]
    [ObservableProperty]
    private string _text = string.Empty;

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendText(string additionalText)
    {
        _textBuilder.Append(additionalText);
        Text += additionalText;
    }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendLineText(string additionalText)
    {
        _textBuilder.AppendLine(additionalText);
        Text = _textBuilder.ToString();
    }
}