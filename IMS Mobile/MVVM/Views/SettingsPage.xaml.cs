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

    private void OnChangeAvatarClicked(object sender, EventArgs e)
    {
        // TODO: Implement avatar selection from gallery/camera
        // For now, just set a placeholder path
        _viewModel.Avatar = "user.png";
    }
}