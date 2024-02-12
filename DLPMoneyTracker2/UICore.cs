using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.BankReconciliation;
using DLPMoneyTracker2.BankReconciliation;
using DLPMoneyTracker2.Config.AddEditBudgetPlans;
using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.LedgerEntry;
using DLPMoneyTracker2.Main.AccountSummary;
using DLPMoneyTracker2.Main.BankReconciliation;
using DLPMoneyTracker2.Main.BudgetAnalysis;
using DLPMoneyTracker2.Main.TransactionList;
using DLPMoneyTracker2.Main.UpcomingReminders;
using DLPMoneyTracker2.Main.YTD;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            services.AddSingleton<IJournalPlanner, JournalPlanner>();
            services.AddSingleton<IBRManager, BRManager>();

            //#pragma warning disable CS0612 // Type or member is obsolete
            //#pragma warning disable CS0618 // Type or member is obsolete
            //            // Keep until conversion is done
            //            services.AddSingleton<ILedger, Ledger>();
            //            services.AddSingleton<IMoneyPlanner, MoneyPlanner>();
            //#pragma warning restore CS0618 // Type or member is obsolete
            //#pragma warning restore CS0612 // Type or member is obsolete

            // Main UI
            services.AddSingleton<MainWindow>();

            services.AddSingleton<MoneyAccountOverview>();
            services.AddSingleton<MoneyAccountOverviewVM>();
            services.AddTransient<MoneyAccountSummary>();
            services.AddTransient<MoneyAccountSummaryVM>();

            services.AddSingleton<CurrentMonthBudget>();
            services.AddSingleton<CurrentMonthBudgetVM>();
            services.AddTransient<JournalAccountBudgetVM>();

            services.AddSingleton<TransactionDetail>();
            services.AddSingleton<TransactionDetailVM>();

            services.AddTransient<AccountTransactionDetailVM>();

            services.AddTransient<RemindersUI>();
            services.AddTransient<RemindersVM>();

            services.AddTransient<YearToDateUI>();
            services.AddTransient<YearToDateVM>();

            // Ledger
            services.AddTransient<IncomeJournalEntryVM>();
            services.AddTransient<ExpenseJournalEntryVM>();
            services.AddTransient<DebtPaymentJournalEntryVM>();
            services.AddTransient<TransferJournalEntryVM>();
            services.AddTransient<CorrectionJournalEntryVM>();
            services.AddTransient<DebtAdjustmentJournalEntryVM>();

            // Config
            services.AddTransient<AddEditMoneyAccount>();
            services.AddTransient<AddEditMoneyAccountVM>();
            services.AddTransient<AddEditLedgerAccount>();
            services.AddTransient<AddEditLedgerAccountVM>();
            services.AddTransient<AddEditBudgetPlan>();
            services.AddTransient<AddEditBudgetPlanVM>();
            services.AddTransient<RecurrenceEditor>();
            services.AddTransient<RecurrenceEditorVM>();

            // Bank Reconciliation
            services.AddTransient<BankReconciliationVM>();
            services.AddTransient<BankReconciliationUI>();
            services.AddTransient<BankReconciliationListingVM>();
            services.AddTransient<BankReconciliationListingUI>();
        }
    }
}