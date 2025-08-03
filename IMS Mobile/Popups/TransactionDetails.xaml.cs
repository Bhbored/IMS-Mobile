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
		Products = new ObservableCollection<TransactionProductItem>(products);
		BindingContext = this;
	}
	private void OnCloseClicked(object sender, EventArgs e)
	{
		CloseAsync();
	}
}