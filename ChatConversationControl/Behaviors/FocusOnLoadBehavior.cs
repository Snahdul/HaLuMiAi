using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace ChatConversationControl.Behaviors;

public class FocusOnLoadBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AssociatedObject.Focus();
    }
}