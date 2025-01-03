namespace ChatConversationControl.Contracts;

public interface IFileDialog
{
    bool? ShowDialog();
    string FileName { get; }
}