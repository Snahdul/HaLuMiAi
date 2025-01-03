using ChatConversationControl.Controls;

namespace ChatConversationControl.Test;

public class ConversationControlTests
{
    [WpfFact]
    public void ItemsSourceProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new object();

        // Act
        control.ItemsSource = expectedValue;
        var actualValue = control.ItemsSource;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void IsLoadingProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = true;

        // Act
        control.IsLoading = expectedValue;
        var actualValue = control.IsLoading;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void PromptProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = "Test Prompt";

        // Act
        control.Prompt = expectedValue;
        var actualValue = control.Prompt;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void ClearConversationCommandProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new RelayCommandForUnittests();

        // Act
        control.ClearConversationCommand = expectedValue;
        var actualValue = control.ClearConversationCommand;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void SaveConversationCommandProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new RelayCommandForUnittests();

        // Act
        control.SaveConversationCommand = expectedValue;
        var actualValue = control.SaveConversationCommand;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void LoadConversationCommandProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new RelayCommandForUnittests();

        // Act
        control.LoadConversationCommand = expectedValue;
        var actualValue = control.LoadConversationCommand;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void SendPromptCommandProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new RelayCommandForUnittests();

        // Act
        control.SendPromptCommand = expectedValue;
        var actualValue = control.SendPromptCommand;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [WpfFact]
    public void ConversationListProperty_Should_SetAndGetValue()
    {
        // Arrange
        var control = new ConversationControl();
        var expectedValue = new object();

        // Act
        control.ConversationList = expectedValue;
        var actualValue = control.ConversationList;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }
}

// Dummy RelayCommand class for testing purposes