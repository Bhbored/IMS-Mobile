using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ContactsVM
    {

        #region fields
        #endregion

        #region Properties
        public ObservableCollection<Contact> Contacts { get; set; } = [new Contact
            {
                Id = 1,
                Name = "John Smith",
                PhoneNumber = 234567,
                CreditScore = 750.00,
                TotalPurchases = 2500.50
            },

            new Contact
            {
                Id = 2,
                Name = "Sarah Johnson",
                PhoneNumber = 71048125,
                CreditScore = 820.00,
                TotalPurchases = 1800.75
            },

            new Contact
            {
                Id = 3,
                Name = "Mike Davis",
                PhoneNumber = 71048125,
                CreditScore = 680.00,
                TotalPurchases = 3200.25
            },

            new Contact
            {
                Id = 4,
                Name = "Emily Wilson",
                PhoneNumber = 71048125,
                CreditScore = 900.00,
                TotalPurchases = 4500.00
            },

            new Contact
            {
                Id = 5,
                Name = "David Brown",
                PhoneNumber = 71048125,
                CreditScore = 720.00,
                TotalPurchases = 1950.80
            }];
        #endregion

        #region Methods

        #endregion

        #region commands
        #endregion

        #region Tasks
        #endregion
    }
}