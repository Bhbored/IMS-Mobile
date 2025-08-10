using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class ContactsPage : ContentPage
{
	public ContactsPage(ContactsVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ContactsVM vm)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await vm.LoadContacts();
            });
        }
    }

    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if (BindingContext is ContactsVM vm)
        {
            vm.FilterContacts();
        }
    }
}