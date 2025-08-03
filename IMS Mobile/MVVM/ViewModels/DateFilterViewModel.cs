using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class DateFilterViewModel : INotifyPropertyChanged
    {

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                UpdateSummary();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                UpdateSummary();
            }
        }

        public string FilterSummary
        {
            get => _filterSummary;
            set
            {
                _filterSummary = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startDate = DateTime.Now.AddDays(-7);
        private DateTime _endDate = DateTime.Now;
        private string _filterSummary = "Last 7 days";
        private readonly HomeVM _homeVM;

        public DateFilterViewModel(HomeVM homeVM)
        {
            _homeVM = homeVM;
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            var days = (EndDate - StartDate).Days;
            FilterSummary = $"Showing transactions from {StartDate:MMM dd} to {EndDate:MMM dd} ({days + 1} days)";
        }

        public async Task ApplyFilter()
        {
            try
            {
                if (_homeVM != null)
                {
                    _homeVM.FilterByDateRange(StartDate, EndDate);
                    await ShowToast($"Filtered {_homeVM.FilteredTransactions.Count} transactions");
                }
                else
                {
                    await ShowToast("HomeVM is null");
                }
            }
            catch (Exception ex)
            {
                await ShowToast($"Error: {ex.Message}");
            }
        }

        private async Task ShowToast(string message)
        {
            try
            {
                await Toast.Make(message, duration: ToastDuration.Short).Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
