using System.Collections.ObjectModel;

namespace WPFUiDesktopApp.ViewModels;

public partial class StorageManagementViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> _storageIndexes = [];

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
    private void RemoveIndex(object parameter)
    {
        if (parameter is string indexToRemove && StorageIndexes.Contains(indexToRemove))
        {
            StorageIndexes.Remove(indexToRemove);
        }
    }
}