using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM
    {
       

        #region fields
        #endregion

        #region Properties
        public ObservableCollection<Transaction> Transactions { get; set; } = [new Transaction
            {
                Id = 1,
                totalamount = 250.50,
                Type = "sell",
                IsPaid = true,
                CreatedDate = DateTime.Now.AddDays(-5)
            },

            new Transaction
            {
                Id = 2,
                totalamount = 120.00,
                Type = "buy",
                IsPaid = false,
                CreatedDate = DateTime.Now.AddDays(-3)
            },

            new Transaction
            {
                Id = 3,
                totalamount = 500.75,
                Type = "sell",
                IsPaid = true,
                CreatedDate = DateTime.Now.AddDays(-2)
            },

            new Transaction
            {
                Id = 4,
                totalamount = 89.99,
                Type = "buy",
                IsPaid = false,
                CreatedDate = DateTime.Now.AddHours(-6)
            },

            new Transaction
            {
                Id = 5,
                totalamount = 1000.00,
                Type = "sell",
                IsPaid = false,
                CreatedDate = DateTime.Now.AddHours(-1)
            },

            new Transaction
            {
                Id = 6,
                totalamount = 345.20,
                Type = "buy",
                IsPaid = true,
                CreatedDate = DateTime.Now.AddDays(-10)
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
