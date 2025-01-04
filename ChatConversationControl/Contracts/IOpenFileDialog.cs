namespace ChatConversationControl.Contracts;

/// <summary>
/// Represents an open file dialog.
/// </summary>
public interface IOpenFileDialog
{
    /// <summary>
    /// Shows the open file dialog.
    /// </summary>
    /// <returns>True if the user clicks OK; otherwise, false.</returns>
    bool ShowDialog();

    /// <summary>
    /// Gets or sets a value indicating whether the dialog box allows multiple files to be selected.
    /// </summary>
    bool Multiselect { get; set; }

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

