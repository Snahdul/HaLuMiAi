using System.Windows.Controls;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for OllamaPage.xaml
    /// </summary>
    public partial class OllamaPage : Page, INavigationAware
    {
        public OllamaPage(OllamaViewModel viewModel)
        {
            InitializeComponent();

            this.ViewModel = viewModel;
            this.DataContext = this;
        }

        public OllamaViewModel ViewModel { get; }

        #region Implementation of INavigationAware

        /// <inheritdoc />
        public void OnNavigatedTo()
        {
            ViewModel.OllamaMemoryViewModel.OnNavigatedTo();
        }

        /// <inheritdoc />
        public void OnNavigatedFrom()
        {
            ViewModel.OllamaMemoryViewModel.OnNavigatedFrom();
        }

        #endregion
    }
}