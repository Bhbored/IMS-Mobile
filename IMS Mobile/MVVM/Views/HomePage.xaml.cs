

using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.Popups;

namespace IMS_Mobile.MVVM.Views;

public partial class HomePage : ContentPage
{
	private static readonly IList<string> ActiveFilter = new List<string> { "ActiveFilterButtonStyle" };
	private static readonly IList<string> InActiveFilter = new List<string> { "FilterButtonStyle" };
	public HomePage(HomeVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
		var btn = sender as Button;

		ResetAllFilters();

		switch (btn.AutomationId)
		{
			case "filter1":
				Filter11.StyleClass = ActiveFilter;
				break;
			case "filter2":
				Filter22.StyleClass = ActiveFilter;
				break;
			case "filter3":
				Filter33.StyleClass = ActiveFilter;
				break;
			
		}
	}

	private void ResetAllFilters()
	{
		Filter11.StyleClass = InActiveFilter;
		Filter22.StyleClass = InActiveFilter;
		Filter33.StyleClass = InActiveFilter;
	}

    private void Button_Clicked_1(object sender, EventArgs e)
    {
		if(BindingContext is  HomeVM vm)
		{
            AppShell.Current.ShowPopupAsync(new DatePickerPopup(vm));
        }
			
    }
}