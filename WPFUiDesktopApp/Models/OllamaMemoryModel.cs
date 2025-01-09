using Common.Settings;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using HaMiAi.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;

namespace WPFUiDesktopApp.Models;

/// <summary>
/// Model class for managing Ollama memory operations.
/// </summary>
public class OllamaMemoryModel
{
    private readonly IOptions<OllamaSettings> _options;
    private readonly IKernelMemoryServiceFactory _kernelMemoryServiceFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaMemoryModel"/> class.
    /// </summary>
    /// <param name="options">The application settings for Ollama options.</param>
    /// <param name="kernelMemoryServiceFactory">The factory for creating kernel memory services.</param>
    public OllamaMemoryModel(IOptions<OllamaSettings> options, IKernelMemoryServiceFactory kernelMemoryServiceFactory)
    {
        Guard.IsNotNull(kernelMemoryServiceFactory);

        _options = options;
        _kernelMemoryServiceFactory = kernelMemoryServiceFactory;
    }

    /// <summary>
    /// Asynchronously performs a query on the kernel memory.
    /// </summary>
    /// <param name="prompt">The prompt to query.</param>
    /// <param name="storageIndex">The index of the memory to query.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the memory answer.</returns>
    public async Task<MemoryAnswer?> AskAsync(string prompt, string storageIndex)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return default;
        }

        return await ExecuteMemoryOperationAsync(async memoryServiceDecorator => await memoryServiceDecorator.AskAsync(prompt, index: storageIndex));
    }

    /// <summary>
    /// Searches the kernel memory based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="storageIndex">The index of the memory to search.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the search result.</returns>
    public async Task<SearchResult?> SearchAsync(string query, string storageIndex)
    {
        if (string.IsNullOrEmpty(query))
        {
            return default;
        }

        return await ExecuteMemoryOperationAsync(async memoryServiceDecorator => await memoryServiceDecorator.SearchAsync(query, index: storageIndex));
    }

    /// <summary>
    /// Uploads a webpage to the kernel memory asynchronously.
    /// </summary>
    /// <param name="urlString">The URL of the webpage to upload.</param>
    /// <param name="storeIndex">The index for the import operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UploadWebpageAsync(string urlString, string storeIndex)
    {
        await ExecuteMemoryOperationAsync(async memoryServiceDecorator =>
        {
            await memoryServiceDecorator.ImportWebPageAsync(url: urlString, index: storeIndex);
            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Lists all indexes in the kernel memory.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a result containing the list of indexes.</returns>
    public async Task<IEnumerable<IndexDetails>> ListIndexesAsync()
    {
        return await ExecuteMemoryOperationAsync(async memoryServiceDecorator => await memoryServiceDecorator.ListIndexesAsync());
    }

    /// <summary>
    /// Executes a memory operation asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The memory operation to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the specified type.</returns>
    private async Task<T> ExecuteMemoryOperationAsync<T>(Func<MemoryServiceDecorator, Task<T>> operation)
    {
        var host = _kernelMemoryServiceFactory.CreateHostWithDefaultMemoryPipeline(options: _options);
        await host.StartAsync(CancellationToken.None);

        var memoryServiceDecorator = host.Services.GetRequiredService<MemoryServiceDecorator>();
        var result = await operation(memoryServiceDecorator);

        await host.StopAsync(CancellationToken.None);
        return result;
    }
}
