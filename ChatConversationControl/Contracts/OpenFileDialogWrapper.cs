using Microsoft.Win32;

namespace ChatConversationControl.Contracts;

/// <summary>
/// Wraps the <see cref="OpenFileDialog"/> to implement the <see cref="IOpenFileDialog"/> interface.
/// </summary>
public class OpenFileDialogWrapper : IOpenFileDialog
{
    private readonly OpenFileDialog _openFileDialog = new();

    /// <inheritdoc />
    public bool ShowDialog()
    {
        _openFileDialog.Multiselect = Multiselect;
        _openFileDialog.Filter = FileDialogFilter;
        _openFileDialog.FileName = DefaultFileName;
        _openFileDialog.DefaultExt = DefaultFileExtension;
        return _openFileDialog.ShowDialog() == true;
    }

    /// <inheritdoc />
    public bool Multiselect { get; set; }

    /// <inheritdoc />
    public string FileName
    {
        get => _openFileDialog.FileName;
        set => _openFileDialog.FileName = value;
    }

    /// <inheritdoc />
    public string FileDialogFilter { get; set; } = "json files (*.json)|*.json|All files (*.*)|*.*";

    /// <inheritdoc />
    public string DefaultFileName { get; set; } = "Conversation.json";

    /// <inheritdoc />
    public string DefaultFileExtension { get; set; } = ".json";
}