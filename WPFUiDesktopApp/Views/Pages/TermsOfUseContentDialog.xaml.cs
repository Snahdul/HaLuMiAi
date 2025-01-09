// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Windows.Controls;
using Wpf.Ui.Controls;
using WPFUiDesktopApp.ViewModels.Pages;

namespace WPFUiDesktopApp.Views.Pages;

public partial class TermsOfUseContentDialog : ContentDialog
{
    public TermsOfUseContentDialog(ContentPresenter? contentPresenter, WebpageImportDialogViewModel webpageImportDialogViewModel)
        : base(contentPresenter)
    {
        InitializeComponent();

        this.DataContext = webpageImportDialogViewModel;
    }

    //protected override void OnButtonClick(ContentDialogButton button)
    //{
    //}
}
