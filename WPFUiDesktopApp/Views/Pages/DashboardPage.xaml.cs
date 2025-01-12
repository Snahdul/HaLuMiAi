using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.Views.Pages;

public partial class DashboardPage : INavigableView<DashboardViewModel>
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}