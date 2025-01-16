using System.Diagnostics;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// Manages process execution.
/// </summary>
public class ProcessManager : IProcessManager
{
    #region Implementation of IProcessManager

    /// <summary>
    /// Opens a file or URL using the appropriate method.
    /// </summary>
    /// <param name="pathOrUrl">The file path or URL to open.</param>
    /// <returns>True if the file or URL was opened successfully; otherwise, false.</returns>
    public bool Open(string pathOrUrl)
    {
        if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return OpenUrlInDefaultBrowser(pathOrUrl);
        }

        return StartProcess(new ProcessStartInfo
        {
            FileName = pathOrUrl,
            UseShellExecute = true
        });
    }

    #endregion

    /// <summary>
    /// Starts a process using the specified start information.
    /// </summary>
    /// <param name="processStartInfo">The start information for the process.</param>
    /// <returns>True if the process was started successfully; otherwise, false.</returns>
    private bool StartProcess(ProcessStartInfo processStartInfo)
    {
        processStartInfo.UseShellExecute = true; // Use the default program associated with the file type
        var process = new Process { StartInfo = processStartInfo };

        return process.Start();
    }

    /// <summary>
    /// Executes a process using the specified start information and returns the output.
    /// </summary>
    /// <param name="processStartInfo">The start information for the process.</param>
    /// <returns>The output of the process.</returns>
    private string ExecuteProcessAndGetOutput(ProcessStartInfo processStartInfo)
    {
        processStartInfo.UseShellExecute = false; // UseShellExecute must be false to redirect output
        processStartInfo.RedirectStandardOutput = true; // Redirect standard output to capture it
        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }

    /// <summary>
    /// Opens a URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    /// <returns>True if the URL was opened successfully; otherwise, false.</returns>
    private bool OpenUrlInDefaultBrowser(string url)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true // Use the default web browser
        };

        var process = new Process { StartInfo = processStartInfo };
        return process.Start();
    }
}
