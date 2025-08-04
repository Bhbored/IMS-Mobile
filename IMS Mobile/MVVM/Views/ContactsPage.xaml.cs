using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class ContactsPage : ContentPage
{
	public ContactsPage(ContactsVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}