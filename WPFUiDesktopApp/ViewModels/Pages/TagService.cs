using System.Collections.ObjectModel;
using WPFUiDesktopApp.Services;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>Service for managing a collection of tags.</summary>
public class TagService : ITagService
{
    public ObservableCollection<KeyValuePair<string, string>> Tags { get; } = [];

    /// <summary>
    /// Adds a new tag to the collection.
    /// </summary>
    /// <param name="tag">A tuple containing the key and value of the tag.</param>
    public void AddTag(Tuple<string, string> tag)
    {
        if (!string.IsNullOrWhiteSpace(tag.Item1) && !string.IsNullOrWhiteSpace(tag.Item2))
        {
            Tags.Add(new KeyValuePair<string, string>(tag.Item1, tag.Item2));
        }
    }

    /// <summary>
    /// Removes a tag from the collection by its key.
    /// </summary>
    /// <param name="key">The key of the tag to remove.</param>
    public void RemoveTag(string key)
    {
        var tagToRemove = Tags.FirstOrDefault(t => t.Key == key);
        if (!tagToRemove.Equals(default(KeyValuePair<string, string>)))
        {
            Tags.Remove(tagToRemove);
        }
    }

    /// <summary>
    /// Gets the tags as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the tags.</returns>
    public Dictionary<string, string> GetTagsAsDictionary()
    {
        return Tags.ToDictionary(t => t.Key, t => t.Value);
    }
}