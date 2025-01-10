using HaMiAi.Implementation;

namespace HaMiAi.Contracts;

public interface IMemoryOperationExecutor
{
    /// <summary>
    /// Executes a memory operation asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The memory operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the specified type.</returns>
    Task<T> ExecuteMemoryOperationAsync<T>(Func<MemoryServiceDecorator, Task<T>> operation, CancellationToken cancellationToken = default);
}