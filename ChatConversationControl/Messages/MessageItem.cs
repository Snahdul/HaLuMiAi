using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace ChatConversationControl.Messages;

/// <summary>
/// Represents a message item with text and color properties.
/// </summary>
public partial class MessageItem : ObservableObject
{
    private readonly StringBuilder _textBuilder = new();

    [ObservableProperty]
    private string _colorString = "";

    /// <summary>
    /// Gets or sets the text of the message.
    /// </summary>
    public string Text
    {
        get => _textBuilder.ToString();
        set
        {
            if (_textBuilder.ToString() == value) return;
            _textBuilder.Clear();
            _textBuilder.Append(value);
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendText(string additionalText)
    {
        _textBuilder.Append(additionalText);
        OnPropertyChanged(nameof(Text));
    }

    /// <summary>
    /// Appends additional text to the existing message text.
    /// </summary>
    /// <param name="additionalText">The text to append.</param>
    public void AppendLineText(string additionalText)
    {
        _textBuilder.AppendLine(additionalText);
        OnPropertyChanged(nameof(Text));
    }
}