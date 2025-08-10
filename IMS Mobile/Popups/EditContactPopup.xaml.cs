using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.ViewModels;
using PropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Contact = IMS_Mobile.MVVM.Models.Contact;
namespace IMS_Mobile.Popups;

public partial class EditContactPopup : Popup, INotifyPropertyChanged
{
    private Contact editedContact = new Contact();

    public Contact EditedContact
    {
        get => editedContact;
        set
        {
            editedContact = value;
            OnPropertyChanged();
        }
    }
    public EditContactPopup(ContactsVM VM, Contact contact)
    {
        InitializeComponent();
        BindingContext = VM;
        EditedContact = contact;
        FillEntries();
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
    public  void FillEntries()
    {
        NameEntry.Text = EditedContact.Name;
        PhoneNumberEntry.Text = EditedContact.PhoneNumber.ToString();
        AddressEntry.Text = EditedContact.Address;
        EmailEntry.Text = EditedContact.Email;
        OnPropertyChanged(nameof(EditedContact));
    }
    private void ConfirmEdit(object sender, EventArgs e)
    {
        if (BindingContext is ContactsVM vm)
        {
            EditedContact.Name = NameEntry.Text;
            EditedContact.PhoneNumber = int.TryParse(PhoneNumberEntry.Text, out var phone) ? phone : 0;
            EditedContact.Address = AddressEntry.Text;
            EditedContact.Email = EmailEntry.Text;
            vm.EditContact(EditedContact);
        }
        CloseAsync();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        CloseAsync();
    }
    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}