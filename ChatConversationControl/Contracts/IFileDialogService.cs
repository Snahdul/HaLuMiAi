namespace ChatConversationControl.Contracts;

/// <summary>
/// Represents a service for creating file dialogs.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Creates a new instance of a save file dialog.
    /// </summary>
    /// <returns>An instance of <see cref="ISaveFileDialog"/>.</returns>
    ISaveFileDialog CreateSaveFileDialog();

    /// <summary>
    /// Creates a new instance of an open file dialog.
    /// </summary>
    /// <returns>An instance of <see cref="IOpenFileDialog"/>.</returns>
    IOpenFileDialog CreateOpenFileDialog();
}