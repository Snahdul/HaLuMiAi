using System.Collections.ObjectModel;

namespace WPFUiDesktopApp.ViewModels.Pages;

public partial class TagManagerViewModel : ObservableObject
{
    public TagManagerViewModel()
    {
        Tags = new ObservableCollection<KeyValuePair<string, string>>();
    }

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> tags;

    [RelayCommand]
    private void AddTag(Tuple<string, string> tag)
    {
        if (!string.IsNullOrWhiteSpace(tag.Item1) && !string.IsNullOrWhiteSpace(tag.Item2))
        {
            Tags.Add(new KeyValuePair<string, string>(tag.Item1, tag.Item2));
        }
    }

    [RelayCommand]
    private void RemoveTag(string key)
    {
        var tagToRemove = Tags.FirstOrDefault(t => t.Key == key);
        if (!tagToRemove.Equals(default(KeyValuePair<string, string>)))
        {
            Tags.Remove(tagToRemove);
        }
    }

    public Dictionary<string, string> GetTagsAsDictionary()
    {
        return Tags.ToDictionary(t => t.Key, t => t.Value);
    }
}