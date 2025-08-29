using CommunityToolkit.Maui.Views;

namespace IMS_Mobile.Popups;

public partial class OfflineAlertPopup : Popup
{
	public OfflineAlertPopup()
	{
		InitializeComponent();
	}
    private void OnContinueClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
}