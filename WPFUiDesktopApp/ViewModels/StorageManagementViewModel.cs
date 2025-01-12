using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using System.Collections.ObjectModel;

namespace WPFUiDesktopApp.ViewModels;

/// <summary>
/// ViewModel for managing storage indexes.
/// </summary>
public partial class StorageManagementViewModel : ObservableObject
{
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageManagementViewModel"/> class.
    /// </summary>
    /// <param name="memoryOperationExecutor">The memory operation executor.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="memoryOperationExecutor" /> is <see langword="null" />.</exception>
    public StorageManagementViewModel(IMemoryOperationExecutor memoryOperationExecutor)
    {
        Guard.IsNotNull(memoryOperationExecutor);

        _memoryOperationExecutor = memoryOperationExecutor;
    }

    /// <summary>
    /// Gets or sets the collection of storage indexes.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _storageIndexes = [];

    /// <summary>
    /// Gets or sets the selected storage index item.
    /// </summary>
    [ObservableProperty]
    private string _selectedItem = string.Empty;

    /// <summary>
    /// Adds a new index to the storage indexes collection.
    /// </summary>
    /// <param name="parameter">The new index to add.</param>
    [RelayCommand]
    private void AddIndex(object parameter)
    {
        if (parameter is string newIndex && !string.IsNullOrWhiteSpace(newIndex))
        {
            StorageIndexes.Add(newIndex);
        }
    }

    /// <summary>
    /// Removes an index from the storage indexes collection asynchronously.
    /// </summary>
    /// <param name="parameter">The index to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand]
    private async Task RemoveIndexAsync(object parameter)
    {
        if (parameter is string indexToRemove && StorageIndexes.Contains(indexToRemove))
        {
            await _memoryOperationExecutor.ExecuteMemoryOperationAsync(async memoryServiceDecorator =>
            {
                await memoryServiceDecorator.DeleteIndexAsync(indexToRemove);
                return Task.CompletedTask;
            });

            StorageIndexes.Remove(indexToRemove);
        }
    }
}
