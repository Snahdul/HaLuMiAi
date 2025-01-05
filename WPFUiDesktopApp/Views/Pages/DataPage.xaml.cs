using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
