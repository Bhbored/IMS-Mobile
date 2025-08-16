using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using System.Collections.ObjectModel;
namespace IMS_Mobile.Popups;

public partial class TransactionDetails : Popup
{
	public ObservableCollection<TransactionProductItem> Products { get; set; } = new ObservableCollection<TransactionProductItem>();

	public TransactionDetails(List<TransactionProductItem> products)
	{
		InitializeComponent();
        Fill(products);
        BindingContext = this;
	}
	private void OnCloseClicked(object sender, EventArgs e)
	{
		CloseAsync();
	}
	public void Fill(List<TransactionProductItem> products)
	{
        foreach (var item in products)
        {
            Products.Add(item);
        }
    }
}