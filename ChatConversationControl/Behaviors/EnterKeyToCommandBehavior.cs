using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatConversationControl.Behaviors;

/// <summary>
/// A behavior that binds the Enter key to a command for a TextBox.
/// </summary>
public class EnterKeyToCommandBehavior : Behavior<TextBox>
{
    /// <summary>
    /// Identifies the Command dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EnterKeyToCommandBehavior), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to execute when the Enter key is pressed.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the AssociatedObject.
    /// Executes the command if the Enter key is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The KeyEventArgs instance containing the event data.</param>
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // Allow the TextBox to handle Shift + Enter to add a new line
                return;
            }

            if (Command != null && Command.CanExecute(AssociatedObject.Text))
            {
                // Set the Text property before executing the command
                Command.Execute(AssociatedObject.Text);
                e.Handled = true; // Prevent the TextBox from handling the Enter key
            }
        }
    }
}