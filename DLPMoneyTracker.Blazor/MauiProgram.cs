using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace DLPMoneyTracker.Blazor;
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

        builder.Logging.ClearProviders();
        builder.Logging.AddNLog();
        LogManager.Setup().LoadConfigurationFromAssemblyResource(Assembly.GetExecutingAssembly());
        
        builder.Services.ConfigureCoreServices();
        
        return builder.Build();
    }

    

    private static void ConfigureCoreServices(this IServiceCollection services)
    {

        services.AddFluentUIComponents(options =>
        {
            options.ValidateClassNames = false;
        });


        services.AddMauiBlazorWebView();
        services.AddFluentUIComponents();
        services.AddBlazorBootstrap();


#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
    }
}
