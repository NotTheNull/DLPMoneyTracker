﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MoneyTrackerBlaze
{
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
                });

            builder.Services.AddLogging(logging =>
            {
                logging.AddFilter("Microsoft.AspNetCore.Components.WebView", LogLevel.Trace);
#if DEBUG
                logging.AddDebug();
#endif
            });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            SetupIOC.Configure(builder);


            return builder.Build();
        }
    }
}
