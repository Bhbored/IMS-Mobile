using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.Popups;

public partial class ContactSearchPopup : Popup, INotifyPropertyChanged
{
    public static Contact? SelectedContactResult { get; set; }
    public static InventoryVM inventoryVM
    {
        get => inventoryVM1;
        set
        {
            inventoryVM1 = value;
        }
    }

    public ContactSearchPopup(InventoryVM VM)
    {
        InitializeComponent();
        BindingContext = this;
        FillContacts();
        inventoryVM  = VM;
    }

    private Contact _selectedContact;
    private static InventoryVM? inventoryVM1;

    public Contact SelectedContact
    {
        get => _selectedContact;
        set
        {
            _selectedContact = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsContactSelected));
            OnPropertyChanged(nameof(IsContactNotSelected));
        }
    }

    public bool IsContactSelected => SelectedContact != null;
    public bool IsContactNotSelected => SelectedContact == null;

    
    public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();

    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        SelectedContactResult = new Contact();
        await CloseAsync();
    }

    private async void SelectButton_Clicked(object sender, EventArgs e)
    {
        SelectedContactResult = SelectedContact;
        inventoryVM.SellCredit(SelectedContact);
        await CloseAsync();
    }

    public void FillContacts()
    {
        try
        {
            Contacts.Clear();
            var dbContacts = App.ContactRepository.GetItems();
            foreach (var contact in dbContacts)
            {
                Contacts.Add(contact);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading contacts: {ex.Message}");
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}