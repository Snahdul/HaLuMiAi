using System.Windows;

namespace ChatConversationControl.Controls;

/// <summary>
/// A custom control for handling memory-based conversation functionalities.
/// </summary>
public class MemoryConversationControl : ConversationControl
{
    static MemoryConversationControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MemoryConversationControl), new FrameworkPropertyMetadata(typeof(MemoryConversationControl)));
    }

    /// <summary>
    /// Identifies the StorageIndexes dependency property.
    /// </summary>
    public static readonly DependencyProperty StorageIndexesProperty =
        DependencyProperty.Register(nameof(StorageIndexes), typeof(object), typeof(MemoryConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the storage indexes for the conversation.
    /// </summary>
    public object StorageIndexes
    {
        get => GetValue(StorageIndexesProperty);
        set => SetValue(StorageIndexesProperty, value);
    }

    /// <summary>
    /// Identifies the SelectedStorageIndex dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedStorageIndexProperty =
        DependencyProperty.Register(nameof(SelectedStorageIndex), typeof(string), typeof(MemoryConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the selected storage index.
    /// </summary>
    public string SelectedStorageIndex
    {
        get => (string)GetValue(SelectedStorageIndexProperty);
        set => SetValue(SelectedStorageIndexProperty, value);
    }
}