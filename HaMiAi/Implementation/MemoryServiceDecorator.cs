using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Context;
using Microsoft.KernelMemory.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;

namespace HaMiAi.Implementation;

/// <summary>
/// Decorator class for IKernelMemory to add logging and validation.
/// </summary>
public class MemoryServiceDecorator : IKernelMemory
{
    private readonly IFileSystem _fileSystem;
    private readonly IKernelMemory _kernelMemory;
    private readonly ILogger<MemoryServiceDecorator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryServiceDecorator"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="kernelMemory">The kernel memory instance to decorate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="kernelMemory" /> or <paramref name="fileSystem" /> is <see langword="null" />.</exception>
    public MemoryServiceDecorator(ILoggerFactory? loggerFactory, IFileSystem fileSystem, IKernelMemory kernelMemory)
    {
        Guard.IsNotNull(fileSystem);
        Guard.IsNotNull(kernelMemory);

        _fileSystem = fileSystem;
        _kernelMemory = kernelMemory;
        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<MemoryServiceDecorator>();
    }

    #region KernelMemoryExtensions

    /// <summary>
    /// Ask the given index for an answer to the given query return it without streaming the content.
    /// </summary>
    /// <param name="question">Question to answer</param>
    /// <param name="index">Optional index name</param>
    /// <param name="filter">Filter to match</param>
    /// <param name="filters">Filters to match (using inclusive OR logic). If 'filter' is provided too, the value is merged into this list.</param>
    /// <param name="minRelevance">Minimum Cosine Similarity required</param>
    /// <param name="options">Options for the request, such as whether to stream results</param>
    /// <param name="context">Unstructured data supporting custom business logic in the current request.</param>
    /// <param name="cancellationToken">Async task cancellation token</param>
    /// <returns>Answer to the query, if possible</returns>
    public async Task<MemoryAnswer> AskAsync(
        string question,
        string? index = null,
        MemoryFilter? filter = null,
        ICollection<MemoryFilter>? filters = null,
        double minRelevance = 0,
        SearchOptions? options = null,
        IContext? context = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrWhiteSpace(question);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error: question is null or whitespace.");
            return new MemoryAnswer();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error: question is null or whitespace.");
            return new MemoryAnswer();
        }

        return await _kernelMemory.AskAsync(question, index, filter, filters, minRelevance, options, context, cancellationToken);
    }

    /// <summary>
    /// Return a list of synthetic memories of the specified type
    /// </summary>
    /// <param name="syntheticType">Type of synthetic data to return</param>
    /// <param name="index">Optional name of the index where to search</param>
    /// <param name="filter">Filter to match</param>
    /// <param name="filters">Filters to match (using inclusive OR logic). If 'filter' is provided too, the value is merged into this list.</param>
    /// <param name="cancellationToken">Async task cancellation token</param>
    /// <returns>List of search results</returns>
    public async Task<List<Citation>> SearchSyntheticsAsync(string syntheticType,
        string? index = null,
        MemoryFilter? filter = null,
        ICollection<MemoryFilter>? filters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrWhiteSpace(syntheticType);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error: syntheticType is null or whitespace.");
            return [];
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error: syntheticType is null or whitespace.");
            return [];
        }

        return await _kernelMemory.SearchSyntheticsAsync(syntheticType, index, filter, filters, cancellationToken);
    }

    /// <summary>
    /// Return a list of summaries matching the given filters
    /// </summary>
    /// <param name="index">Optional name of the index where to search</param>
    /// <param name="filter">Filter to match</param>
    /// <param name="filters">Filters to match (using inclusive OR logic). If 'filter' is provided too, the value is merged into this list.</param>
    /// <param name="cancellationToken">Async task cancellation token</param>
    /// <returns>List of search results</returns>
    public async Task<List<Citation>> SearchSummariesAsync(
        string? index = null,
        MemoryFilter? filter = null,
        ICollection<MemoryFilter>? filters = null,
        CancellationToken cancellationToken = default)
    {
        return await _kernelMemory.SearchSyntheticsAsync(Constants.TagsSyntheticSummary, index, filter, filters, cancellationToken);
    }

    #endregion KernelMemoryExtensions

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<string> ImportDocumentAsync(string filePath, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNull(filePath);
            if (!_fileSystem.File.Exists(filePath))
            {
                return string.Empty;
            }

            return await _kernelMemory.ImportDocumentAsync(filePath, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while importing document from file path");
            return string.Empty;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error while importing text");
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public async Task<string> ImportWebPageAsync(string url, string? documentId = null, TagCollection? tags = null, string? index = null, IEnumerable<string>? steps = null, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrWhiteSpace(url);

            return await _kernelMemory.ImportWebPageAsync(url, documentId, tags, index, steps, context, cancellationToken);
        }
        catch (UriFormatException ex)
        {
            _logger.LogError(ex, "URI format error while importing web page");
            return string.Empty;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error: url is null or whitespace.");
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IndexDetails>> ListIndexesAsync(CancellationToken cancellationToken = default)
    {
        return await _kernelMemory.ListIndexesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteIndexAsync(string? index = null, CancellationToken cancellationToken = default)
    {
        await _kernelMemory.DeleteIndexAsync(index, cancellationToken);
    }

    /// <inheritdoc />
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument is empty error while deleting document");
        }
    }

    /// <inheritdoc />
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument is empty error while deleting document");
            return false;
        }
    }

    /// <inheritdoc />
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
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument is empty error while deleting document");
            return new DataPipelineStatus();
        }
    }

    /// <inheritdoc />
    public async Task<StreamableFileContent> ExportFileAsync(string documentId, string fileName, string? index = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(documentId);
            Guard.IsNotNullOrEmpty(fileName);
            Guard.IsTrue(_fileSystem.File.Exists(fileName));

            return await _kernelMemory.ExportFileAsync(documentId, fileName, index, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Argument null error while exporting file");
            return new StreamableFileContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument is empty error while deleting document");
            return new StreamableFileContent();
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "File not found error while exporting file");
            return new StreamableFileContent();
        }
    }

    /// <inheritdoc />
    public async Task<SearchResult> SearchAsync(string query, string? index = null, MemoryFilter? filter = null, ICollection<MemoryFilter>? filters = null, double minRelevance = 0, int limit = -1, IContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.IsNotNullOrEmpty(query);

            return await _kernelMemory.SearchAsync(query, index, filter, filters, minRelevance, limit, context, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error while searching");
            return new SearchResult();
        }
    }

    /// <inheritdoc />
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
            _logger.LogError(ex, "Argument null error while asking streaming");
            yield break;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument error while asking streaming");
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

