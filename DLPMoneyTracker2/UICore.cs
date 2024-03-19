using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.JSON.Repositories;
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

            // TODO: Swap these to the SQL repository when ready
            // Repositories
            services.AddSingleton<ILedgerAccountRepository, JSONLedgerAccountRepository>();
            services.AddSingleton<IBudgetPlanRepository, JSONBudgetPlanRepository>();
            services.AddSingleton<ITransactionRepository, JSONTransactionRepository>();
            services.AddSingleton<IBankReconciliationRepository, JSONBankReconciliationRepository>();
            services.AddSingleton<NotificationSystem>();

            // Use Cases
            services.AddTransient<ISaveJournalAccountUseCase, SaveJournalAccountUseCase>();
            services.AddTransient<IGetJournalAccountListByTypesUseCase, GetJournalAccountListByTypesUseCase>();
            services.AddTransient<IGetLedgerAccountsUseCase, GetLedgerAccountsUseCase>();
            services.AddTransient<IGetMoneyAccountsUseCase, GetMoneyAccountsUseCase>();
            services.AddTransient<IGetJournalAccountByUIDUseCase, GetJournalAccountByUIDUseCase>();
            services.AddTransient<IDeleteJournalAccountUseCase, DeleteJournalAccountUseCase>();
            services.AddTransient<IGetBudgetPlanListUseCase, GetBudgetPlanListUseCase>();
            services.AddTransient<IDeleteBudgetPlanUseCase, DeleteBudgetPlanUseCase>();
            services.AddTransient<ISaveBudgetPlanUseCase, SaveBudgetPlanUseCase>();
            services.AddTransient<ISaveTransactionUseCase, SaveTransactionUseCase>();
            services.AddTransient<IGetJournalAccountBalanceUseCase, GetJournalAccountBalanceUseCase>();
            services.AddTransient<IGetUpcomingPlansForAccountUseCase, GetUpcomingPlansForAccountUseCase>();
            services.AddTransient<IFindTransactionForBudgetPlanUseCase, FindTransactionForBudgetPlanUseCase>();

            // Factories
            services.AddTransient<JournalAccountFactory>();
            services.AddTransient<BudgetPlanFactory>();
            services.AddTransient<ScheduleRecurrenceFactory>();
            

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