using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Messages;

public partial class MessageItem : ObservableObject
{
    private readonly StringBuilder _textBuilder = new();

    [JsonPropertyName("colorString")]
    public string ColorString { get; set; } = "";

    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendText(string additionalText)
    {
        _textBuilder.Append(additionalText);
        Text = _textBuilder.ToString();
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