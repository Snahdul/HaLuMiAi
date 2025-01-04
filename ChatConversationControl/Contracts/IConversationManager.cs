﻿using ChatConversationControl.Messages;
using System.Collections.ObjectModel;

namespace ChatConversationControl.Contracts;

/// <summary>
/// Defines the contract for managing conversations.
/// </summary>
public interface IConversationManager
{
    /// <summary>
    /// Gets the list of conversation messages.
    /// </summary>
    ObservableCollection<MessageItem> ConversationList { get; }

    /// <summary>
    /// Loads a conversation.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task LoadConversation();

    /// <summary>
    /// Saves the conversation.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task SaveConversation();
}