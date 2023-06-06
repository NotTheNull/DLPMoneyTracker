using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DLPMoneyTracker.Data;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;

namespace DLPMoneyTracker2
{
    internal static class UICore
    {
        public static IServiceProvider DependencyHost { get; set; }

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

            services.AddSingleton<MainWindow>();

            services.AddTransient<AddEditMoneyAccount>();
            services.AddTransient<AddEditMoneyAccountVM>();
            

        }

    }
}
