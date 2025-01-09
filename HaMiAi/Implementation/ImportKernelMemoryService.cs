using Common.Settings;
using HaMiAi.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory.AI.Ollama;

namespace HaMiAi.Implementation;

/// <summary>
/// Service for importing data into the kernel memory.
/// </summary>
public class ImportKernelMemoryService : IImportKernelMemoryService
{
    private readonly IImportWebpageKernelMemory _importWebpageKernelMemory;
    private readonly IImportDocumentKernelMemory _importDocumentKernelMemory;
    private readonly OllamaConfig _ollamaConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportKernelMemoryService"/> class.
    /// </summary>
    /// <param name="options">The application settings options.</param>
    /// <param name="importWebpageKernelMemory">The service for importing webpages into the kernel memory.</param>
    /// <param name="importDocumentKernelMemory">The service for importing documents into the kernel memory.</param>
    public ImportKernelMemoryService(
        IOptions<OllamaSettings> options,
        IImportWebpageKernelMemory importWebpageKernelMemory,
        IImportDocumentKernelMemory importDocumentKernelMemory)
    {
        _importWebpageKernelMemory = importWebpageKernelMemory;
        _importDocumentKernelMemory = importDocumentKernelMemory;

        var ollamaSettings = options.Value;

        _ollamaConfig = new OllamaConfig
        {
            Endpoint = ollamaSettings.Endpoint,
            TextModel = new OllamaModelConfig(ollamaSettings.TextModelId),
            EmbeddingModel = new OllamaModelConfig(ollamaSettings.EmbeddingModelId)
        };
    }

    /// <summary>
    /// Asynchronously imports a webpage into the kernel memory.
    /// </summary>
    /// <param name="urlString">The URL of the webpage to import.</param>
    /// <param name="storeIndex">The store index for the import operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ImportWebpageAsync(string urlString, string storeIndex) =>
        await _importWebpageKernelMemory.ImportWebpageAsync(_ollamaConfig, urlString, storeIndex);

    /// <summary>
    /// Asynchronously imports a document into the kernel memory.
    /// </summary>
    /// <param name="filename">The filename of the document to import.</param>
    /// <param name="storeIndex">The store index for the import operation.</param>
    /// <param name="tag">A dictionary of tags associated with the document.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ImportDocumentAsync(string filename, string storeIndex, Dictionary<string, string> tag) =>
        await _importDocumentKernelMemory.ImportDocumentAsync(filename, storeIndex, tag);
}