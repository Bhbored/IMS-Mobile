using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.Popups;

public partial class DeleteContactPopup : Popup
{
    public Contact DeletedContact { get; set; } = new Contact();
    public DeleteContactPopup(ContactsVM vm , Contact contact)
	{
		InitializeComponent();
		BindingContext = vm;
        DeletedContact = contact;
    }
    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if(BindingContext is ContactsVM vm)
        {
           vm.DeleteContact(DeletedContact);
        }
        CloseAsync();
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
}