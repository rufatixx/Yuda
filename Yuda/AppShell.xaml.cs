namespace yuda;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        bool onboardingSeen = Preferences.Default.Get("onboardingSeen", false);
		if (onboardingSeen)
		{

            var tabPage = new CustomTabPage();
            shellContent.ContentTemplate = new DataTemplate(() => tabPage);


        }
        else {
            var onboardingPage = new OnboardingPage();
            shellContent.ContentTemplate = new DataTemplate(() => onboardingPage);
           
        }

       

    }
}

