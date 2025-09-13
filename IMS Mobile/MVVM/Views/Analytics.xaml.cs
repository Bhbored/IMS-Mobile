using IMS_Mobile.MVVM.ViewModels;
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.Views;

public partial class Analytics : ContentPage
{
    public Analytics(AnalyticsVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AnalyticsVM viewModel)
        {
            _ = Task.Run(async () =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await viewModel.RefreshDataAsync();
                });
            });
        }
    }
}