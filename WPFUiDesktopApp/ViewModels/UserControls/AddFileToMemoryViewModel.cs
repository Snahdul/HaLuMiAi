using Common.Settings;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using HaMiAi.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.Win32;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.ViewModels.UserControls;

public partial class AddFileToMemoryViewModel : ObservableObject
{
    private readonly IOptions<OllamaSettings> _options;
    private readonly IKernelMemoryServiceFactory _kernelMemoryServiceFactory;

    [ObservableProperty]
    private Visibility _openedFilePathVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private Visibility _openedMultiplePathVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string _openedMultiplePath = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddFileToMemoryViewModel"/> class.
    /// </summary>
    /// <param name="options">The application settings for Ollama options.</param>
    /// <param name="kernelMemoryServiceFactory">The factory for creating kernel memory services.</param>
    /// <param name="tagManagerViewModel">The tag manager view model.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    public AddFileToMemoryViewModel(
        IOptions<OllamaSettings> options,
        IKernelMemoryServiceFactory kernelMemoryServiceFactory,
        TagManagerViewModel tagManagerViewModel,
        StorageManagementViewModel storageManagementViewModel)
    {
        Guard.IsNotNull(options);
        Guard.IsNotNull(kernelMemoryServiceFactory);
        Guard.IsNotNull(tagManagerViewModel);
        Guard.IsNotNull(storageManagementViewModel);

        _options = options;
        _kernelMemoryServiceFactory = kernelMemoryServiceFactory;
        TagManagerViewModel = tagManagerViewModel;
        StorageManagementViewModel = storageManagementViewModel;
    }

    public TagManagerViewModel TagManagerViewModel { get; }

    public StorageManagementViewModel StorageManagementViewModel { get; }

    /// <summary>
    /// Opens a file dialog to pick multiple files and loads them asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    public async Task PickFilesAsync()
    {
        OpenedMultiplePathVisibility = Visibility.Collapsed;

        OpenFileDialog openFileDialog =
            new()
            {
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "All files (*.*)|*.*"
            };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        if (openFileDialog.FileNames.Length == 0)
        {
            return;
        }

        var fileNames = openFileDialog.FileNames;

        OpenedMultiplePath = string.Join("\n", fileNames);
        OpenedMultiplePathVisibility = Visibility.Visible;

        await LoadFilesAsync(fileNames);
    }

    /// <summary>
    /// Loads the specified files asynchronously into the kernel memory.
    /// </summary>
    /// <param name="fileNames">An array of file names to load.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LoadFilesAsync(string[] fileNames)
    {
        var tags = TagManagerViewModel.GetTagsAsDictionary();

        TagCollection? tagsCollection = new TagCollection();

        foreach (var tag in tags)
        {
            tagsCollection.Add(tag.Key, tag.Value);
        }

        foreach (var file in fileNames)
        {
            await ExecuteMemoryOperationAsync(
                async memoryServiceDecorator =>
                    await memoryServiceDecorator.ImportDocumentAsync(
                        filePath: file,
                        index: this.StorageManagementViewModel.SelectedItem,
                        tags: tagsCollection));
        }
    }

    /// <summary>
    /// Executes a memory operation asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The memory operation to execute.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the specified type.</returns>
    private async Task<T> ExecuteMemoryOperationAsync<T>(Func<MemoryServiceDecorator, Task<T>> operation)
    {
        try
        {
            var host = _kernelMemoryServiceFactory.CreateHostWithDefaultMemoryPipeline(options: _options);
            await host.StartAsync(CancellationToken.None);

            var memoryServiceDecorator = host.Services.GetRequiredService<MemoryServiceDecorator>();
            var result = await operation(memoryServiceDecorator);

            await host.StopAsync(CancellationToken.None);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
