using Common.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HaMiAi.Contracts
{
    public interface IKernelMemoryServiceFactory
    {
        /// <summary>
        /// Creates the host with the default memory pipeline.
        /// </summary>
        /// <param name="options">The application settings for Ollama options.</param>
        /// <returns>The created host.</returns>
        IHost CreateHostWithDefaultMemoryPipeline(IOptions<OllamaSettings> options);

        /// <summary>
        /// Creates the host with a custom memory pipeline.
        /// </summary>
        /// <param name="options">The application settings for Ollama options.</param>
        /// <param name="handlers">The handlers to register in the pipeline.</param>
        /// <returns>The created host.</returns>
        IHost CreateHostWithCustomMemoryPipeline(IOptions<OllamaSettings> options,
            Type[] handlers);
    }
}