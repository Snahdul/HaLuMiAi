using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.ViewModels.UserControls;

public partial class AddWebpageToMemoryViewModel : ObservableObject
{
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;

    [ObservableProperty]
    private string _webpageUrl = string.Empty;

    [ObservableProperty]
    private Dictionary<string, string> _tags = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AddWebpageToMemoryViewModel"/> class.
    /// </summary>
    /// <param name="memoryOperationExecutor"></param>
    /// <param name="tagManagerViewModel">The tag manager view model.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    public AddWebpageToMemoryViewModel(
        IMemoryOperationExecutor memoryOperationExecutor,
        TagManagerViewModel tagManagerViewModel,
        StorageManagementViewModel storageManagementViewModel)
    {
        _memoryOperationExecutor = memoryOperationExecutor;
        Guard.IsNotNull(tagManagerViewModel);

        TagManagerViewModel = tagManagerViewModel;
        StorageManagementViewModel = storageManagementViewModel;
    }

    /// <summary>
    /// Gets the tag manager view model.
    /// </summary>
    public TagManagerViewModel TagManagerViewModel { get; }

    /// <summary>
    /// Gets the storage management view model.
    /// </summary>
    public StorageManagementViewModel StorageManagementViewModel { get; }

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

        await _memoryOperationExecutor.ExecuteMemoryOperationAsync(async memoryServiceDecorator =>
            await memoryServiceDecorator.ImportWebPageAsync(WebpageUrl, index: this.StorageManagementViewModel.SelectedItem));
    }
}