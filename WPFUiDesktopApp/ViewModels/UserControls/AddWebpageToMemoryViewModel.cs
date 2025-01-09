using Common.Settings;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using HaMiAi.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.ViewModels.UserControls;

public partial class AddWebpageToMemoryViewModel : ObservableObject
{
    private readonly IOptions<OllamaSettings> _options;
    private readonly IKernelMemoryServiceFactory _kernelMemoryServiceFactory;

    [ObservableProperty]
    private string _webpageUrl = string.Empty;

    [ObservableProperty]
    private Dictionary<string, string> _tags = new();

    [ObservableProperty] private string _storeIndex = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddWebpageToMemoryViewModel"/> class.
    /// </summary>
    /// <param name="kernelMemoryServiceFactory"></param>
    /// <param name="tagManagerViewModel">The tag manager view model.</param>
    /// <param name="options"></param>
    public AddWebpageToMemoryViewModel(
        IOptions<OllamaSettings> options,
        IKernelMemoryServiceFactory kernelMemoryServiceFactory,
        TagManagerViewModel tagManagerViewModel)
    {
        _options = options;
        _kernelMemoryServiceFactory = kernelMemoryServiceFactory;
        Guard.IsNotNull(options);
        Guard.IsNotNull(kernelMemoryServiceFactory);
        Guard.IsNotNull(tagManagerViewModel);

        TagManagerViewModel = tagManagerViewModel;
    }

    /// <summary>
    /// Gets the tag manager view model.
    /// </summary>
    public TagManagerViewModel TagManagerViewModel { get; }

    /// <summary>
    /// Adds the webpage to memory.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    public async Task AddWebpageToMemoryAsync()
    {
        if (string.IsNullOrWhiteSpace(WebpageUrl))
        {
            return;
        }

        await ExecuteMemoryOperationAsync(async memoryServiceDecorator => await memoryServiceDecorator.ImportWebPageAsync(WebpageUrl, index: StoreIndex));
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