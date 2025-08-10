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
    private async void ConfirmAdd(object sender, EventArgs e)
    {
        if (BindingContext is ContactsVM vm)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await Application.Current.MainPage.DisplayAlert("Required Field", "Please enter a name to continue", "Got it");
                return;
            }

            int phoneNumber = 0;
            if (!string.IsNullOrWhiteSpace(PhoneNumberEntry.Text))
            {
                if (!int.TryParse(PhoneNumberEntry.Text, out phoneNumber))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid phone number", "OK");
                    return;
                }
            }

            NewContact = new Contact
            {
                Name = NameEntry.Text.Trim(),
                PhoneNumber = phoneNumber,
                Email = EmailEntry.Text ?? string.Empty,
                Address = AddressEntry.Text ?? string.Empty,
            };

            vm.AddContact(NewContact);
        }

        CloseAsync();
    }
}