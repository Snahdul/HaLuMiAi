using System.Collections.ObjectModel;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// Interface for managing a collection of tags.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    ObservableCollection<KeyValuePair<string, string>> Tags { get; }

    /// <summary>
    /// Adds a new tag to the collection.
    /// </summary>
    /// <param name="tag">A tuple containing the key and value of the tag.</param>
    void AddTag(Tuple<string, string> tag);

    /// <summary>
    /// Removes a tag from the collection by its key.
    /// </summary>
    /// <param name="key">The key of the tag to remove.</param>
    void RemoveTag(string key);

    /// <summary>
    /// Gets the tags as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the tags.</returns>
    Dictionary<string, string> GetTagsAsDictionary();
}