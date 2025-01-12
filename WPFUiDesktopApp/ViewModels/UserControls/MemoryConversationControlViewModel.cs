using ChatConversationControl.Contracts;
using ChatConversationControl.ViewModels;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using Microsoft.Extensions.AI;
using System.Text;
using Wpf.Ui.Controls;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// ViewModel for handling stream conversation control with memory.
/// </summary>
public partial class MemoryConversationControlViewModel : BaseConversationControlViewModel, INavigationAware
{
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _loadIndexesTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="memoryOperationExecutor">The executor for memory operations.</param>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> is <see langword="null" />.
    /// </exception>
    public MemoryConversationControlViewModel(
        IMemoryOperationExecutor memoryOperationExecutor,
        IConversationManager conversationManager,
        IChatClient chatClient,
        StorageManagementViewModel storageManagementViewModel) : base(conversationManager, chatClient)
    {
        Guard.IsNotNull(memoryOperationExecutor);
        Guard.IsNotNull(conversationManager);
        Guard.IsNotNull(chatClient);
        Guard.IsNotNull(storageManagementViewModel);

        StorageManagementViewModel = storageManagementViewModel;
        _memoryOperationExecutor = memoryOperationExecutor;
    }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        // Initialize the CancellationTokenSource
        _cancellationTokenSource = new CancellationTokenSource();

        // Start the asynchronous operation without awaiting it
        _loadIndexesTask = LoadIndexesAsync(_cancellationTokenSource.Token);
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
    }

    #endregion

    #region Overrides of BaseConversationControlViewModel

    /// <summary>
    /// Streams the chat asynchronously.
    /// </summary>
    /// <param name="prompt">The prompt to send.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <inheritdoc />
    protected override async Task DoChatStreamAsync(object? prompt, CancellationToken cancellationToken)
    {
        if (prompt is not string promptText || string.IsNullOrWhiteSpace(promptText))
        {
            return;
        }

        IsLoading = true;

        try
        {
            var memoryAnswer = await _memoryOperationExecutor.ExecuteMemoryOperationAsync(async memoryServiceDecorator =>
                await memoryServiceDecorator.AskAsync(
                    promptText,
                    StorageManagementViewModel.SelectedItem,
                    cancellationToken: cancellationToken), cancellationToken);

            if (memoryAnswer.NoResult || string.IsNullOrEmpty(memoryAnswer.Result))
            {
                return;
            }

            StringBuilder stringBuilder = new(memoryAnswer.Result);
            stringBuilder.Append(promptText);

            IsLoading = false;

            await base.DoChatStreamAsync(stringBuilder.ToString(), cancellationToken);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // TODO: Handle the System.ArgumentOutOfRangeException
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    public StorageManagementViewModel StorageManagementViewModel { get; }

    private async Task LoadIndexesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var indexes = await _memoryOperationExecutor.ExecuteMemoryOperationAsync(async memoryServiceDecorator =>
                await memoryServiceDecorator.ListIndexesAsync(cancellationToken).ConfigureAwait(false), cancellationToken);

            // Update UI elements on the UI thread
#pragma warning disable VSTHRD001
            await Application.Current.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001
            {
                foreach (var indexDetail in indexes)
                {
                    StorageManagementViewModel.StorageIndexes.Add(indexDetail.Name);
                }

                if (StorageManagementViewModel.StorageIndexes.Any())
                {
                    StorageManagementViewModel.SelectedItem = StorageManagementViewModel.StorageIndexes.First();
                }
            });
        }
        catch (OperationCanceledException)
        {
            // Handle task cancellation if needed
        }
    }
}