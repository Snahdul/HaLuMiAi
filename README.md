# Ollama Chat Conversation Control

# ChatConversationControl

ChatConversationControl is a WPF class library designed to manage and interact with chat conversations using Ollama and Retrieval-Augmented Generation (RAG). It leverages the CommunityToolkit.Mvvm for MVVM architecture and Microsoft.Extensions.AI for AI-based chat functionalities.

## Features

- Load and save chat conversations.
- Send chat prompts and receive responses.
- Stream chat responses.
- Clear chat conversations.
- Retrieval-Augmented Generation (RAG) for enhanced responses.
- Kernel memory for efficient data handling.

## Requirements

- .NET 9.0
- Visual Studio 2022
- Ollama

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [Ollama](https://ollama.com/)

### Installation

1. Clone the repository:
    
2. Open the solution in Visual Studio 2022.

3. Restore the NuGet packages: `dotnet restore`
  
4. To configure the Ollama settings for the application, you need to update the `appSettings.Production.json` file located in the `WPFUiDesktopApp` directory.
        
### Building the Project

To build the project, open the solution in Visual Studio and build the solution using `Ctrl+Shift+B` or by selecting `Build > Build Solution` from the menu.

### Running the Application

To run the application, press `F5` or select `Debug > Start Debugging` from the menu.

## Project Structure

- **ChatConversationControl**: The main WPF application project.
- **ChatConversationControl.Test**: The test project for unit tests.

## Key Components

### ViewModels

- `BaseConversationControlViewModel`: The base view model for conversation control, handling commands and chat interactions.

### Contracts

- `IConversationManager`: Interface defining the contract for managing conversations.

### Messages

- `MessageItem`: Represents a message item in the conversation.

## Commands

- `SendPromptAsyncCommand`: Command to send a chat prompt.
- `SendPromptStreamAsyncCommand`: Command to send a chat prompt with streaming response.
- `CancelAsyncCommand`: Command to cancel a send command.
- `SaveConversationAsyncCommand`: Command to save the conversation.
- `ClearConversationAsyncCommand`: Command to clear the conversation.
- `LoadConversationAsyncCommand`: Command to load a conversation.

## Chat Conversation with Ollama (Pure)

This mode allows for direct interaction with Ollama, providing chat functionalities without additional enhancements.

## Chat Conversation with RAG

RAG is used to enhance the responses by retrieving relevant information from a knowledge base before generating the final response. This helps in providing more accurate and contextually relevant answers.

## Kernel Memory

Kernel memory is utilized for efficient data handling and storage within the application. It ensures that the application can manage large amounts of data without performance degradation.

## Testing

The project uses xUnit for unit testing. To run the tests, use the Test Explorer in Visual Studio or run the following command: `dotnet test`.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [CommunityToolkit](https://github.com/CommunityToolkit/dotnet)
- [Microsoft.Extensions.AI](https://github.com/dotnet/ai-samples/blob/main/src/microsoft-extensions-ai/README.md)
- [WPF UI](https://github.com/lepoco/wpfui)

