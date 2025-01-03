namespace ChatConversationControl.Contracts;

public interface IFileDialogService
{
    IFileDialog CreateSaveFileDialog();
    IFileDialog CreateOpenFileDialog();
}