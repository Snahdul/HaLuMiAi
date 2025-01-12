using Common.Settings;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
public class KernelMemoryServiceFactory : IKernelMemoryServiceFactory
{
    private readonly IFileSystem _fileSystem;

    private readonly ILogger<KernelMemoryServiceFactory> _logger;

    private readonly ILoggerFactory? _loggerFactory;
    private readonly IOptions<OllamaSettings> _options;

    /// <summary>
    /// Factory for creating the kernel memory service.
    /// </summary>
    /// <param name="loggerFactory">The logger factory used to create a logger for this factory.</param>
    /// <param name="fileSystem">The file system used to access the file system.</param>
    /// <param name="options">The application settings for Ollama options.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileSystem" /> or <paramref name="options" /> is <see langword="null" />.</exception>
    public KernelMemoryServiceFactory(ILoggerFactory? loggerFactory, IFileSystem fileSystem, IOptions<OllamaSettings> options)
    {
        Guard.IsNotNull(fileSystem);
        Guard.IsNotNull(options);

        _loggerFactory = loggerFactory;
        _options = options;
        _fileSystem = fileSystem;
        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<KernelMemoryServiceFactory>();
    }

    /// <summary>
    /// Creates the host with the default memory pipeline.
    /// </summary>
    /// <returns>The created host.</returns>
    public IHost CreateOllamaHostWithDefaultMemoryPipeline()
    {
        OllamaConfig ollamaConfig = OllamaConfig();

        var host = new HostApplicationBuilder();

        IKernelMemoryBuilder memoryBuilder = new KernelMemoryBuilder(host.Services)
            .WithSimpleQueuesPipeline()
            .WithOllamaTextGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithOllamaTextEmbeddingGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

        MemoryServerless memoryServerless = memoryBuilder.Build<MemoryServerless>();
        MemoryServiceDecorator memoryServiceDecorator = new(_loggerFactory, _fileSystem, memoryServerless);

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
    public IHost CreateOllamaHostWithCustomMemoryPipeline(Type[] handlers)
    {
        OllamaConfig ollamaConfig = OllamaConfig();

        // Alternative for web apps: var host = WebApplication.CreateBuilder();
        HostApplicationBuilder host = new HostApplicationBuilder();

        var memoryBuilder = new KernelMemoryBuilder(host.Services)
            .WithoutDefaultHandlers() // remove default handlers, add our custom ones below
            .WithSimpleQueuesPipeline()
            .WithOllamaTextGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithOllamaTextEmbeddingGeneration(ollamaConfig, new GPT4oTokenizer())
            .WithSimpleVectorDb(SimpleVectorDbConfig.Persistent)
            .WithSimpleFileStorage(SimpleFileStorageConfig.Persistent);

        var handlerMappings = new Dictionary<Type, string>
        {
            { typeof(TextExtractionHandler), Constants.PipelineStepsExtract },
            { typeof(TextPartitioningHandler), Constants.PipelineStepsPartition },
            { typeof(GenerateEmbeddingsHandler), Constants.PipelineStepsGenEmbeddings },
            { typeof(SaveRecordsHandler), Constants.PipelineStepsSaveRecords }
        };

        foreach (var handler in handlers)
        {
            if (handlerMappings.TryGetValue(handler, out var pipelineStep))
            {
                host.Services.AddHandlerAsHostedService(handler, pipelineStep);
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
        MemoryServiceDecorator memoryServiceDecorator = new(_loggerFactory, _fileSystem, memoryService);

        host.Services.AddSingleton<MemoryServiceDecorator>(memoryServiceDecorator);

        var hostingApp = host.Build();

        return hostingApp;
    }

    private OllamaConfig OllamaConfig()
    {
        var ollamaSettings = _options.Value;

        var ollamaConfig = new OllamaConfig
        {
            Endpoint = ollamaSettings.Endpoint,
            TextModel = new OllamaModelConfig(ollamaSettings.TextModelId),
            EmbeddingModel = new OllamaModelConfig(ollamaSettings.EmbeddingModelId)
        };
        return ollamaConfig;
    }
}
