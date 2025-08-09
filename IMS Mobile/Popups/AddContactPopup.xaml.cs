using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.ViewModels;
using Contact = IMS_Mobile.MVVM.Models.Contact;
namespace IMS_Mobile.Popups;

public partial class AddContactPopup : Popup
{
    public Contact NewContact { get; set; } = new Contact();
    public AddContactPopup(ContactsVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        CloseAsync();
    }


    private void Button_Clicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
    private void ConfirmAdd(object sender, EventArgs e)
    {
        if (BindingContext is ContactsVM vm)
        {
            NewContact = new Contact
            {
                Name = NameEntry.Text,
                PhoneNumber = int.Parse(PhoneNumberEntry.Text),
                Email = EmailEntry.Text,
                Address = AddressEntry.Text,
            };
            vm.AddContact(NewContact);
        }
        CloseAsync();
    }
}