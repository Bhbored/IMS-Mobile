using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.Popups;

public partial class DatePickerPopup : Popup
{
    private readonly HomeVM _homeVM;

    public DatePickerPopup(HomeVM homeVM)
    {
        InitializeComponent();
        _homeVM = homeVM;
        BindingContext = new DateFilterViewModel(homeVM);
    }

    

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void OnApplyClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as DateFilterViewModel;
        if (vm != null)
        {
            await vm.ApplyFilter();
        }
        await CloseAsync();
    }
}
