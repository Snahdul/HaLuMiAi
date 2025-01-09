using CommunityToolkit.Mvvm.Messaging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.Messages;
using WPFUiDesktopApp.Views.Pages;

namespace WPFUiDesktopApp.ViewModels.Pages;

/// <summary>
/// ViewModel for the Webpage Import Dialog.
/// </summary>
public partial class WebpageImportDialogViewModel(IContentDialogService contentDialogService) : ObservableObject
{
    /// <summary>
    /// The caption of the dialog.
    /// </summary>
    [ObservableProperty] private string _caption = "Import Webpage";

    /// <summary>
    /// The URL string to be imported.
    /// </summary>
    [ObservableProperty] private string _urlString = string.Empty;

    /// <summary>
    /// Command to show the Terms of Use content dialog.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnShowTermsOfUseContentDialogAsync()
    {
        var contentPresenter = contentDialogService.GetDialogHost();

        var termsOfUseContentDialog = new TermsOfUseContentDialog(contentPresenter, this);
        var contentDialogResult = await termsOfUseContentDialog.ShowAsync();

        switch (contentDialogResult)
        {
            case ContentDialogResult.None:
                // close
                break;
            case ContentDialogResult.Primary:
                // save
                if (!string.IsNullOrWhiteSpace(UrlString))
                {
                    WeakReferenceMessenger.Default.Send(new ImportWebPageMessage(UrlString));
                }
                break;
            case ContentDialogResult.Secondary:
                // cancel
                break;
            default:
                break;
        }
    }
}