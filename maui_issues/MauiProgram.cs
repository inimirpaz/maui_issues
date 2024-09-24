﻿using Microsoft.Extensions.Logging;
using UraniumUI;

namespace maui_issues;

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
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            })
            .UseUraniumUI()
            .UseUraniumUIMaterial();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}