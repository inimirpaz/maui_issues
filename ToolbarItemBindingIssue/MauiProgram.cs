using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace ToolbarItemBindingIssue;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCompatibility()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
			})
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddCompatibilityRenderer(typeof(SearchBar), typeof(Platforms.Android.Renderers.CustomSearchBarRenderer));
#elif IOS
#endif
            })
            ;

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}