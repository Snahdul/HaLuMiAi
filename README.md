# ChatConversationControl

ChatConversationControl is a WPF class library designed to manage and interact with chat conversations (Ollama, see settings). It leverages the CommunityToolkit.Mvvm for MVVM architecture and Microsoft.Extensions.AI for AI-based chat functionalities.

## Features

- Load and save chat conversations.
- Send chat prompts and receive responses.
- Stream chat responses.
- Clear chat conversations.

## Requirements

- .NET 9.0
- Visual Studio 2022
- Ollama

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Installation

1. Clone the repository:
    
2. Open the solution in Visual Studio 2022.

3. Restore the NuGet packages: `dotnet restore`

4. Install the WPF UI library: `dotnet add package Wpf-Ui`
        
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
- `CancelAsyncCommand`: Command to send a chat prompt.
- `SaveConversationAsyncCommand`: Command to save the conversation.
- `ClearConversationAsyncCommand`: Command to clear the conversation.
- `LoadConversationAsyncCommand`: Command to load a conversation.

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
