using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using System.Collections.ObjectModel;

namespace WPFUiDesktopApp.ViewModels;

public partial class StorageManagementViewModel : ObservableObject
{
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;

    public StorageManagementViewModel(IMemoryOperationExecutor memoryOperationExecutor)
    {
        Guard.IsNotNull(memoryOperationExecutor);

        _memoryOperationExecutor = memoryOperationExecutor;
    }

    [ObservableProperty]
    private ObservableCollection<string> _storageIndexes = new();

    /// <summary>
    /// The selected storage index item.
    /// </summary>
    [ObservableProperty]
    private string _selectedItem = string.Empty;

    [RelayCommand]
    private void AddIndex(object parameter)
    {
        if (parameter is string newIndex && !string.IsNullOrWhiteSpace(newIndex))
        {
            StorageIndexes.Add(newIndex);
        }
    }

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