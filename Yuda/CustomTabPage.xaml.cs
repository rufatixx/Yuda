
namespace yuda;

using System.Threading.Tasks;
using Sharpnado.Tabs;
public partial class CustomTabPage : ContentPage
{
    MainPage mainPage = new MainPage();
  
    AdvicePage advicePage = new AdvicePage();
    public CustomTabPage()
    {
        InitializeComponent();

      
       
        // Select the first tab by setting its index to 0
        tabView.SelectedIndex = 0;
        // Update the ContentPresenter with the content of the selected tab on the UI thread
        Dispatcher.Dispatch(async () =>
        {
            TabContent.Content = await GetContentForTabAsync(Convert.ToInt32(tabView.SelectedIndex));
        });
        //Device.BeginInvokeOnMainThread(() =>
        //{
        //    // Update the ContentPresenter with the content of the selected tab
        //    TabContent.Content = GetContentForTab(Convert.ToInt32(tabView.SelectedIndex));
        //});



        tabView.SelectedTabIndexChanged += async (sender, args) =>
        {
            var selectedTabIndex = args.SelectedPosition;
            // Update the ContentPresenter with the content of the selected tab
            TabContent.Content = await GetContentForTabAsync(Convert.ToInt32(selectedTabIndex));
        };

    }
    private async Task<View> GetContentForTabAsync(int tabIndex)
    {
        // Return the content for the selected tab
        switch (tabIndex)
        {
            case 0:
                return mainPage;
            case 1:
               
                return new MapPage();
            case 2:
                return advicePage;
            default:
                return null;
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();


        AppShell.SetNavBarIsVisible(this, false);
    }
}
