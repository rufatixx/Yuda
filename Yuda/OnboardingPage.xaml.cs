namespace yuda;

public partial class OnboardingPage : ContentPage
{
	public OnboardingPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();


        AppShell.SetNavBarIsVisible(this, false);
    }
    private async void NextButtonCLickedAsync(object sender, EventArgs e)
    {
        Preferences.Default.Set("onboardingSeen", true);
        await Navigation.PushAsync(new CustomTabPage());

    }
}
