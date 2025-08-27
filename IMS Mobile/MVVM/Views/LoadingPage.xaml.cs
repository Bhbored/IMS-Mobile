using IMS_Mobile.MVVM.ViewModels;


namespace IMS_Mobile.MVVM.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LoadingViewModel viewModel)
        {
            await viewModel.CheckAuthenticationAsync();
        }
    }
}