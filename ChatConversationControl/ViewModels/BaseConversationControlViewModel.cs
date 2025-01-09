using ChatConversationControl.Contracts;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace ChatConversationControl.ViewModels;

/// <summary>
/// Base view model for conversation control.
/// </summary>
public abstract partial class BaseConversationControlViewModel : ObservableObject
{
    protected readonly IConversationManager ConversationManager;
    protected readonly IChatClient ChatClient;

    /// <summary>
    /// Indicates whether the control is in a loading state.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> is <see langword="null" />.</exception>
    protected BaseConversationControlViewModel(IConversationManager conversationManager, IChatClient chatClient)
    {
        Guard.IsNotNull(conversationManager);
        Guard.IsNotNull(chatClient);

        ConversationManager = conversationManager;
        ChatClient = chatClient;

        // Initialize commands
        SaveConversationAsyncCommand = new AsyncRelayCommand(ConversationManager.SaveConversationAsync);
        ClearConversationAsyncCommand = new AsyncRelayCommand(ClearConversationAsync);
        LoadConversationAsyncCommand = new AsyncRelayCommand(ConversationManager.LoadConversationAsync);
        SendPromptAsyncCommand = new AsyncRelayCommand<object>(DoChatAsync);
        SendPromptStreamAsyncCommand = new AsyncRelayCommand<object>(DoChatStreamAsync);
    }

    /// <summary>
    /// Gets the command to save the conversation.
    /// </summary>
    public IAsyncRelayCommand SaveConversationAsyncCommand { get; }

    /// <summary>
    /// Gets the command to clear the conversation.
    /// </summary>
    public IAsyncRelayCommand ClearConversationAsyncCommand { get; }

    /// <summary>
    /// Gets the command to load a conversation.
    /// </summary>
    public IAsyncRelayCommand LoadConversationAsyncCommand { get; }

    /// <summary>
    /// Gets the command to send a prompt.
    /// </summary>
    public IAsyncRelayCommand<object> SendPromptAsyncCommand { get; }

    /// <summary>
    /// Gets the command to send a prompt with streaming response.
    /// </summary>
    public IAsyncRelayCommand<object> SendPromptStreamAsyncCommand { get; }

    /// <summary>
    /// Gets the list of conversation messages.
    /// </summary>
    public ObservableCollection<Messages.MessageItem> ConversationList => ConversationManager.ConversationList;

    /// <summary>
    /// Clears the conversation.
    /// </summary>
    private async Task ClearConversationAsync()
    {
        // Clear the conversation list
        ConversationList.Clear();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Sends a chat prompt and processes the response.
    /// </summary>
    /// <param name="prompt">The chat prompt.</param>
    protected virtual async Task DoChatAsync(object? prompt)
    {
        if (prompt is not string promptString || string.IsNullOrWhiteSpace(promptString)) return;

        var responseMessageItem = new Messages.MessageItem
        {
            ColorString = "LightBlue"
        };

        try
        {
            IsLoading = true;

            // Add the response message item to the conversation list
            ConversationList.Add(responseMessageItem);

            // Send the prompt to the chat client and get the response
            var response = await ChatClient.CompleteAsync(promptString);
            var responseText = response?.Message.Text ?? "Response could not be generated!";

            // Append the response text to the message item
            responseMessageItem.AppendLineText($"{promptString}{Environment.NewLine}Model-Response: {responseText}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Sends a chat prompt and processes the streaming response.
    /// </summary>
    /// <param name="prompt">The chat prompt.</param>
    protected virtual async Task DoChatStreamAsync(object? prompt)
    {
        if (prompt is not string promptString || string.IsNullOrWhiteSpace(promptString))
            return;

        var responseMessageItem = new Messages.MessageItem
        {
            ColorString = "LightBlue"
        };

        try
        {
            IsLoading = true;

            // Add the response message item to the conversation list on the UI thread
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ConversationList.Add(responseMessageItem);
            });

            // Stream the response from the chat client
            await foreach (var part in ChatClient.CompleteStreamingAsync(promptString).ConfigureAwait(false))
            {
                if (part.Text != null)
                {
                    Debug.WriteLine(part.Text);

                    // Update the UI-bound property on the UI thread
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        responseMessageItem.AppendText(part.Text);
                    }));
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
