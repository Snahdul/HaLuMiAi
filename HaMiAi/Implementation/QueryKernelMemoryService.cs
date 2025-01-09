using Common.Settings;
using HaMiAi.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.MemoryStorage.DevTools;

namespace HaMiAi.Implementation;

/// <summary>
/// Service for querying the kernel memory.
/// </summary>
public class QueryKernelMemoryService : IQueryKernelMemoryService
{
    private readonly OllamaConfig _ollamaConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryKernelMemoryService"/> class.
    /// </summary>
    /// <param name="options">The application settings options.</param>
    public QueryKernelMemoryService(IOptions<OllamaSettings> options)
    {
        var ollamaSettings = options.Value;

        _ollamaConfig = new OllamaConfig
        {
            Endpoint = ollamaSettings.Endpoint,
            TextModel = new OllamaModelConfig(ollamaSettings.TextModelId),
            EmbeddingModel = new OllamaModelConfig(ollamaSettings.EmbeddingModelId)
        };
    }

    /// <summary>
    /// Asynchronously performs a query on the kernel memory.
    /// </summary>
    /// <param name="prompt">The prompt to query.</param>
    /// <param name="storageIndex">The storage index of the memory to query.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the memory answer.</returns>
    public async Task<MemoryAnswer?> AskAsync(string prompt, string storageIndex)
    {
        var memory = InitializeKernelMemory();
        return await memory.AskAsync(prompt, index: storageIndex);
    }

    /// <summary>
    /// Searches the kernel memory based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="storageIndex">The index of the memory to search.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the search result.</returns>
    public async Task<SearchResult?> SearchAsync(string query, string storageIndex)
    {
        var memory = InitializeKernelMemory();
        return await memory.SearchAsync(query, storageIndex);
    }

    /// <summary>
    /// Lists the indexes in the kernel memory.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, with a result containing the list of indexes.
    /// </returns>
    public async Task<IEnumerable<IndexDetails>> ListIndexesAsync()
    {
        var memory = InitializeKernelMemory();
        return await memory.ListIndexesAsync();
    }

    /// <summary>
    /// Initializes the kernel memory instance with Ollama configuration.
    /// </summary>
    /// <returns>The kernel memory instance.</returns>
    private IKernelMemory InitializeKernelMemory()
    {
        var memory = new KernelMemoryBuilder()
            .WithOllamaTextGeneration(_ollamaConfig, new GPT4oTokenizer())
            .WithOllamaTextEmbeddingGeneration(_ollamaConfig, new GPT4oTokenizer())
            .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent)
            .Configure(builder => builder.Services.AddLogging(l =>
            {
#if DEBUG
                l.SetMinimumLevel(LogLevel.Trace);
#else
                l.SetMinimumLevel(LogLevel.Warning);
#endif
                l.AddSimpleConsole(c => c.SingleLine = true);
            }))
            .Build<MemoryServerless>();

        return memory;
    }
}