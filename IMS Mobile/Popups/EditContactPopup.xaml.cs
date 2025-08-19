using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.ViewModels;
using PropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.Popups;

public partial class EditContactPopup : Popup
{
    public Contact EditedContact { get; set; }

    public EditContactPopup(ContactsVM vm, Contact contact)
    {
        InitializeComponent();
        BindingContext = vm;

        EditedContact = new Contact
        {
            Id = contact.Id,
            Name = contact.Name,
            Email = contact.Email,
            Address = contact.Address,
            CreditScore = contact.CreditScore,
            PhoneNumber = contact.PhoneNumber,
            TotalPurchases = contact.TotalPurchases
        };

        NameEntry.Text = EditedContact.Name;
        PhoneNumberEntry.Text = EditedContact.PhoneNumber.ToString();
        AddressEntry.Text = EditedContact.Address;
        EmailEntry.Text = EditedContact.Email;
    }

    private async void ConfirmEdit(object sender, EventArgs e)
    {
        if (BindingContext is ContactsVM vm)
        {
            EditedContact.Name = NameEntry.Text;
            EditedContact.PhoneNumber = int.TryParse(PhoneNumberEntry.Text, out var phone) ? phone : 0;
            EditedContact.Address = AddressEntry.Text;
            EditedContact.Email = EmailEntry.Text;

            await vm.EditContact(EditedContact);
        }
        await CloseAsync();
    }

    private async void Button_Clicked(object sender, EventArgs e) => await CloseAsync();
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) => await CloseAsync();
}
