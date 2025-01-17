namespace WPFUiDesktopApp.ViewModels.UserControls;

public interface IProcessManager
{
    /// <summary>
    /// Opens a file or URL using the appropriate method.
    /// </summary>
    /// <param name="pathOrUrl">The file path or URL to open.</param>
    /// <returns>True if the file or URL was opened successfully; otherwise, false.</returns>
    public bool Open(string pathOrUrl);
}