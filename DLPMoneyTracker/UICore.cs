using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.DataEntry.AddEditCategories;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;
using DLPMoneyTracker.DataEntry.AddTransaction;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using DLPMoneyTracker.DataEntry.ScheduleRecurrence;
using DLPMoneyTracker.ReportViews;
using DLPMoneyTracker.ReportViews.HistoricalViews;
using DLPMoneyTracker.ReportViews.JournalViews;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLPMoneyTracker
{
    public static class UICore
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

#pragma warning disable CS0612 // Type or member is obsolete
            // Keep until conversion is done
            services.AddSingleton<ILedger, Ledger>();
#pragma warning restore CS0612 // Type or member is obsolete

            services.AddSingleton<IJournal, DLPJournal>();
            services.AddSingleton<IMoneyPlanner, MoneyPlanner>();
            services.AddSingleton<IBudgetTracker, BudgetTracker>();
            services.AddSingleton<MainWindow>();

            services.AddTransient<JournalDetailView>();
            services.AddTransient<JournalDetailVM>();

            //services.AddSingleton<MoneyAccountsOverview>();
            //services.AddSingleton<MoneyAccountOverviewVM>();

            //services.AddTransient<AddEditMoneyAccount>();
            //services.AddTransient<AddEditMoneyAccountVM>();

            //services.AddTransient<AddEditCategory>();
            //services.AddTransient<AddEditCategoryVM>();

            //services.AddTransient<AddIncomeView>();
            //services.AddTransient<AddIncomeVM>();

            //services.AddTransient<AddExpenseView>();
            //services.AddTransient<AddExpenseVM>();

            //services.AddTransient<AddDebtPayment>();
            //services.AddTransient<AddDebtPaymentVM>();

            //services.AddTransient<TransferMoney>();
            //services.AddTransient<TransferMoneyVM>();

            //services.AddTransient<LedgerDetailView>();

            //services.AddTransient<RecurrenceEditorView>();
            //services.AddTransient<RecurrenceEditorVM>();

            //services.AddTransient<MoneyPlannerView>();
            //services.AddTransient<MoneyPlannerVM>();

            //services.AddTransient<MonthlyBudgetPlannerView>();
            //services.AddTransient<MonthlyBudgetPlannerVM>();

            //services.AddTransient<MonthlyHistoricalView>();
            //services.AddTransient<MonthlyHistoricalVM>();
        }

        public static string GetResourceText(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
#if DEBUG
            var resourceList = assembly.GetManifestResourceNames().ToList();
#endif
            string resourcePath = assembly.GetManifestResourceNames().Single(s => s.EndsWith(resourceName));
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

}