using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using cashRegister = IMS_Mobile.MVVM.Models.cashRegister;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ReportsVM : INotifyPropertyChanged
    {
        private cashRegister cashRegister = new cashRegister();

        public ReportsVM()
        {
        }
        #region fields
        #endregion

        #region Properties
        public cashRegister CashRegister
        {
            get => cashRegister;
            set
            {
                cashRegister = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public void FillCashRegister()
        {

            var products = App.ProductRepository.GetItems();
            var transactions = App.TransactionRepository.GetItems();
            var contacts = App.ContactRepository.GetItems();
            CashRegister.CashFlow = transactions
            .Where(x => x.Type == "sell" && x.IsPaid == true)
            .Sum(x => x.totalamount);
            CashRegister.InventoryValue = products
            .Sum(x => x.Quantity * x.Price);
            CashRegister.TotalSales = transactions
            .Where(x => x.Type == "sell")
            .Sum(x => x.totalamount);
            CashRegister.TotalCredit = transactions
            .Where(x => x.Type == "sell" && x.IsPaid == false)
            .Sum(x => x.totalamount);
            CashRegister.TotalPurchases = transactions
            .Where(x => x.Type == "buy")
            .Sum(x => x.totalamount);
            OnPropertyChanged(nameof(CashRegister));

        }
        public void load()
        {
            CashRegister = new cashRegister();
            FillCashRegister();
            OnPropertyChanged(nameof(CashRegister));
        }
        #endregion

        #region commands
        #endregion

        #region Tasks

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