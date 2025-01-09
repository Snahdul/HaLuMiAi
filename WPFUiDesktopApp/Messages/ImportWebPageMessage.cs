namespace WPFUiDesktopApp.Messages;

internal class ImportWebPageMessage(string urlString)
{
    public string UrlString { get; } = urlString;
}