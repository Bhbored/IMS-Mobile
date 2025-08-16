using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsPage(ReportsVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ReportsVM;
        vm.load();
    }
}