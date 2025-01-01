using CommunityToolkit.Maui;
using Sharpnado.Tabs;

namespace yuda;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSharpnadoTabs(loggerEnable: false)
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
           
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
				fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
				fonts.AddFont("Poppins-Semibold.ttf", "PoppinsSemibold");
				fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
			})
             ;

		return builder.Build();
	}
}

