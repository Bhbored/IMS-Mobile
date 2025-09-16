using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.Popups;
using Syncfusion.Maui.Calendar;

namespace IMS_Mobile.MVVM.Views;

public partial class GeneratedReport : ContentPage
{

    public GeneratedReport(GeneratedReportVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        if (viewModel != null)
        {
            viewModel.DataGrid = DataGrid;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GeneratedReportVM viewModel)
        {
            _ = Task.Run(() =>
            {
                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await viewModel.LoadTransactionsAsync();
                });
            });
        }
    }

    private void OnCalendarSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        if (BindingContext is GeneratedReportVM viewModel && e.NewValue is CalendarDateRange dateRange)
        {
            viewModel.SelectedDateRange = dateRange;
        }
    }

    private void FilterButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button clickedButton)
        {
            if (clickedButton.StyleClass.Contains("ActiveFilterButtonStyle"))
                return;

            FilterDay.StyleClass = new List<string> { "FilterButtonStyle" };
            FilterWeek.StyleClass = new List<string> { "FilterButtonStyle" };
            FilterMonth.StyleClass = ["FilterButtonStyle"];
            clickedButton.StyleClass = new List<string> { "ActiveFilterButtonStyle" };
        }
    }
}
