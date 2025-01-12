using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;

namespace HaMiAi.Implementation;

/// <summary>
/// Executes memory operations using a kernel memory service factory.
/// </summary>
public class MemoryOperationExecutor : IMemoryOperationExecutor
{
    private readonly IKernelMemoryServiceFactory _kernelMemoryServiceFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryOperationExecutor"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="kernelMemoryServiceFactory">The kernel memory service factory.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="kernelMemoryServiceFactory" /> is <see langword="null" />.</exception>
    public MemoryOperationExecutor(ILoggerFactory? loggerFactory, IKernelMemoryServiceFactory kernelMemoryServiceFactory)
    {
        Guard.IsNotNull(kernelMemoryServiceFactory);

        _logger = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<MemoryOperationExecutor>();

        _kernelMemoryServiceFactory = kernelMemoryServiceFactory;
    }

    /// <summary>
    /// Executes a memory operation asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The memory operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the specified type.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="T" />.</exception>
    public async Task<T> ExecuteMemoryOperationAsync<T>(Func<MemoryServiceDecorator, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        using var host = _kernelMemoryServiceFactory.CreateOllamaHostWithDefaultMemoryPipeline();
        await host.StartAsync(CancellationToken.None);

        try
        {
            _logger.LogInformation("Starting memory operation.");

            var memoryServiceDecorator = host.Services.GetRequiredService<MemoryServiceDecorator>();
            var result = await operation(memoryServiceDecorator);

            _logger.LogInformation("Memory operation completed successfully.");

            return result;
        }
        finally
        {
            await host.StopAsync(CancellationToken.None);
        }
    }
}