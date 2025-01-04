using ChatConversationControl.Contracts;
using ChatConversationControl.Implementation;
using ChatConversationControl.Messages;
using Moq;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatConversationControl.Test
{
    public class ConversationManagerTests
    {
        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly Mock<IFile> _fileMock;
        private readonly Mock<IFileDialogService> _fileDialogServiceMock;
        private readonly Mock<IOpenFileDialog> _openFileDialogMock;
        private readonly Mock<ISaveFileDialog> _saveFileDialogMock;
        private readonly TestConversationManager _conversationManager;

        public ConversationManagerTests()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _fileMock = new Mock<IFile>();
            _fileDialogServiceMock = new Mock<IFileDialogService>();
            _openFileDialogMock = new Mock<IOpenFileDialog>();
            _saveFileDialogMock = new Mock<ISaveFileDialog>();

            _fileSystemMock.Setup(fs => fs.File).Returns(_fileMock.Object);

            _conversationManager = new TestConversationManager(_fileSystemMock.Object, _fileDialogServiceMock.Object);
        }

        [Fact]
        public async Task SaveConversation_ShouldSaveToFile_WhenDialogConfirmed()
        {
            // Arrange
            var filePath = "test.json";
            var conversationList = new ObservableCollection<MessageItem>
            {
                new MessageItem { Text = "Hello" },
                new MessageItem { Text = "World" }
            };
            _conversationManager.ConversationList.Add(conversationList[0]);
            _conversationManager.ConversationList.Add(conversationList[1]);

            _saveFileDialogMock.Setup(d => d.ShowDialog()).Returns(true);
            _saveFileDialogMock.Setup(d => d.FileName).Returns(filePath);
            _fileDialogServiceMock.Setup(s => s.CreateSaveFileDialog()).Returns(_saveFileDialogMock.Object);

            // Act
            await _conversationManager.SaveConversation();

            // Assert
            _fileMock.Verify(f => f.WriteAllTextAsync(filePath, It.IsAny<string>(), default), Times.Once);
        }

        [Fact]
        public async Task LoadConversation_ShouldLoadFromFile_WhenDialogConfirmed()
        {
            // Arrange
            var filePath = "test.json";
            var conversationList = new ObservableCollection<MessageItem>
            {
                new MessageItem { Text = "Hello" },
                new MessageItem { Text = "World" }
            };
            var jsonContent = JsonSerializer.Serialize(conversationList, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });

            _openFileDialogMock.Setup(d => d.ShowDialog()).Returns(true);
            _openFileDialogMock.Setup(d => d.FileName).Returns(filePath);
            _fileDialogServiceMock.Setup(s => s.CreateOpenFileDialog()).Returns(_openFileDialogMock.Object);
            _fileMock.Setup(f => f.ReadAllTextAsync(filePath, default)).ReturnsAsync(jsonContent);

            // Act
            await _conversationManager.LoadConversation();

            // Assert
            Assert.Equal(2, _conversationManager.ConversationList.Count);
            Assert.Equal("Hello", _conversationManager.ConversationList[0].Text);
            Assert.Equal("World", _conversationManager.ConversationList[1].Text);
        }

        private class TestConversationManager : ConversationManager
        {
            public TestConversationManager(IFileSystem fileSystem, IFileDialogService fileDialogService)
                : base(fileSystem, fileDialogService)
            {
            }
        }
    }
}

