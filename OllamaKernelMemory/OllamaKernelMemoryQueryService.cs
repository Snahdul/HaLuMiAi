using Common.Settings;
using HaMiAi.KernelMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.Handlers;
using Microsoft.KernelMemory.MemoryStorage.DevTools;

namespace OllamaKernelMemory;

/// <summary>
/// Service for querying the kernel memory.
/// </summary>
public class OllamaKernelMemoryQueryService : IOllamaKernelMemoryQueryService
{
    /// <summary>
    /// Asynchronously performs a query on the kernel memory.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama settings to use for the query.</param>
    /// <param name="prompt">The prompt to query.</param>
    /// <param name="storageIndex">The storage index of the memory to query.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the memory answer.</returns>
    public async Task<MemoryAnswer?> AskAsync(OllamaSettings ollamaSettings, string prompt, string storageIndex)
    {
        var memory = InitializeKernelMemory(ollamaSettings);
        return await memory.AskAsync(prompt, index: storageIndex);
    }

    /// <summary>
    /// Searches the kernel memory based on the provided query.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama settings to use for the search.</param>
    /// <param name="query">The search query.</param>
    /// <param name="storageIndex">The index of the memory to search.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the search result.</returns>
    public async Task<SearchResult?> SearchAsync(OllamaSettings ollamaSettings, string query, string storageIndex)
    {
        var memory = InitializeKernelMemory(ollamaSettings);
        return await memory.SearchAsync(query, storageIndex);
    }

    /// <summary>
    /// Lists the indexes in the kernel memory.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama settings to use for the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result containing the list of indexes.
    /// </returns>
    public async Task<IEnumerable<IndexDetails>> ListIndexesAsync(OllamaSettings ollamaSettings)
    {
        var memory = InitializeKernelMemory(ollamaSettings);
        return await memory.ListIndexesAsync();
    }

    public IHost CreateHost(OllamaConfig ollamaConfig)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var memoryBuilder = new KernelMemoryBuilder(services)
                    .WithoutDefaultHandlers() // remove default handlers, add our custom ones below
                    .WithSimpleQueuesPipeline()
                    .WithOllamaTextGeneration(ollamaConfig, new GPT4oTokenizer())
                    .WithOllamaTextEmbeddingGeneration(ollamaConfig, new GPT4oTokenizer())
                    .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
                    .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

                Type[] handlers =
                {
                    typeof(TextPartitioningHandler),
                    typeof(GenerateEmbeddingsHandler),
                    typeof(SaveRecordsHandler),
                };

                foreach (var handler in handlers)
                {
                    services.AddHandlerAsHostedService(handler, handler.Name);
                }

                services.AddSingleton(memoryBuilder.Build<MemoryService>());
            })
            .Build();
    }


    /// <summary>
    /// Initializes the kernel memory instance with Ollama configuration.
    /// </summary>
    /// <param name="ollamaSettings">The Ollama settings to use for the query.</param>
    /// <returns>The kernel memory instance.</returns>
    private IKernelMemory InitializeKernelMemory(OllamaSettings ollamaSettings)
    {
        var ollamaConfig = new OllamaConfig
        {
            Endpoint = "http://localhost:11434",
            TextModel = new OllamaModelConfig("llama3.2", 131072),
            EmbeddingModel = new OllamaModelConfig("nomic-embed-text", 2048)
        };

        try
        {
            var memoryBuilder = new KernelMemoryBuilder()
                .WithOllamaTextGeneration(ollamaConfig, new CL100KTokenizer())
                .WithOllamaTextEmbeddingGeneration(ollamaConfig, new CL100KTokenizer())
                .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
                .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

            memoryBuilder.Configure(
                builder => builder.Services.AddSingleton(memoryBuilder.Build()));

            var memory = memoryBuilder.Build();

            return memory;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}