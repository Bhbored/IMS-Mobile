using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.Popups
{
    public partial class AvatarPickerPopup : CommunityToolkit.Maui.Views.Popup
    {
        public AvatarPickerPopup(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            BindingContext = settingsViewModel;
        }

        private async void OnCloseClicked(object? sender, EventArgs e)
        {
            await CloseAsync();
        }
    }
}
