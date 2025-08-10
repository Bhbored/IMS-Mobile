using IMS_Mobile.MVVM.Views;

namespace IMS_Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("ContactsPage", typeof(ContactsPage));
            Routing.RegisterRoute("InventoryPage", typeof(InventoryPage));
            Routing.RegisterRoute("ReportsPage", typeof(ReportsPage)); 
            Routing.RegisterRoute("ContactDetailsPage", typeof(ContactDetailsPage));
        }
        //await Shell.Current.GoToAsync("//products");
    }
}
