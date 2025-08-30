using IMS_Mobile.MVVM.ViewModels;


namespace IMS_Mobile.MVVM.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as LoadingViewModel;
        _ = Task.Run(async () =>
        {
            await Task.Delay(100); 
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await vm.CheckAuthenticationAsync();
            });
        });
    }
    
}