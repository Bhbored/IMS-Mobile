using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;

namespace IMS_Mobile
{
    public partial class AppShell : Shell
    {
        private readonly SupabaseAuthService _authService;

        public AppShell(SupabaseAuthService authService)
        {
            InitializeComponent();
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("ContactsPage", typeof(ContactsPage));
            Routing.RegisterRoute("InventoryPage", typeof(InventoryPage));
            Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
            Routing.RegisterRoute("ContactDetailsPage", typeof(ContactDetailsPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            _authService = authService;
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            Shell.Current.ShowPopupAsync(new LogoutConfirmationPopup(_authService));
        }


        //await Shell.Current.GoToAsync("//products");
    }
}
