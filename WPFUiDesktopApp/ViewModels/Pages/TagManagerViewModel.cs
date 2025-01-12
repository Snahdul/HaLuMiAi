using System.Collections.ObjectModel;
using WPFUiDesktopApp.Services;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// ViewModel for managing tags in the application.
/// </summary>
public partial class TagManagerViewModel : ObservableObject
{
    private readonly ITagService _tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagManagerViewModel"/> class.
    /// </summary>
    public TagManagerViewModel(ITagService tagService)
    {
        _tagService = tagService;
        Tags = _tagService.Tags;
    }

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _tags;

    /// <summary>
    /// Adds a new tag to the collection.
    /// </summary>
    /// <param name="tag">A tuple containing the key and value of the tag.</param>
    [RelayCommand]
    private void AddTag(Tuple<string, string> tag) =>
        _tagService.AddTag(tag);

    /// <summary>
    /// Removes a tag from the collection by its key.
    /// </summary>
    /// <param name="key">The key of the tag to remove.</param>
    [RelayCommand]
    private void RemoveTag(string key) =>
        _tagService.RemoveTag(key);

    /// <summary>
    /// Gets the tags as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the tags.</returns>
    public Dictionary<string, string> GetTagsAsDictionary() =>
        _tagService.GetTagsAsDictionary();
}

