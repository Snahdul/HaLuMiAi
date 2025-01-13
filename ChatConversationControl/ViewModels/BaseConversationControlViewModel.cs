using ChatConversationControl.Contracts;
using ChatConversationControl.Extensions;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
using AuthorRole = Microsoft.SemanticKernel.ChatCompletion.AuthorRole;
using ChatHistory = Microsoft.SemanticKernel.ChatCompletion.ChatHistory;

namespace ChatConversationControl.ViewModels;

/// <summary>
/// Base view model for conversation control.
/// </summary>
public abstract partial class BaseConversationControlViewModel : ObservableObject
{
    protected readonly ChatHistory ChatHistory = new();
    protected readonly IConversationManager ConversationManager;
    protected readonly IChatClient ChatClient;

    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Indicates whether the control is in a loading state.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Indicates whether the control should use history.
    /// </summary>
    [ObservableProperty]
    private bool _useHistory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> is <see langword="null" />.</exception>
    [Experimental("SKEXP0001")]
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
        SendPromptAsyncCommand = new AsyncRelayCommand<object>(prompt => DoChatAsync(prompt, _cancellationTokenSource?.Token ?? CancellationToken.None));
        SendPromptStreamAsyncCommand = new AsyncRelayCommand<object>(prompt => DoChatStreamAsync(prompt, _cancellationTokenSource?.Token ?? CancellationToken.None));
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        _cancellationTokenSource = new CancellationTokenSource();
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
    /// Gets the command to cancel the current operation.
    /// </summary>
    public IAsyncRelayCommand CancelCommand { get; }

    /// <summary>
    /// Gets the list of conversation messages.
    /// </summary>
    public ObservableCollection<Messages.MessageItem> ConversationList => ConversationManager.ConversationList;

    /// <summary>
    /// Clears the conversation.
    /// </summary>
    private Task ClearConversationAsync()
    {
        // Clear the conversation list
        ConversationList.Clear();
        ChatHistory.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cancels the current operation.
    /// </summary>
    private async Task CancelAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        await Task.Delay(0); // Ensure the cancellation is processed asynchronously
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Sends a chat prompt and processes the response.
    /// </summary>
    /// <param name="prompt">The chat prompt.</param>
    /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
    protected virtual async Task DoChatAsync(object? prompt, CancellationToken cancellationToken)
    {
        if (prompt is not string promptString || string.IsNullOrWhiteSpace(promptString)) return;

        var responseMessageItem = new Messages.MessageItem
        {
            ColorString = "LightBlue"
        };

        IsLoading = true;

        ChatHistory.Add(new ChatMessageContent(AuthorRole.User, promptString));

        try
        {
            // Add the response message item to the conversation list
            ConversationList.Add(responseMessageItem);

            // Send the prompt to the chat client and get the response
            var response = await ChatClient.CompleteAsync(promptString, cancellationToken: cancellationToken);
            var responseText = response?.Message.Text ?? "Response could not be generated!";

            // Append the response text to the message item
            responseMessageItem.AppendLineText($"{promptString}{Environment.NewLine}Model-Response: {responseText}");

            if (UseHistory)
            {
                ChatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, responseText));
            }
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation exception
            Debug.WriteLine("Operation was canceled.");
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
    /// <param name="cancellationToken">The cancellation token to use for the operation.</param>
    [Experimental("SKEXP0001")]
    protected virtual async Task DoChatStreamAsync(object? prompt, CancellationToken cancellationToken)
    {
        if (prompt is not string promptString || string.IsNullOrWhiteSpace(promptString))
            return;

        var responseMessageItem = new Messages.MessageItem
        {
            ColorString = "LightBlue"
        };

        IsLoading = true;

        ChatHistory.Add(new ChatMessageContent(AuthorRole.User, promptString));

        try
        {
            // Switch to the UI thread to add the response message item to the conversation list
#pragma warning disable VSTHRD001
            // Add the response message item to the conversation list on the UI thread
            // ReSharper disable once ExceptionNotDocumented
            await Application.Current.Dispatcher.InvokeAsync(() => ConversationList.Add(responseMessageItem), DispatcherPriority.Normal, cancellationToken);
#pragma warning restore VSTHRD001

            // Concatenate all messages from ChatHistory
            var fullPrompt = ChatHistory.GetFullPrompt();

            // Stream the response from the chat client
            await foreach (var part in ChatClient.CompleteStreamingAsync(fullPrompt, null, cancellationToken).ConfigureAwait(false))
            {
                if (part.Text == null)
                {
                    continue;
                }

                Debug.WriteLine(part.Text);

                // Update the UI-bound property on the UI thread
#pragma warning disable VSTHRD001
                await Application.Current.Dispatcher.InvokeAsync(() => responseMessageItem.AppendText(part.Text), DispatcherPriority.Normal, cancellationToken);
#pragma warning restore VSTHRD001
            }

            if (UseHistory)
            {
                ChatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, responseMessageItem.Text));
            }
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation exception
            Debug.WriteLine("Debug-Out: Operation was canceled.");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
