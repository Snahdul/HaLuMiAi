using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;
using Microsoft.KernelMemory.Diagnostics;
using System.Runtime.CompilerServices;

namespace HaMiAi.Implementation;

public class MemoryServiceDecorator : IKernelMemory
{
    private readonly IKernelMemory _kernelMemory;
    private readonly ILogger<MemoryServiceDecorator> _logger;

    public MemoryServiceDecorator(ILoggerFactory? loggerFactory, IKernelMemory kernelMemory)
    {
        Guard.IsNotNull(kernelMemory);

        _kernelMemory = kernelMemory;
        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<MemoryServiceDecorator>();
    }

    public async Task<string> ImportDocumentAsync(Document document, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNull(document);
            return await _kernelMemory.ImportDocumentAsync(document, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing document");
            return string.Empty;
        }
    }

    public async Task<string> ImportDocumentAsync(string filePath, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNull(filePath);
            Guard.IsTrue(File.Exists(filePath));

            return await _kernelMemory.ImportDocumentAsync(filePath, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing document from file path");
            return string.Empty;
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "File not found error while importing document from file path");
            return string.Empty;
        }
    }

    public async Task<string> ImportDocumentAsync(DocumentUploadRequest uploadRequest, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNull(uploadRequest);

            return await _kernelMemory.ImportDocumentAsync(uploadRequest, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing document upload request");
            return string.Empty;
        }
    }

    public async Task<string> ImportDocumentAsync(Stream content, string? fileName = null, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNull(content);

            return await _kernelMemory.ImportDocumentAsync(content, fileName, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing document from stream");
            return string.Empty;
        }
    }

    public async Task<string> ImportTextAsync(string text, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(text);

            return await _kernelMemory.ImportTextAsync(text, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing text");
            return string.Empty;
        }
    }

    public async Task<string> ImportWebPageAsync(string url, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(url);

            return await _kernelMemory.ImportWebPageAsync(url, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing web page");
            return string.Empty;
        }
        catch (UriFormatException ex)
        {
            _logger.LogError(ex, "URI format error while importing web page");
            return string.Empty;
        }
    }

    public async Task<IEnumerable<IndexDetails>> ListIndexesAsync(CancellationToken cancellationToken = default)
    {
        return await _kernelMemory.ListIndexesAsync(cancellationToken);
    }

    public async Task DeleteIndexAsync(string? index = null, CancellationToken cancellationToken = default)
    {
        await _kernelMemory.DeleteIndexAsync(index, cancellationToken);
    }

    public async Task DeleteDocumentAsync(string documentId, string? index = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(documentId);

            await _kernelMemory.DeleteDocumentAsync(documentId, index, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while deleting document");
        }
    }

    public async Task<bool> IsDocumentReadyAsync(string documentId, string? index = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(documentId);
            return await _kernelMemory.IsDocumentReadyAsync(documentId, index, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while checking if document is ready");
            return false;
        }
    }

    public async Task<DataPipelineStatus?> GetDocumentStatusAsync(string documentId, string? index = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(documentId);
            return await _kernelMemory.GetDocumentStatusAsync(documentId, index, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while getting document status");
            return new DataPipelineStatus();
        }
    }

    public async Task<StreamableFileContent> ExportFileAsync(string documentId, string fileName, string? index = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(documentId);
            Guard.IsNotNullOrEmpty(fileName);
            Guard.IsTrue(File.Exists(fileName));

            return await _kernelMemory.ExportFileAsync(documentId, fileName, index, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while exporting file");
            return new StreamableFileContent();
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "File not found error while exporting file");
            return new StreamableFileContent();
        }
    }

    public async Task<SearchResult> SearchAsync(string query, string? index = null, MemoryFilter? filter = null, ICollection<MemoryFilter>? filters = null, double minRelevance = 0, int limit = -1, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(query);

            return await _kernelMemory.SearchAsync(query, index, filter, filters, minRelevance, limit, context, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument null error while search");
            return new SearchResult();
        }
    }

    public async IAsyncEnumerable<MemoryAnswer> AskStreamingAsync(string question, string? index = null,
        MemoryFilter? filter = null, ICollection<MemoryFilter>? filters = null, double minRelevance = 0,
        SearchOptions? options = null, IContext? context = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrWhiteSpace(question);
        }
        catch (ArgumentNullException ex)
        {
            yield break;
        }

        await foreach (var answer in _kernelMemory
                           .AskStreamingAsync(question, index, filter, filters, minRelevance, options, context,
                               cancellationToken).ConfigureAwait(false))
        {
            yield return answer;
        }
    }
}
