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
using System.Transactions;
using Contact = IMS_Mobile.MVVM.Models.Contact;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

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
        public bool Animation { get; set; } = false;
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
        public async Task EditContact(Contact edited)
        {
            if (edited is null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{edited.Name} Already Exist",
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
                return;
            }

            var original = App.ContactRepository.GetItem(edited.Id);
            if (original is null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"{edited.Name} is Not Found",
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
                return;
            }

            ClonedContact = new Contact
            {
                Id = original.Id,
                Name = original.Name,
                Email = original.Email,
                Address = original.Address,
                CreditScore = original.CreditScore,
                PhoneNumber = original.PhoneNumber,
                TotalPurchases = original.TotalPurchases
            };

            original.Name = edited.Name;
            original.Email = edited.Email;
            original.Address = edited.Address;
            original.CreditScore = edited.CreditScore;
            original.PhoneNumber = edited.PhoneNumber;
            original.TotalPurchases = edited.TotalPurchases;

            App.ContactRepository.UpdateItem(original);
            await LoadContacts();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                var snackbar = Snackbar.Make(
                message: $"{original.Name} Updated !",
                duration: TimeSpan.FromSeconds(2),
                 action: () => UndoEdit(original.Id),
                    actionButtonText: "UNDO",
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

        public async void DeleteContact(Contact contact)
        {
            if (contact != null)
            {
                var tobecloned = App.ContactRepository.GetItems().FirstOrDefault(x => x.Id == contact.Id);
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
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                Address = contact.Address,
                TotalPurchases = contact.TotalPurchases,
                CreditScore = contact.CreditScore,
            };
        }
        public async void UndoEdit(int contactId)
        {
            if (ClonedContact is null || ClonedContact.Id != contactId)
            {
                await Snackbar.Make("Nothing to undo", duration: TimeSpan.FromSeconds(2), anchor: contactsPage).Show();
                return;
            }

            var current = App.ContactRepository.GetItem(contactId);
            if (current is not null)
            {
                current.Name = ClonedContact.Name;
                current.Email = ClonedContact.Email;
                current.Address = ClonedContact.Address;
                current.CreditScore = ClonedContact.CreditScore;
                current.PhoneNumber = ClonedContact.PhoneNumber;
                current.TotalPurchases = ClonedContact.TotalPurchases;
                App.ContactRepository.UpdateItem(current);
            }
            else
            {
                App.ContactRepository.InsertItem(ClonedContact);
            }

            ClonedContact = null;
            await LoadContacts();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                var snackbar = Snackbar.Make(
                message: "Edit Undone",
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
        public List<Transaction> LoadContactTransaction(Contact contact)
        {
            var transactions = App.TransactionRepository.GetItemsWithChildren()
            .Where(x => x.ContactId == contact.Id)
            .ToList();
            return transactions;
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
            Animation = true;
            await App.Current.MainPage.Navigation.PushAsync(new ContactDetailsPage(contact, LoadContactTransaction(contact)));
            Animation = false;
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