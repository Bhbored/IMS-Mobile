using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ContactsVM : INotifyPropertyChanged
    {


        #region fields

        private ObservableCollection<Contact> filteredContacts = new ObservableCollection<Contact>();
        private ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();
        private ContactsPage contactsPage;
        #endregion

        #region Properties
        public ObservableCollection<Contact> Contacts
        {
            get => contacts;
            set
            {
                contacts = value;
                OnPropertyChanged();
            }
        }
        public IList<object> SelectedContacts { get; set; } = new List<object>();
        public ObservableCollection<Contact> FilteredContacts
        {
            get => filteredContacts;
            set
            {
                filteredContacts = value;
                OnPropertyChanged();
            }
        }
        public ContactsPage ContactsPage
        {
            get => contactsPage;
            set
            {
                contactsPage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods

        public void FilterProducts()
        {
            if (SelectedContacts.Count == 0)
            {
                FilteredContacts = new ObservableCollection<Contact>(Contacts);
            }
            else
            {
                var filteredList = new List<Contact>();

                foreach (var selectedItem in SelectedContacts)
                {
                    if (selectedItem is Contact selectedProduct)
                    {
                        var matchingProduct = Contacts.FirstOrDefault(p => p.Name == selectedProduct.Name);
                        if (matchingProduct != null)
                        {
                            filteredList.Add(matchingProduct);
                        }
                    }
                }

                FilteredContacts = new ObservableCollection<Contact>(filteredList);
            }
        }
        public async void AddContact(Contact contact)
        {
            if (contact != null && !Contacts.Any(x => x.Name == contact.Name))
            {
                App.ContactRepository.InsertItem(contact);
                await LoadContacts();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Added Successfully",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.LightGreen,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: contactsPage
                );
                    await snackbar.Show();
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Already Exist",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CornerRadius = 10,
                    },
                    anchor: contactsPage
                );
                    await snackbar.Show();
                });
            }
        }
        #endregion

        #region commands

        public Command AddContactCommand => new Command(() =>
        {
            AppShell.Current.ShowPopupAsync(new AddContactPopup(this));
        });
        #endregion

        #region Tasks
        public async Task LoadContacts()
        {

            Contacts.Clear();
            filteredContacts.Clear();
            var DBContacts = App.ContactRepository.GetItems();

            foreach (var contact in DBContacts)
            {
                Contacts.Add(contact);
            }
            FilteredContacts = new ObservableCollection<Contact>(Contacts);
            await Task.CompletedTask;
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}