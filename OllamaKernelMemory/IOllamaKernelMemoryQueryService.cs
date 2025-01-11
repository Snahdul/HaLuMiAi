using Common.Settings;
using Microsoft.KernelMemory;

namespace HaMiAi.KernelMemory;

/// <summary>
/// Interface for querying the kernel memory.
/// </summary>
public interface IOllamaKernelMemoryQueryService
{
    /// <summary>
    /// Asynchronously performs a query on the kernel memory.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama configuration.</param>
    /// <param name="prompt">The prompt to query.</param>
    /// <param name="storageIndex">The index of the memory to query.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the memory answer.</returns>
    Task<MemoryAnswer?> AskAsync(OllamaSettings ollamaSettings, string prompt, string storageIndex);

    /// <summary>
    /// Lists the indexes in the kernel memory.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama configuration.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result containing the list of indexes.
    /// </returns>
    Task<IEnumerable<IndexDetails>> ListIndexesAsync(OllamaSettings ollamaSettings);

    /// <summary>
    /// Searches the kernel memory based on the provided query.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama configuration.</param>
    /// <param name="query">The search query.</param>
    /// <param name="storageIndex">The index of the memory to search.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the search result.</returns>
    Task<SearchResult?> SearchAsync(OllamaSettings ollamaSettings, string query, string storageIndex);
}