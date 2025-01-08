using Microsoft.KernelMemory.AI.Ollama;

namespace OllamaKernelMemory.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            OllamaKernelMemoryQueryService ollamaKernelMemoryQueryService = new OllamaKernelMemoryQueryService();

            var ollamaConfig = new OllamaConfig
            {
                Endpoint = "http://localhost:11434",
                TextModel = new OllamaModelConfig("llama3.2", 131072),
                EmbeddingModel = new OllamaModelConfig("nomic-embed-text", 2048)
            };

            var prompt = "prompt";

            var storageIndex = "storageIndex";

            try
            {
                var result = ollamaKernelMemoryQueryService.CreateHost(ollamaConfig);
                Assert.NotNull(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
