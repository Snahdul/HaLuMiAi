using System.Windows.Input;

namespace ChatConversationControl.Test;

public class RelayCommandForUnittests : ICommand
{
    public event EventHandler? CanExecuteChanged = delegate { };

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) { }
}