namespace ChatConversationControl.Contracts;

/// <summary>
/// Represents a save file dialog.
/// </summary>
public interface ISaveFileDialog
{
    /// <summary>
    /// Shows the save file dialog.
    /// </summary>
    /// <returns>True if the user clicks OK; otherwise, false.</returns>
    bool ShowDialog();

    /// <summary>
    /// Gets or sets the file name selected in the dialog.
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Gets or sets the filter string that determines what types of files are displayed.
    /// </summary>
    public string FileDialogFilter { get; set; }

    /// <summary>
    /// Gets or sets the default file name.
    /// </summary>
    public string DefaultFileName { get; set; }

    /// <summary>
    /// Gets or sets the default file extension.    
    /// </summary>
    public string DefaultFileExtension { get; set; }
}