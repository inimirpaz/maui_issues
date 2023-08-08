using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using TestMauiEffect.Controls;

namespace TestMauiEffect;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseMauiCompatibility()
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(ExtendedEntry), typeof(TestMauiEffect.Platforms.Android.Renderers.ExtendedEntryRenderer));
#elif IOS || MACCATALYST
				handlers.AddHandler(typeof(ExtendedEntry), typeof(TestMauiEffect.Platforms.iOS.Renderers.ExtendedEntryRenderer));
#endif
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

