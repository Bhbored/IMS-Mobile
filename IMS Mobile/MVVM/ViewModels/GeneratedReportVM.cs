using PropertyChanged;
using Syncfusion.Maui.Calendar;
using Syncfusion.Maui.DataGrid.Exporting;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class GeneratedReportVM : INotifyPropertyChanged
    {


        #region Properties

        public ObservableCollection<Transaction> AllTransactions
        {
            get => allTransactions;
            set
            {
                allTransactions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Transaction> FilteredTransactions
        {
            get => filteredTransactions;
            set
            {
                filteredTransactions = value;
                OnPropertyChanged();
            }
        }
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                OnPropertyChanged();
                OnSelectedDateChanged();
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged();
            }
        }

        public CalendarDateRange SelectedDateRange
        {
            get => selectedDateRange;
            set
            {
                selectedDateRange = value;
                OnPropertyChanged();
                if (value != null && value.StartDate.HasValue && value.EndDate.HasValue)
                {
                    StartDate = value.StartDate.Value;
                    EndDate = value.EndDate.Value;
                }
            }
        }
        public string SelectedFilter { get; set; } = "1 Month";
        public bool IsCustomDateRangeVisible { get; set; } = false;
        public bool IsDayFilterActive { get; set; } = false;
        public bool IsWeekFilterActive { get; set; } = false;
        public bool IsMonthFilterActive { get; set; } = true;
        public bool IsCustomFilterActive { get; set; } = false;

        public Syncfusion.Maui.DataGrid.SfDataGrid DataGrid { get; set; }

        #endregion

        #region Commands

        public ICommand FilterByDay => new Command(() =>
        {
            SelectedFilter = "1 Day";
            IsCustomDateRangeVisible = false;
            StartDate = SelectedDate;
            EndDate = SelectedDate;

            // Update active states
            IsDayFilterActive = true;
            IsWeekFilterActive = false;
            IsMonthFilterActive = false;
            IsCustomFilterActive = false;

            ApplyDateFilter();
        });

        public ICommand FilterByWeek => new Command(() =>
        {
            SelectedFilter = "1 Week";
            IsCustomDateRangeVisible = false;
            StartDate = SelectedDate.AddDays(-7);
            EndDate = SelectedDate;
            IsDayFilterActive = false;
            IsWeekFilterActive = true;
            IsMonthFilterActive = false;
            IsCustomFilterActive = false;

            ApplyDateFilter();
        });

        public ICommand FilterByMonth => new Command(() =>
        {
            SelectedFilter = "1 Month";
            IsCustomDateRangeVisible = false;
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            IsDayFilterActive = false;
            IsWeekFilterActive = false;
            IsMonthFilterActive = true;
            IsCustomFilterActive = false;

            ApplyDateFilter();
        });

        public ICommand ToggleCustomDateRange => new Command(() =>
        {
            IsCustomDateRangeVisible = !IsCustomDateRangeVisible;
            if (IsCustomDateRangeVisible)
            {
                SelectedFilter = "Custom Range";
            }
        });

        public ICommand ApplyCustomDateRange => new Command(() =>
        {
            if (SelectedDateRange != null && SelectedDateRange.StartDate.HasValue && SelectedDateRange.EndDate.HasValue)
            {
                StartDate = SelectedDateRange.StartDate.Value;
                EndDate = SelectedDateRange.EndDate.Value;
                SelectedFilter = "Custom Range";
                IsDayFilterActive = false;
                IsWeekFilterActive = false;
                IsMonthFilterActive = false;
                IsCustomFilterActive = true;
                ApplyDateFilter();
                IsCustomDateRangeVisible = false;
            }
        });

        public ICommand ResetCustomDateRange => new Command(() =>
        {
            SelectedDateRange = null;
            OnPropertyChanged(nameof(SelectedDateRange));
            var newDateRange = new CalendarDateRange(DateTime.Now.AddDays(-30), DateTime.Now);
            SelectedDateRange = newDateRange;
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            ApplyDateFilter();

        });

        public ICommand ExportToPdf => new Command(async () =>
        {
            await ExportDataGridToPdf();
        });


        #endregion

        #region Data Loading

        public async Task LoadTransactionsAsync()
        {
            var dbtransaction = App.TransactionRepository!.GetItems();
            await Task.Delay(100);
            AllTransactions.Clear();
            FilteredTransactions.Clear();
            foreach (var item in dbtransaction)
            {
                AllTransactions.Add(item);
            }
            OnPropertyChanged(nameof(AllTransactions));
            ApplyDateFilter();
        }

        #endregion

        #region Date Filtering

        private void ApplyDateFilter()
        {

            var filtered = AllTransactions
                .Where(t => t.CreatedDate.Date >= StartDate.Date && t.CreatedDate.Date <= EndDate.Date)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();

            FilteredTransactions = new ObservableCollection<Transaction>(filtered);
            OnPropertyChanged(nameof(FilteredTransactions));

        }

        public void OnSelectedDateChanged()
        {
            if (SelectedFilter == "1 Day")
            {
                StartDate = SelectedDate;
                EndDate = SelectedDate;
            }
            else if (SelectedFilter == "1 Week")
            {
                StartDate = SelectedDate.AddDays(-7);
                EndDate = SelectedDate;
            }
            ApplyDateFilter();
        }

        #endregion

        #region Export Functionality

        private async Task ExportDataGridToPdf()
        {


            if (DataGrid == null)
            {
                await Application.Current.MainPage!.DisplayAlert("Export Error", "DataGrid is not available", "OK");
                return;
            }
            var fileName = $"TransactionReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            using var stream = new MemoryStream();
            var pdfExport = new DataGridPdfExportingController();
            var option = new DataGridPdfExportingOption();
            option.CanFitAllColumnsInOnePage = true;
            var pdfDoc = pdfExport.ExportToPdf(DataGrid, option);
            pdfDoc.Save(stream);
            pdfDoc.Close(true);
            var saveService = new IMS_Mobile.Service.SaveService();
            saveService.SaveAndView(fileName, "application/pdf", stream);

        }

        #endregion

        private ObservableCollection<Transaction> filteredTransactions = new ObservableCollection<Transaction>();
        private DateTime selectedDate = DateTime.Now;
        private DateTime startDate = DateTime.Now.AddDays(-30);
        private DateTime endDate = DateTime.Now;
        private CalendarDateRange selectedDateRange = new CalendarDateRange(DateTime.Now.AddDays(-30), DateTime.Now);
        private ObservableCollection<Transaction> allTransactions = new ObservableCollection<Transaction>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
