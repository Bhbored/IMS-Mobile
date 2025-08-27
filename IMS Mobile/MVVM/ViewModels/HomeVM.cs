using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
using Microsoft.Maui.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class HomeVM : ObservableObject, INotifyPropertyChanged
    {
        private readonly SupabaseAuthService _authService;
        private readonly SyncService _syncService;

        [ObservableProperty]
        private bool _isSyncing;

        [ObservableProperty]
        private string _syncStatus = "Ready";
       public static HomeVM? Instance { get; private set; }
        public HomeVM(SupabaseAuthService authService, SyncService syncService)
        {
            _authService = authService;
            _syncService = syncService;
        }

        [RelayCommand]
        private async Task SyncToCloudAsync()
        {
            if (IsSyncing) return;

            IsSyncing = true;
            SyncStatus = "Syncing to cloud...";

            try
            {
                if (await _authService.ValidateSessionAsync())
                {
                    await _syncService.SyncToSupabase();
                    SyncStatus = "Sync completed successfully!";
                }
                else
                {
                    SyncStatus = "Authentication required";
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception ex)
            {
                SyncStatus = $"Sync failed: {ex.Message}";
            }
            finally
            {
                IsSyncing = false;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool result = await Shell.Current.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes", "No");

            if (result)
            {
                await _authService.SignOutAsync();
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        // Quick logout without confirmation (wire to a button if desired)
        [RelayCommand]
        public async Task QuickLogoutAsync()
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        #region Properties
        public HomePage HomePage { get; set; }
        public bool IsRefreshing { get; set; } = false;

        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> FilteredTransactions { get; set; } = new ObservableCollection<Transaction>();
        public double CashFLow
        {
            get => cashFLow;
            set
            {
                cashFLow = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pagination
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                OnPropertyChanged();
            }
        }

        private int _pageIndex = 1;
        private List<Transaction> _currentFilteredList = new List<Transaction>();
        private double cashFLow;
        

        public void incrementPageIndex()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);
            if (PageIndex < maxPages)
            {
                PageIndex++;
                ApplyPagination();
            }
        }

        public void decrementPageIndex()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                ApplyPagination();
            }
        }

        private void ApplyPagination()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);

            if (PageIndex > maxPages && maxPages > 0)
                PageIndex = maxPages;
            if (PageIndex < 1)
                PageIndex = 1;

            var paginatedTransactions = _currentFilteredList
                .Skip((PageIndex - 1) * 10)
                .Take(10)
                .ToList();

            FilteredTransactions.Clear();
            foreach (var transaction in paginatedTransactions)
            {
                FilteredTransactions.Add(transaction);
            }
        }

        public void BackToFirstPage()
        {
            PageIndex = 1;
            ApplyPagination();
        }

        public void BackToLastPage()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);
            PageIndex = maxPages > 0 ? maxPages : 1;
            ApplyPagination();
        }

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
        #endregion

        #region Filtering
        public void FilterByDateRange(DateTime startDate, DateTime endDate)
        {

            _currentFilteredList = Transactions
                .Where(t => t.CreatedDate.Date >= startDate.Date && t.CreatedDate.Date <= endDate.Date)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
            OnPropertyChanged(nameof(_currentFilteredList));
            var days = (endDate - startDate).Days;
            if (_currentFilteredList.Count != 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: $"Showing transactions from {startDate:MMM dd} to {endDate:MMM dd} ({days + 1} days)",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.LightGreen,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: HomePage
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
                    message: $"No transactions from {startDate:MMM dd} to {endDate:MMM dd}",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: HomePage
                );
                    await snackbar.Show();
                });
            }
        }


        public void FilterBuy()
        {
            _currentFilteredList = Transactions
                .Where(t => t.Type == "buy")
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public void FilterSell()
        {
            _currentFilteredList = Transactions
                .Where(t => t.Type == "sell")
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public void ShowAllTransactions()
        {
            _currentFilteredList = Transactions
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public ICommand BuyFilterCommand => new Command(() =>
        {
            FilterBuy();
        });
        public ICommand SellFilterCommand => new Command(() =>
        {
            FilterSell();
        });
        public ICommand AllFilterCommand => new Command(() =>
        {
            ShowAllTransactions();
        });
        #endregion

        #region Commands
        public ICommand DetailsPopup => new Command<Transaction>((transaction) =>
        {
            var products = transaction.Products;
            var items = new List<TransactionProductItem>(products);
            AppShell.Current.ShowPopupAsync(new TransactionDetails(items));
        });
        public Command RefreshCommand => new Command(async () =>
        {
            await RefreshTransactions();
        });

        #endregion

        #region Tasks
        public Task LoadTransactionsAsync()
        {
            var transactions = App.TransactionRepository.GetItemsWithChildren();
            Transactions.Clear();
            FilteredTransactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
            ShowAllTransactions();
            CashFLow = transactions.Where(x => x.Type == "sell" && x.IsPaid == true)
            .Sum(x => x.totalamount);
            return Task.CompletedTask;
        }
        public async Task RefreshTransactions()
        {
            IsRefreshing = true;
            await Task.Delay(1000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HomePage.Current?.ResetAllFilters();
            });
            await LoadTransactionsAsync();
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
