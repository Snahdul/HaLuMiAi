using Microsoft.Extensions.Hosting;

namespace HaMiAi.Contracts;

/// <summary>
/// Factory interface for creating hosts with memory pipelines.
/// </summary>
public interface IKernelMemoryServiceFactory
{
    /// <summary>
    /// Creates the host with the default memory pipeline.
    /// </summary>
    /// <returns>The created host.</returns>
    IHost CreateOllamaHostWithDefaultMemoryPipeline();

    /// <summary>
    /// Creates the host with a custom memory pipeline.
    /// </summary>
    /// <param name="handlers">The handlers to register in the pipeline.</param>
    /// <returns>The created host.</returns>
    IHost CreateOllamaHostWithCustomMemoryPipeline(Type[] handlers);
}