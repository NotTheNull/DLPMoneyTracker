using DLPMoneyTracker.Data;
using DLPMoneyTrackerWeb.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;

namespace DLPMoneyTrackerWeb
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

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSingleton<ITrackerConfig, TrackerConfig>();
            builder.Services.AddSingleton<ILedger, Ledger>();
            builder.Services.AddSingleton<IMoneyPlanner, MoneyPlanner>();
            builder.Services.AddSingleton<IBudgetTracker, BudgetTracker>();

            builder.Services.AddTransient<MoneyAccountSummaryVM>();
            builder.Services.AddTransient<MoneyAccountDetailVM>();
            

            return builder.Build();
        }
    }
}