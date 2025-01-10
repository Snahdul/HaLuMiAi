using Common.Settings;
using HaMiAi.Contracts;
using HaMiAi.Implementation.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.Handlers;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using System.IO.Abstractions;

namespace HaMiAi.Implementation;

/// <summary>
/// Factory for creating the kernel memory service.
/// </summary>
/// <param name="options">The application settings for Ollama options.</param>
public class KernelMemoryServiceFactory(ILoggerFactory? loggerFactory, IFileSystem fileSystem, IOptions<OllamaSettings> options) : IKernelMemoryServiceFactory
{
    private readonly IFileSystem _fileSystem = fileSystem;

    private readonly ILogger<KernelMemoryServiceFactory> _logger =
        (loggerFactory ?? DefaultLogger.Factory).CreateLogger<KernelMemoryServiceFactory>();

    /// <summary>
    /// Creates the host with the default memory pipeline.
    /// </summary>
    /// <returns>The created host.</returns>
    public IHost CreateHostWithDefaultMemoryPipeline()
    {
        var ollamaSettings = options.Value;

        var ollamaConfig = new OllamaConfig
        {
            Endpoint = ollamaSettings.Endpoint,
            TextModel = new OllamaModelConfig(ollamaSettings.TextModelId),
            EmbeddingModel = new OllamaModelConfig(ollamaSettings.EmbeddingModelId)
        };

        var host = new HostApplicationBuilder();

        IKernelMemoryBuilder memoryBuilder = new KernelMemoryBuilder(host.Services)
            .WithSimpleQueuesPipeline()
            .WithOllamaTextGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithOllamaTextEmbeddingGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

        MemoryServerless memoryServerless = memoryBuilder.Build<MemoryServerless>();
        MemoryServiceDecorator memoryServiceDecorator = new(new NullLoggerFactory(), _fileSystem, memoryServerless);

        memoryBuilder.Services.AddSingleton<MemoryServiceDecorator>(memoryServiceDecorator);

        host.Services.AddSingleton(memoryBuilder);

        var hostingApp = host.Build();

        return hostingApp;
    }

    /// <summary>
    /// Creates the host with a custom memory pipeline.
    /// </summary>
    /// <param name="handlers">The handlers to register in the pipeline.</param>
    /// <returns>The created host.</returns>
    public IHost CreateHostWithCustomMemoryPipeline(Type[] handlers)
    {
        var ollamaSettings = options.Value;

        var ollamaConfig = new OllamaConfig
        {
            Endpoint = ollamaSettings.Endpoint,
            TextModel = new OllamaModelConfig(ollamaSettings.TextModelId),
            EmbeddingModel = new OllamaModelConfig(ollamaSettings.EmbeddingModelId)
        };

        // Alternative for web apps: var host = WebApplication.CreateBuilder();
        HostApplicationBuilder host = new HostApplicationBuilder();

        var memoryBuilder = new KernelMemoryBuilder(host.Services)
            .WithoutDefaultHandlers() // remove default handlers, add our custom ones below
            .WithSimpleQueuesPipeline()
            .WithOllamaTextGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithOllamaTextEmbeddingGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

        foreach (var handler in handlers)
        {
            if (handler == typeof(TextExtractionHandler))
            {
                host.Services.AddHandlerAsHostedService(handler, Constants.PipelineStepsExtract);
            }
            else if (handler == typeof(TextPartitioningHandler))
            {
                host.Services.AddHandlerAsHostedService(handler, Constants.PipelineStepsPartition);
            }
            else if (handler == typeof(GenerateEmbeddingsHandler))
            {
                host.Services.AddHandlerAsHostedService(handler, Constants.PipelineStepsGenEmbeddings);
            }
            else if (handler == typeof(SaveRecordsHandler))
            {
                host.Services.AddHandlerAsHostedService(handler, Constants.PipelineStepsSaveRecords);
            }
            else if (handler == typeof(HaMiSummarizationHandler))
            {
                host.Services.AddHandlerAsHostedService(handler, Constants.PipelineStepsSummarize);
            }
            else
            {
                _logger.LogWarning("Handler {handler} is not recognized.", handler.Name);
            }
        }

        /*********************************************************
         * Start asynchronous handlers
         *********************************************************/

        // Notes:
        // * It's recommended building the Memory before building the hosting app, because KM builder
        //   might register missing dependencies in the shared service collection.
        // * Build() and Build<MemoryService>() in this case are equivalent because we added a queue service
        //IKernelMemory memory = memoryBuilder.Build<MemoryServerless>();

        MemoryService memoryService = memoryBuilder.Build<MemoryService>();
        MemoryServiceDecorator memoryServiceDecorator = new(new NullLoggerFactory(), fileSystem, memoryService);

        host.Services.AddSingleton<MemoryServiceDecorator>(memoryServiceDecorator);

        var hostingApp = host.Build();

        return hostingApp;
    }
}
