using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatConversationControl.Controls;

public class StorageManagementControl : Control
{
    static StorageManagementControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(StorageManagementControl), new FrameworkPropertyMetadata(typeof(StorageManagementControl)));
    }

    /// <summary>
    /// Gets or sets the collection of indexes.
    /// </summary>
    public ObservableCollection<string> StorageIndexes
    {
        get => (ObservableCollection<string>)GetValue(StorageIndexesProperty);
        set => SetValue(StorageIndexesProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StorageIndexes"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StorageIndexesProperty =
        DependencyProperty.Register(nameof(StorageIndexes), typeof(ObservableCollection<string>), typeof(StorageManagementControl), new PropertyMetadata(new ObservableCollection<string>()));

    /// <summary>
    /// Gets or sets the command to add an index.
    /// </summary>
    public ICommand AddIndexCommand
    {
        get => (ICommand)GetValue(AddIndexCommandProperty);
        set => SetValue(AddIndexCommandProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="AddIndexCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AddIndexCommandProperty =
        DependencyProperty.Register(nameof(AddIndexCommand), typeof(ICommand), typeof(StorageManagementControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to remove an index.
    /// </summary>
    public ICommand RemoveIndexCommand
    {
        get => (ICommand)GetValue(RemoveIndexCommandProperty);
        set => SetValue(RemoveIndexCommandProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RemoveIndexCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RemoveIndexCommandProperty =
        DependencyProperty.Register(nameof(RemoveIndexCommand), typeof(ICommand), typeof(StorageManagementControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the name of the new element.
    /// </summary>
    public string NewElementName
    {
        get => (string)GetValue(NewElementNameProperty);
        set => SetValue(NewElementNameProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="NewElementName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NewElementNameProperty =
        DependencyProperty.Register(nameof(NewElementName), typeof(string), typeof(StorageManagementControl), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the selected index.
    /// </summary>
    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(string), typeof(StorageManagementControl), new PropertyMetadata(string.Empty));
}
