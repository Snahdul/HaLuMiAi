using Microsoft.Win32;

namespace ChatConversationControl.Contracts;

/// <summary>
/// Wraps the <see cref="SaveFileDialog"/> to implement the <see cref="ISaveFileDialog"/> interface.
/// </summary>
public class SaveFileDialogWrapper : ISaveFileDialog
{
    private readonly SaveFileDialog _saveFileDialog = new();

    /// <inheritdoc />
    public bool ShowDialog() => _saveFileDialog.ShowDialog() == true;

    /// <inheritdoc />
    public string FileName
    {
        get => _saveFileDialog.FileName;
        set => _saveFileDialog.FileName = value;
    }

    /// <inheritdoc />
    public string FileDialogFilter { get; set; } = "json files (*.json)|*.json|All files (*.*)|*.*";

    /// <inheritdoc />
    public string DefaultFileName { get; set; } = "Conversation.json";

    /// <inheritdoc />
    public string DefaultFileExtension { get; set; } = ".json";
}