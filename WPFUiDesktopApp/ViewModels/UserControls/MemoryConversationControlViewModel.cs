using ChatConversationControl.Contracts;
using ChatConversationControl.ViewModels;
using CommunityToolkit.Diagnostics;
using HaMiAi.Contracts;
using Microsoft.Extensions.AI;
using Microsoft.KernelMemory;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// ViewModel for handling stream conversation control with memory.
/// </summary>
public partial class MemoryConversationControlViewModel : BaseConversationControlViewModel, INavigationAware
{
    private readonly IFileSystem _fileSystem;
    private readonly IMemoryOperationExecutor _memoryOperationExecutor;
    private readonly IProcessManager _processManager;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _loadIndexesTask;

    /// <summary>
    /// Gets or sets the minimum relevance.
    /// </summary>
    [ObservableProperty] private double _minRelevance = .6;

    /// <summary>
    /// Gets or sets the relevant sources.
    /// </summary>
    [ObservableProperty] private ObservableCollection<Citation> _relevantSources = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="fileSystem">File system used to access the file system.</param>
    /// <param name="memoryOperationExecutor">The executor for memory operations.</param>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="processManager">The process manager.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> is <see langword="null" />.
    /// </exception>
    public MemoryConversationControlViewModel(
        IFileSystem fileSystem,
        IMemoryOperationExecutor memoryOperationExecutor,
        IConversationManager conversationManager,
        IChatClient chatClient,
        IProcessManager processManager,
        StorageManagementViewModel storageManagementViewModel) : base(conversationManager, chatClient)
    {
        Guard.IsNotNull(memoryOperationExecutor);
        Guard.IsNotNull(conversationManager);
        Guard.IsNotNull(chatClient);
        Guard.IsNotNull(storageManagementViewModel);

        StorageManagementViewModel = storageManagementViewModel;
        _fileSystem = fileSystem;
        _memoryOperationExecutor = memoryOperationExecutor;
        _processManager = processManager;
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
        // Cancel the ongoing task if it is running
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        // Optionally, you can wait for the task to complete if needed
        if (_loadIndexesTask == null)
        {
            return;
        }

        try
        {
            _loadIndexesTask.Wait();
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
        {
            // Handle the task cancellation if needed
        }
        finally
        {
            _loadIndexesTask = null;
        }
    }

    #endregion

    #region Overrides of BaseConversationControlViewModel

    /// <summary>
    /// Ask a query to the memory and if it found some content this is streams the chat asynchronously.
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

        try
        {
            RelevantSources.Clear();

            IsLoading = true;
            MemoryAnswer memoryAnswer = await _memoryOperationExecutor.ExecuteMemoryOperationAsync(
                async memoryServiceDecorator =>
                    await memoryServiceDecorator.AskAsync(
                        promptText,
                        StorageManagementViewModel.SelectedItem,
                        minRelevance: MinRelevance,
                        cancellationToken: cancellationToken), cancellationToken);

            if (memoryAnswer.NoResult)
            {
                // display a message to the user
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Memory Search - No result",
                    Content =
                        $"No result could be found. Reason {memoryAnswer.NoResultReason}.",
                };

                _ = await uiMessageBox.ShowDialogAsync(cancellationToken: cancellationToken);
                return;
            }

            // Set the relevant sources
            RelevantSources = new ObservableCollection<Citation>(memoryAnswer.RelevantSources);

            await base.DoChatStreamAsync(memoryAnswer, cancellationToken);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // TODO: Handle the System.ArgumentOutOfRangeException
        }
        catch (TaskCanceledException ex)
        {
            // TODO: Handle the System.Threading.Tasks.TaskCanceledException
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    public ICommand HyperlinkRequestNavigateCommand => new AsyncRelayCommand<Citation>(HyperlinkRequestNavigate);

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

    private async Task HyperlinkRequestNavigate(Citation? commandParameter)
    {
        if (commandParameter is null || string.IsNullOrEmpty(commandParameter.SourceUrl))
        {
            Debug.WriteLine("Command parameter or SourceUrl is null or empty.");
            return;
        }

        if (commandParameter.SourceName.EndsWith(".url"))
        {
            _processManager.Open(commandParameter.SourceUrl);
            return;
        }

        var documentId = commandParameter.DocumentId;
        var index = commandParameter.Index;
        var sourceName = commandParameter.SourceName;

        if (string.IsNullOrEmpty(index) || string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(sourceName))
        {
            Debug.WriteLine("Index, DocumentId, or Filename is null or empty.");
            return;
        }

        StreamableFileContent result = await _memoryOperationExecutor.ExecuteMemoryOperationAsync(
            async memoryServiceDecorator =>
                await memoryServiceDecorator.ExportFileAsync(documentId: documentId, fileName: sourceName, index: index));
        var stream = new MemoryStream();
        await (await result.GetStreamAsync()).CopyToAsync(stream);
        var bytes = stream.ToArray();

        await SaveBytesToFileAsync(bytes, sourceName);

        _processManager.Open(sourceName);
    }

    private async Task SaveBytesToFileAsync(byte[] bytes, string filePath)
    {
        if (bytes == null || string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(bytes), "Bytes array or file path cannot be null or empty.");
        }

        await _fileSystem.File.WriteAllBytesAsync(filePath, bytes);
    }
}