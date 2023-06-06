using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DLPMoneyTracker.Data;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Main.AccountSummary;

namespace DLPMoneyTracker2
{
    internal static class UICore
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static IServiceProvider DependencyHost { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static void Init()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            DependencyHost = services.BuildServiceProvider();
        }


        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ITrackerConfig, TrackerConfig>();
            services.AddSingleton<IJournal, DLPJournal>();

#pragma warning disable CS0612 // Type or member is obsolete
            // Keep until conversion is done
            services.AddSingleton<ILedger, Ledger>();
#pragma warning restore CS0612 // Type or member is obsolete

            // Main UI
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MoneyAccountOverview>();
            services.AddSingleton<MoneyAccountOverviewVM>();
            services.AddTransient<MoneyAccountSummary>();
            services.AddTransient<MoneyAccountSummaryVM>();
            
            // Config
            services.AddTransient<AddEditMoneyAccount>();
            services.AddTransient<AddEditMoneyAccountVM>();
            services.AddTransient<AddEditLedgerAccount>();
            services.AddTransient<AddEditLedgerAccountVM>();

        }

    }
}
