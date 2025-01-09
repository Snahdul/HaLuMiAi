using Common.Settings;
using HaMiAi.Contracts;
using HaMiAi.Implementation;
using HaMiAi.Implementation.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;

namespace HaMiAi.Test
{
    public class KernelMemoryTests
    {
        [Fact]
        public async Task AskAsyncTest()
        {
            // Create an instance of OllamaSettings
            var ollamaSettings = new OllamaSettings
            {
                Endpoint = "http://localhost:11434",
                TextModelId = "llama3.2",
                EmbeddingModelId = "nomic-embed-text"
            };

            // Create an IOptions<OllamaSettings> instance
            IOptions<OllamaSettings> options = Options.Create(ollamaSettings);

            IQueryKernelMemoryService queryKernelMemoryService = new QueryKernelMemoryService(options);

            var queryResponse = await queryKernelMemoryService.AskAsync("How to maintain robot X50 SW?", "bh");

            Assert.NotNull(queryResponse);
        }


        [Fact]
        public async Task ListIndexesAsyncTest()
        {
            // Create an instance of OllamaSettings
            var ollamaSettings = new OllamaSettings
            {
                Endpoint = "http://localhost:11434",
                TextModelId = "llama3.2",
                EmbeddingModelId = "nomic-embed-text"
            };

            // Create an IOptions<OllamaSettings> instance
            IOptions<OllamaSettings> options = Options.Create(ollamaSettings);

            IQueryKernelMemoryService queryKernelMemoryService = new QueryKernelMemoryService(options);

            var memoryIndexesAsync = await queryKernelMemoryService.ListIndexesAsync();

            Assert.True(memoryIndexesAsync.Any());
        }

        [Fact]
        public async Task MemoryServiceTest()
        {
            KernelMemoryServiceFactory kernelMemoryServiceFactory = new(new NullLoggerFactory());

            const string memoryIndex = "Unittest";

            // Create an instance of OllamaSettings
            var ollamaSettings = new OllamaSettings
            {
                Endpoint = "http://localhost:11434",
                TextModelId = "llama3.2",
                EmbeddingModelId = "nomic-embed-text"
            };

            // Create an IOptions<OllamaSettings> instance
            IOptions<OllamaSettings> options = Options.Create(ollamaSettings);

            var host = kernelMemoryServiceFactory.CreateHostWithDefaultMemoryPipeline(options);

            await host.StartAsync(CancellationToken.None);

            var memoryServiceDecorator = host.Services.GetRequiredService<MemoryServiceDecorator>();

            var docId = await memoryServiceDecorator.ImportDocumentAsync(
                @"ImportDocuments\What is AI.txt",
            index: memoryIndex);

            // Wait for import to complete
            var status = await memoryServiceDecorator.GetDocumentStatusAsync(documentId: docId);
            while (status is { Completed: false })
            {
                Console.WriteLine("* Work in progress...");
                Console.WriteLine("Steps:     " + string.Join(", ", status.Steps));
                Console.WriteLine("Completed: " + string.Join(", ", status.CompletedSteps));
                Console.WriteLine("Remaining: " + string.Join(", ", status.RemainingSteps));
                Console.WriteLine();
                await Task.Delay(TimeSpan.FromSeconds(1));
                status = await memoryServiceDecorator.GetDocumentStatusAsync(documentId: docId);
            }

            var indexes = await memoryServiceDecorator.ListIndexesAsync(CancellationToken.None);

            var searchResult = await memoryServiceDecorator.AskAsync(
                "What is Artificial Intelligence?",
                index: memoryIndex,
                minRelevance: .02);

            Assert.False(searchResult.NoResult);

            await host.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ImportWithStepsTest()
        {
            KernelMemoryServiceFactory kernelMemoryServiceFactory = new(new NullLoggerFactory());

            const string memoryIndex = "Unittest";

            // Create an instance of OllamaSettings
            var ollamaSettings = new OllamaSettings
            {
                Endpoint = "http://localhost:11434",
                TextModelId = "llama3.2",
                EmbeddingModelId = "nomic-embed-text"
            };

            // Create an IOptions<OllamaSettings> instance
            IOptions<OllamaSettings> options = Options.Create(ollamaSettings);

            Type[] handlers =
            {
                typeof(TextExtractionHandler),
                typeof(TextPartitioningHandler),
                typeof(GenerateEmbeddingsHandler),
                typeof(SaveRecordsHandler),
                typeof(HaMiSummarizationHandler)
            };

            var host = kernelMemoryServiceFactory.CreateHostWithCustomMemoryPipeline(options, handlers);

            await host.StartAsync(CancellationToken.None);

            var memoryServiceDecorator = host.Services.GetRequiredService<MemoryServiceDecorator>();

            var docId = await memoryServiceDecorator.ImportDocumentAsync(
                new Document("WhatIsAI")
                    .AddFile(@"ImportDocuments\What is AI.txt"),
                steps:
                [
                    "extract_text",
                    Constants.PipelineStepsPartition,
                    Constants.PipelineStepsGenEmbeddings,
                    Constants.PipelineStepsSaveRecords,
                    Constants.PipelineStepsSummarize
                ],
                index: memoryIndex);

            // Wait for import to complete
            var status = await memoryServiceDecorator.GetDocumentStatusAsync(documentId: docId);
            while (status is { Completed: false })
            {
                Console.WriteLine("* Work in progress...");
                Console.WriteLine("Steps:     " + string.Join(", ", status.Steps));
                Console.WriteLine("Completed: " + string.Join(", ", status.CompletedSteps));
                Console.WriteLine("Remaining: " + string.Join(", ", status.RemainingSteps));
                Console.WriteLine();
                await Task.Delay(TimeSpan.FromSeconds(1));
                status = await memoryServiceDecorator.GetDocumentStatusAsync(documentId: docId);
            }

            var indexes = await memoryServiceDecorator.ListIndexesAsync(CancellationToken.None);

            var searchResult = await memoryServiceDecorator.AskAsync("What is Artificial Intelligence?", index: memoryIndex,
                minRelevance: .02);

            Assert.False(searchResult.NoResult);

            await host.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CreateHostTest()
        {
            var storeIndex = "cv";

            // Create an instance of OllamaSettings
            var ollamaSettings = new OllamaSettings
            {
                Endpoint = "http://localhost:11434",
                TextModelId = "llama3.2",
                EmbeddingModelId = "mxbai-embed-large"
            };

            // Create an IOptions<OllamaSettings> instance
            IOptions<OllamaSettings> options = Options.Create(ollamaSettings);

            IImportDocumentKernelMemory importDocumentKernelMemory =
                new ImportDocumentKernelMemory(
                    new NullLoggerFactory(),
                    new KernelMemoryServiceFactory(new NullLoggerFactory()),
                    options);

            var docId = await importDocumentKernelMemory.ImportDocumentAsync(@"ImportDocuments\What is AI.txt", storeIndex: storeIndex, new Dictionary<string, string>());



            Assert.NotNull(docId);

            IQueryKernelMemoryService queryKernelMemoryService = new QueryKernelMemoryService(options);
            var response = await queryKernelMemoryService.AskAsync("What is AI?", storeIndex);

            Assert.NotNull(response);
        }
    }
}
