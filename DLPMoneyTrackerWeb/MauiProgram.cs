﻿using DLPMoneyTracker.Data;
using DLPMoneyTrackerWeb.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DLPMoneyTrackerWeb
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            SetupSerilog();

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
            builder.Services.AddLogging(logging => logging.AddSerilog(dispose: true));
            builder.Services.AddSingleton<ITrackerConfig, TrackerConfig>();
            builder.Services.AddSingleton<IJournal, DLPJournal>();
            builder.Services.AddSingleton<IJournalPlanner, JournalPlanner>();

            builder.Services.AddSingleton<IEditJournalAccountService, EditJournalAccountService>();
            builder.Services.AddSingleton<IEditBudgetPlanService, EditBudgetPlanService>();



            return builder.Build();
        }

        private static void SetupSerilog()
        {
            var flushInterval = new TimeSpan(0, 0, 1);
            var file = Path.Combine(FileSystem.AppDataDirectory, "DLPMoneyTracker.log");
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
            .CreateLogger();
        }
    }
}