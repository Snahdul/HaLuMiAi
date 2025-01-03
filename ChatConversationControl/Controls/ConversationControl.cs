using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatConversationControl.Controls;

/// <summary>
/// A custom control for handling conversation-related functionalities.
/// </summary>
public class ConversationControl : Control
{
    static ConversationControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ConversationControl), new FrameworkPropertyMetadata(typeof(ConversationControl)));
    }

    /// <summary>
    /// Identifies the ItemsSource dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the source of items for the conversation.
    /// </summary>
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Identifies the IsLoading dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ConversationControl), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether the control is in a loading state.
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Identifies the Prompt dependency property.
    /// </summary>
    public static readonly DependencyProperty PromptProperty =
        DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(ConversationControl), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the prompt text for the conversation.
    /// This property is bound to the "PromptTextBox" in the control template to capture user input.
    /// </summary>
    public string Prompt
    {
        get => (string)GetValue(PromptProperty);
        set => SetValue(PromptProperty, value);
    }

    /// <summary>
    /// Identifies the ClearConversationCommand dependency property.
    /// </summary>
    public static readonly DependencyProperty ClearConversationCommandProperty =
        DependencyProperty.Register(nameof(ClearConversationCommand), typeof(ICommand), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to clear the conversation.
    /// </summary>
    public ICommand ClearConversationCommand
    {
        get => (ICommand)GetValue(ClearConversationCommandProperty);
        set => SetValue(ClearConversationCommandProperty, value);
    }

    /// <summary>
    /// Identifies the SaveConversationCommand dependency property.
    /// </summary>
    public static readonly DependencyProperty SaveConversationCommandProperty =
        DependencyProperty.Register(nameof(SaveConversationCommand), typeof(ICommand), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to save the conversation.
    /// </summary>
    public ICommand SaveConversationCommand
    {
        get => (ICommand)GetValue(SaveConversationCommandProperty);
        set => SetValue(SaveConversationCommandProperty, value);
    }

    /// <summary>
    /// Identifies the LoadConversationCommand dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadConversationCommandProperty =
        DependencyProperty.Register(nameof(LoadConversationCommand), typeof(ICommand), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to load a conversation.
    /// </summary>
    public ICommand LoadConversationCommand
    {
        get => (ICommand)GetValue(LoadConversationCommandProperty);
        set => SetValue(LoadConversationCommandProperty, value);
    }

    /// <summary>
    /// Identifies the SendPromptCommand dependency property.
    /// </summary>
    public static readonly DependencyProperty SendPromptCommandProperty =
        DependencyProperty.Register(nameof(SendPromptCommand), typeof(ICommand), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command to send the prompt.
    /// </summary>
    public ICommand SendPromptCommand
    {
        get => (ICommand)GetValue(SendPromptCommandProperty);
        set => SetValue(SendPromptCommandProperty, value);
    }

    /// <summary>
    /// Identifies the ConversationList dependency property.
    /// </summary>
    public static readonly DependencyProperty ConversationListProperty =
        DependencyProperty.Register(nameof(ConversationList), typeof(object), typeof(ConversationControl), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the list of conversations.
    /// </summary>
    public object ConversationList
    {
        get => GetValue(ConversationListProperty);
        set => SetValue(ConversationListProperty, value);
    }
}
