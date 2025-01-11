using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatConversationControl.Controls;

/// <summary>
/// A control for managing tags, represented as key-value pairs.
/// </summary>
public class TagManagementControl : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagManagementControl"/> class.
    /// </summary>
    static TagManagementControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TagManagementControl), new FrameworkPropertyMetadata(typeof(TagManagementControl)));
    }

    /// <summary>
    /// Gets or sets the header of the control.
    /// </summary>
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(TagManagementControl), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the collection of tags.
    /// </summary>
    public ObservableCollection<KeyValuePair<string, string>> Tags
    {
        get => (ObservableCollection<KeyValuePair<string, string>>)GetValue(TagsProperty);
        set => SetValue(TagsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Tags"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TagsProperty =
        DependencyProperty.Register(nameof(Tags), typeof(ObservableCollection<KeyValuePair<string, string>>), typeof(TagManagementControl), new PropertyMetadata(new ObservableCollection<KeyValuePair<string, string>>()));

    /// <summary>
    /// Gets or sets the command to add a tag.
    /// </summary>
    public ICommand AddTagCommand
    {
        get => (ICommand)GetValue(AddTagCommandProperty);
        set => SetValue(AddTagCommandProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="AddTagCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AddTagCommandProperty =
        DependencyProperty.Register(nameof(AddTagCommand), typeof(ICommand), typeof(TagManagementControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to remove a tag.
    /// </summary>
    public ICommand RemoveTagCommand
    {
        get => (ICommand)GetValue(RemoveTagCommandProperty);
        set => SetValue(RemoveTagCommandProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RemoveTagCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RemoveTagCommandProperty =
        DependencyProperty.Register(nameof(RemoveTagCommand), typeof(ICommand), typeof(TagManagementControl), new PropertyMetadata(null));
}
