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
        private Contact clonedContact = new Contact();
        private bool isRefreshing = false;
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
        public Contact ClonedContact
        {
            get => clonedContact;
            set
            {
                clonedContact = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public void SortContacts()
        {
            var sortedList = Contacts.OrderBy(c => c.Name).ToList();
            foreach (var contact in sortedList)
            {
                if (!Contacts.Contains(contact))
                {
                    Contacts.Add(contact);
                }
            }
            FilteredContacts = new ObservableCollection<Contact>(sortedList);
        }
        public void FilterContacts()
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
        public async void EditContact(Contact contact)
        {
            if (contact != null)
            {
                var tobecloned = App.ContactRepository.GetItems().FirstOrDefault(x => x.Name == contact.Name);
                CloneContact(tobecloned);
                App.ContactRepository.UpdateItem(contact);
                await LoadContacts();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Updated Successfully",
                    action: () => UndoEdit(contact),
                    actionButtonText: "UNDO",
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
                    message: $"{contact.Name} Not Found",
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
        public async void DeleteContact(Contact contact)
        {
            if (contact != null)
            {
                var tobecloned = App.ContactRepository.GetItems().FirstOrDefault(x => x.Name == contact.Name);
                CloneContact(tobecloned);
                App.ContactRepository.DeleteItem(contact);
                await LoadContacts();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Deleted Successfully",
                     action: () => UndoDelete(contact),
                    actionButtonText: "UNDO",
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
                    message: $"{contact.Name} Not Found",
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

        public void CloneContact(Contact contact)
        {

            ClonedContact = new Contact
            {
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                Address = contact.Address
            };
        }
        public void UndoEdit(Contact contact)
        {
            if (contact != null)
            {
                App.ContactRepository.DeleteItem(contact);
                App.ContactRepository.InsertItem(ClonedContact);
                ClonedContact = new Contact();
                _ = LoadContacts();
                OnPropertyChanged(nameof(ClonedContact));
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Edit Undone Successfully",
                    duration: TimeSpan.FromSeconds(3),
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
                    message: $"{contact.Name} Not Found",
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
        public void UndoDelete(Contact contact)
        {
            if (contact != null)
            {

                App.ContactRepository.InsertItem(ClonedContact);
                ClonedContact = new Contact();
                _ = LoadContacts();
                OnPropertyChanged(nameof(ClonedContact));
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{contact.Name} Delete Undone Successfully",
                    duration: TimeSpan.FromSeconds(3),
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
                    message: $"{contact.Name} Not Found",
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
        public Command EditContactCommand => new Command<Contact>((Contact) =>
        {
            AppShell.Current.ShowPopupAsync(new EditContactPopup(this, Contact));
        });
        public Command DeleteContactCommand => new Command<Contact>((Contact) =>
        {
            AppShell.Current.ShowPopupAsync(new DeleteContactPopup(this, Contact));
        });
        public Command RefreshCommand => new Command(async () =>
        {
            await RefreshContacts();
        });
        public Command ShowDetailsCommand => new Command<Contact>(async (contact) =>
        {
            await App.Current.MainPage.Navigation.PushAsync(new ContactDetailsPage(contact));
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
            SortContacts();
            await Task.CompletedTask;
        }
        public async Task RefreshContacts()
        {
            IsRefreshing = true;
            await Task.Delay(1000);
            await LoadContacts();
            IsRefreshing = false;
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