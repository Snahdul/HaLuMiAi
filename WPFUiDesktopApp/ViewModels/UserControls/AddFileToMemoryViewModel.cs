using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using Microsoft.KernelMemory;
using Microsoft.Win32;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// ViewModel for adding files to memory.
/// </summary>
public partial class AddFileToMemoryViewModel : ObservableObject
{
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;

    [ObservableProperty]
    private Visibility _openedFilePathVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private Visibility _openedMultiplePathVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string _openedMultiplePath = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddFileToMemoryViewModel"/> class.
    /// </summary>
    /// <param name="memoryOperationExecutor">The executor for memory operations.</param>
    /// <param name="tagManagerViewModel">The tag manager view model.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tagManagerViewModel" /> or <paramref name="storageManagementViewModel" /> is <see langword="null" />.</exception>
    public AddFileToMemoryViewModel(
        IMemoryOperationExecutor memoryOperationExecutor,
        TagManagerViewModel tagManagerViewModel,
        StorageManagementViewModel storageManagementViewModel)
    {
        Guard.IsNotNull(tagManagerViewModel);
        Guard.IsNotNull(storageManagementViewModel);

        _memoryOperationExecutor = memoryOperationExecutor;
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
            await _memoryOperationExecutor.ExecuteMemoryOperationAsync(
                async memoryServiceDecorator =>
                    await memoryServiceDecorator.ImportDocumentAsync(
                        filePath: file,
                        index: this.StorageManagementViewModel.SelectedItem,
                        tags: tagsCollection));
        }
    }
}
