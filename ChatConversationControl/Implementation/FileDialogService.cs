using ChatConversationControl.Contracts;

namespace ChatConversationControl.Implementation;

/// <summary>
/// Represents a service for creating file dialogs.
/// </summary>
public class FileDialogService : IFileDialogService
{
    /// <summary>
    /// Creates a new instance of a save file dialog.
    /// </summary>
    /// <returns>An instance of <see cref="ISaveFileDialog"/>.</returns>
    public ISaveFileDialog CreateSaveFileDialog() => new SaveFileDialogWrapper();

    /// <summary>
    /// Creates a new instance of an open file dialog.
    /// </summary>
    /// <returns>An instance of <see cref="IOpenFileDialog"/>.</returns>
    public IOpenFileDialog CreateOpenFileDialog() => new OpenFileDialogWrapper();
}