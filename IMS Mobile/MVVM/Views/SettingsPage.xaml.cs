using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.Service;

namespace IMS_Mobile.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.SettingsPage = this;
        BindingContext = _viewModel;
    }

}