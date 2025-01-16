using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace ChatConversationControl.Extensions;

public static class ChatHistoryExtensions
{
    /// <summary>
    /// Concatenates all messages in the chat history into a single string.
    /// </summary>
    /// <param name="chatHistory">The chat history.</param>
    /// <returns>A string containing all messages in the chat history.</returns>
    public static string GetFullPrompt(this ChatHistory chatHistory)
    {
        var sb = new StringBuilder();
        foreach (var message in chatHistory)
        {
            sb.AppendLine($"{message.AuthorName}: {message.Content}");
        }
        return sb.ToString();
    }
}