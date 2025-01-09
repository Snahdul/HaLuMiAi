using Common.Settings;
using HaMiAi.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Handlers;

namespace HaMiAi.Implementation;

/// <summary>
/// Service for importing documents into the kernel memory.
/// </summary>
public sealed class ImportDocumentKernelMemory : IImportDocumentKernelMemory, IDisposable
{
    private readonly ILogger<ImportDocumentKernelMemory> _logger;
    private readonly IHost _host;
    private MemoryServerless? _memoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportDocumentKernelMemory"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="kernelMemoryServiceFactory">The kernel memory service factory.</param>
    /// <param name="options">The application settings for Ollama options.</param>
    public ImportDocumentKernelMemory(ILoggerFactory? loggerFactory,
        IKernelMemoryServiceFactory kernelMemoryServiceFactory,
        IOptions<OllamaSettings> options)
    {
        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<ImportDocumentKernelMemory>();

        _host = kernelMemoryServiceFactory.CreateHostWithDefaultMemoryPipeline(options);
    }

    /// <summary>
    /// Asynchronously imports a document into the kernel memory.
    /// </summary>
    /// <param name="filename">The filename of the document to import.</param>
    /// <param name="storeIndex">The store index for the import operation.</param>
    /// <param name="tag">A dictionary of tags associated with the document.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the document ID.</returns>
    public async Task<string> ImportDocumentAsync(string filename, string storeIndex, Dictionary<string, string> tag)
    {
        await InitializeAsync();

        if (_memoryService == null)
        {
            _logger.LogError("Memory service is not initialized.");
            return string.Empty;
        }

        var documentToAdd = CreateDocument(filename, tag);

        var docId = await _memoryService.ImportDocumentAsync(
            documentToAdd,
            steps: new[]
            {
                nameof(TextExtractionHandler),
                //nameof(TextPartitioningHandler),
                //nameof(GenerateEmbeddingsHandler),
                //nameof(SaveRecordsHandler),
                //nameof(HaMiSummarizationHandler),
            }, index: storeIndex);

        await WaitForImportCompletionAsync(docId, storeIndex);

        _logger.LogInformation("* Document import completed with {docId}.", docId);

        return docId;
    }

    /// <summary>
    /// Creates a document with the specified filename and tags.
    /// </summary>
    /// <param name="filename">The filename of the document.</param>
    /// <param name="tag">A dictionary of tags associated with the document.</param>
    /// <returns>The created document.</returns>
    private Document CreateDocument(string filename, Dictionary<string, string> tag)
    {
        var documentToAdd = new Document().AddFile(filename);
        tag.Keys?.ToList()?.ForEach(key => documentToAdd.AddTag(key, tag[key]));
        return documentToAdd;
    }

    /// <summary>
    /// Waits for the import operation to complete.
    /// </summary>
    /// <param name="docId">The document ID.</param>
    /// <param name="storeIndex">The store index for the import operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task WaitForImportCompletionAsync(string docId, string storeIndex)
    {
        if (_memoryService == null)
        {
            _logger.LogError("Memory service is not initialized.");
            return;
        }

        var status = await _memoryService.GetDocumentStatusAsync(documentId: docId, storeIndex);

        while (status is { Completed: false })
        {
            _logger.LogDebug("* Work in progress...");
            _logger.LogDebug("Steps:     {steps}", string.Join(", ", status.Steps));
            _logger.LogDebug("Completed: {completedSteps}", string.Join(", ", status.CompletedSteps));
            _logger.LogDebug("Remaining: {RemainingSteps}", string.Join(", ", status.RemainingSteps));
            _logger.LogDebug("*********************");
            await Task.Delay(TimeSpan.FromSeconds(2));
            status = await _memoryService.GetDocumentStatusAsync(documentId: docId, storeIndex);
        }
    }

    /// <summary>
    /// Initializes the memory service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeAsync()
    {
        _memoryService = _host.Services.GetRequiredService<MemoryServerless>();
        await _host.StartAsync();
    }

    /// <summary>
    /// Disposes the resources used by the class.
    /// </summary>
    public void Dispose()
    {
        _host?.Dispose();
    }
}
