using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace ChatConversationControl.Behaviors;

/// <summary>
/// A behavior that sets focus to a TextBox when it is loaded.
/// </summary>
public class FocusOnLoadBehavior : Behavior<TextBox>
{
    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    /// <summary>
    /// Handles the Loaded event of the AssociatedObject.
    /// Sets focus to the TextBox.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The RoutedEventArgs instance containing the event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AssociatedObject.Focus();
    }
}