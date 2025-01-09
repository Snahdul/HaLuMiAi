using Microsoft.KernelMemory.AI.Ollama;

namespace HaMiAi.Contracts;

/// <summary>
/// Interface for importing web page into the kernel memory.
/// </summary>
public interface IImportWebpageKernelMemory
{
    /// <summary>
    /// Asynchronously imports a webpage into the kernel memory.
    /// </summary>
    /// <param name="ollamaConfig">The Ollama configuration to use for the import operation.</param>
    /// <param name="urlString">The URL of the webpage to import.</param>
    /// <param name="storeIndex">The identifier for the import operation.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the document ID.</returns>
    Task<string> ImportWebpageAsync(OllamaConfig ollamaConfig, string urlString, string storeIndex);
}