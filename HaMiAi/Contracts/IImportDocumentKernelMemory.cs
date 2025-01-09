namespace HaMiAi.Contracts;

/// <summary>
/// Interface for importing documents into the kernel memory.
/// </summary>
public interface IImportDocumentKernelMemory
{
    /// <summary>
    /// Asynchronously imports a document into the kernel memory.
    /// </summary>
    /// <param name="filename">The filename of the document to import.</param>
    /// <param name="storeIndex">The index for the import operation.</param>
    /// <param name="tag">A dictionary of tags associated with the document.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the document ID.</returns>
    Task<string> ImportDocumentAsync(string filename, string storeIndex, Dictionary<string, string> tag);
}