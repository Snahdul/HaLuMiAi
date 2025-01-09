namespace HaMiAi.Contracts;

/// <summary>
/// Interface for importing data into the kernel memory.
/// </summary>
public interface IImportKernelMemoryService
{
    /// <summary>
    /// Asynchronously imports a webpage into the kernel memory.
    /// </summary>
    /// <param name="urlString">The URL of the webpage to import.</param>
    /// <param name="storeIndex">The identifier for the import operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ImportWebpageAsync(string urlString, string storeIndex);

    /// <summary>
    /// Asynchronously imports a document into the kernel memory.
    /// </summary>
    /// <param name="filename">The filename of the document to import.</param>
    /// <param name="storeIndex">The identifier for the import operation.</param>
    /// <param name="tag">A dictionary of tags associated with the document.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ImportDocumentAsync(string filename, string storeIndex, Dictionary<string, string> tag);
}