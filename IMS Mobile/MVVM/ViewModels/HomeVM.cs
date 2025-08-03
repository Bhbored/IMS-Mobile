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
using Microsoft.Maui.Controls;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM : INotifyPropertyChanged
    {
       
        public HomeVM()
        {
            GenerateTestData();
            Pagination();
        }

        #region Properties
        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> FilteredTransactions { get; set; } = new ObservableCollection<Transaction>();

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                OnPropertyChanged();
            }
        }
        #region fields
        private int _pageIndex = 1;

        #endregion

        #endregion



        #region Methods

        private void GenerateTestData()
        {
            var random = new Random();
            var types = new[] { "buy", "sell" };

            for (int i = 1; i <= 50; i++)
            {
                Transactions.Add(new Transaction
                {
                    Id = i,
                    totalamount = Math.Round(random.NextDouble() * 1000 + 50, 2),
                    Type = types[random.Next(types.Length)],
                    IsPaid = random.Next(2) == 1,
                    CreatedDate = DateTime.Now.AddDays(-random.Next(30))
                });
            }
        }

        public void incrementPageIndex()
        {
            var maxPages = (int)Math.Ceiling((double)(Transactions.Count / 10));
            if (PageIndex < maxPages)
            {
                PageIndex++;
                Pagination();
            }
        }

        public void decrementPageIndex()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                Pagination();
            }
        }

        public void Pagination()
        {
            var maxPages = (int)Math.Ceiling((double)(Transactions.Count / 10));
            if (PageIndex <= maxPages || PageIndex >= 1)
            {
                var sortedTransactions = Transactions.OrderByDescending(t => t.CreatedDate).ToList();
                FilteredTransactions = new ObservableCollection<Transaction>();
                var paginatedTransactions = sortedTransactions.Skip((PageIndex - 1) * 10).Take(10).ToList();
                foreach (var transaction in paginatedTransactions)
                {
                    FilteredTransactions.Add(transaction);
                }
                OnPropertyChanged(nameof(FilteredTransactions));
                OnPropertyChanged(nameof(PageIndex));
            }
        }

        public void FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            var filtered = Transactions.Where(t => t.CreatedDate.Date >= startDate.Date && t.CreatedDate.Date <= endDate.Date).ToList();
            FilteredTransactions = new ObservableCollection<Transaction>(filtered);
            FilteredTransactions.OrderByDescending(t => t.CreatedDate).ToList();
            PageIndex = 1;
            OnPropertyChanged(nameof(FilteredTransactions));
            OnPropertyChanged(nameof(PageIndex));
        }
        public void FilterBuy()
        {
            var filtered = Transactions.Where(t => t.Type == "buy").ToList();
            FilteredTransactions = new ObservableCollection<Transaction>(filtered);
            FilteredTransactions.OrderByDescending(t => t.CreatedDate).ToList();
            PageIndex = 1;
            OnPropertyChanged(nameof(FilteredTransactions));
            OnPropertyChanged(nameof(PageIndex));
        }
        public void FilterSell()
        {
            var filtered = Transactions.Where(t => t.Type == "sell").ToList();
            FilteredTransactions = new ObservableCollection<Transaction>(filtered);
            FilteredTransactions.OrderByDescending(t => t.CreatedDate).ToList();
            PageIndex = 1;
            OnPropertyChanged(nameof(FilteredTransactions));
            OnPropertyChanged(nameof(PageIndex));
        }



        public void BackToFirstPage()
        {
            PageIndex = 1;
            Pagination();
        }
        public void BackToLastPage()
        {
            PageIndex = (int)Math.Ceiling((double)(Transactions.Count / 10));
            Pagination();
        }

        #endregion

        #region commands
        public ICommand PreviousPage => new Command(() =>
        {
            decrementPageIndex();
        });
        public ICommand NextPage => new Command(() =>
        {
            incrementPageIndex();
        });
        public ICommand FirstPage => new Command(() =>
        {
            BackToFirstPage();
        });
        public ICommand LastPage => new Command(() =>
        {
            BackToLastPage();
        });
        public ICommand BuyFilterCommand => new Command(() =>
        {
            FilterBuy();
        });
        public ICommand SellFilterCommand => new Command(() =>
        {
            FilterSell();
        });
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
