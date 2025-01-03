using ChatConversationControl.Contracts;
using ChatConversationControl.Implementation;
using ChatConversationControl.Messages;
using Moq;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Test
{
    public class ConversationManagerTests
    {
        private readonly TestConversationManager _conversationManager;
        private readonly ObservableCollection<MessageItem> _testMessages;
        private readonly MockFileSystem _mockFileSystem;
        private readonly Mock<IFileDialogService> _fileDialogServiceMock;
        private readonly Mock<IFileDialog> _fileDialogMock;

        public ConversationManagerTests()
        {
            _mockFileSystem = new MockFileSystem();
            _fileDialogServiceMock = new Mock<IFileDialogService>();
            _fileDialogMock = new Mock<IFileDialog>();
            _conversationManager = new TestConversationManager(_mockFileSystem, _fileDialogServiceMock.Object);
            _testMessages =
            [
                new MessageItem { Text = "Hello", ColorString = "Red" },
                new MessageItem { Text = "World", ColorString = "Blue" }
            ];
        }

        [Fact]
        public async Task SaveConversation_ShouldSaveToFile()
        {
            // Arrange
            _conversationManager.ConversationList.Add(_testMessages[0]);
            _conversationManager.ConversationList.Add(_testMessages[1]);

            _fileDialogMock.Setup(d => d.ShowDialog()).Returns(true);
            _fileDialogMock.Setup(d => d.FileName).Returns("test.json");
            _fileDialogServiceMock.Setup(s => s.CreateSaveFileDialog()).Returns(_fileDialogMock.Object);

            var filePath = "test.json";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var expectedJson = JsonSerializer.Serialize(_testMessages, options);

            // Act
            await _conversationManager.SaveConversation();

            // Assert
            var actualJson = _mockFileSystem.File.ReadAllText(filePath);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task LoadConversation_ShouldLoadFromFile()
        {
            // Arrange
            var filePath = "test.json";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            var json = JsonSerializer.Serialize(_testMessages, options);
            _mockFileSystem.AddFile(filePath, new MockFileData(json));

            _fileDialogMock.Setup(d => d.ShowDialog()).Returns(true);
            _fileDialogMock.Setup(d => d.FileName).Returns(filePath);
            _fileDialogServiceMock.Setup(s => s.CreateOpenFileDialog()).Returns(_fileDialogMock.Object);

            // Act
            await _conversationManager.LoadConversation();

            // Assert
            Assert.Equal(2, _conversationManager.ConversationList.Count);
            Assert.Equal("Hello", _conversationManager.ConversationList[0].Text);
            Assert.Equal("Red", _conversationManager.ConversationList[0].ColorString);
            Assert.Equal("World", _conversationManager.ConversationList[1].Text);
            Assert.Equal("Blue", _conversationManager.ConversationList[1].ColorString);
        }

        private class TestConversationManager(IFileSystem fileSystem, IFileDialogService fileDialogService)
            : ConversationManager(fileSystem, fileDialogService);
    }
}
