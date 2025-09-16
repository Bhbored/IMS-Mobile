using IMS_Mobile.MVVM.Views;
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
using System.Windows.Input;
using cashRegister = IMS_Mobile.MVVM.Models.cashRegister;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ReportsVM : INotifyPropertyChanged
    {
        private cashRegister cashRegister = new cashRegister();
        public bool Animation { get; set; } = false;

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
        public static ReportsVM? Current { get; private set; }
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
            .Sum(x => x.stock * x.Price);
            CashRegister.TotalSales = transactions
            .Where(x => x.Type == "sell")
            .Sum(x => x.totalamount);
            CashRegister.TotalCredit = transactions
            .Where(x => x.Type == "sell" && x.IsPaid == false)
            .Sum(x => x.totalamount);
            CashRegister.TotalPurchases = transactions
            .Where(x => x.Type == "buy")
            .Sum(x => x.totalamount);
            CashRegister.NetProfit = CashRegister.TotalSales - CashRegister.TotalCredit - CashRegister.TotalPurchases;
            OnPropertyChanged(nameof(CashRegister));
            OnPropertyChanged(nameof(CashRegister.TotalSales));
            OnPropertyChanged(nameof(CashRegister.TotalPurchases));
            OnPropertyChanged(nameof(CashRegister.TotalCredit));
            OnPropertyChanged(nameof(CashRegister.InventoryValue));
            OnPropertyChanged(nameof(CashRegister.CashFlow));
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
        public async Task ViewAnalytics()
        {
            Animation = true;
            await Shell.Current.GoToAsync(nameof(Analytics), true);
            Animation = false;
        }
        public async Task ViewReports()
        {
            Animation = true;
            await Shell.Current.GoToAsync(nameof(GeneratedReport), true);
            Animation = false;
        }
        public ICommand NavigateToAnalyticsCommand => new Command(async () => await ViewAnalytics());
        public ICommand NavigateToReportsCommand => new Command(async () => await ViewReports());
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