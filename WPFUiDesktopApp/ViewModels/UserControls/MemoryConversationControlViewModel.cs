using ChatConversationControl.Contracts;
using ChatConversationControl.ViewModels;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.AI;
using System.Text;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Models;

namespace WPFUiDesktopApp.ViewModels.UserControls;

/// <summary>
/// ViewModel for handling stream conversation control with memory.
/// </summary>
public partial class MemoryConversationControlViewModel : BaseConversationControlViewModel, INavigationAware
{
    public StorageManagementViewModel StorageManagementViewModel { get; }
    private readonly OllamaMemoryModel _ollamaKernelMemory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryConversationControlViewModel"/> class.
    /// </summary>
    /// <param name="conversationManager">The conversation manager.</param>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="ollamaKernelMemory">The Ollama kernel memory model.</param>
    /// <param name="storageManagementViewModel">The storage management view model.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="conversationManager" /> or <paramref name="chatClient" /> or <paramref name="ollamaKernelMemory" /> is <see langword="null" />.
    /// </exception>
    public MemoryConversationControlViewModel(
        IConversationManager conversationManager,
        IChatClient chatClient,
        OllamaMemoryModel ollamaKernelMemory,
        StorageManagementViewModel storageManagementViewModel) : base(conversationManager, chatClient)
    {
        Guard.IsNotNull(conversationManager);
        Guard.IsNotNull(chatClient);
        Guard.IsNotNull(ollamaKernelMemory);
        Guard.IsNotNull(storageManagementViewModel);

        _ollamaKernelMemory = ollamaKernelMemory;
        StorageManagementViewModel = storageManagementViewModel;
    }

    #region Implementation of INavigationAware

    /// <inheritdoc />
    public async void OnNavigatedTo()
    {
        var indexes = await _ollamaKernelMemory.ListIndexesAsync();

        foreach (var indexDetail in indexes)
        {
            this.StorageManagementViewModel.StorageIndexes.Add(indexDetail.Name);
        }

        if (this.StorageManagementViewModel.StorageIndexes.Any())
        {
            StorageManagementViewModel.SelectedItem = this.StorageManagementViewModel.StorageIndexes.First();
        }
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
    /// <inheritdoc />
    protected override async Task DoChatStreamAsync(object? prompt)
    {
        IsLoading = true;

        try
        {
            if (prompt is not string promptText || string.IsNullOrWhiteSpace(promptText))
            {
                IsLoading = false;
                return;
            }

            var response = await _ollamaKernelMemory.AskAsync(promptText, StorageManagementViewModel.SelectedItem);

            if (response is null || string.IsNullOrEmpty(response.Result))
            {
                return;
            }

            StringBuilder stringBuilder = new(response.Result);
            stringBuilder.Append(promptText);

            IsLoading = false;

            await base.DoChatStreamAsync(stringBuilder.ToString());
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}