using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.JSON.Repositories;
using DLPMoneyTracker.Plugins.SQL.Repositories;
using DLPMoneyTracker2.BankReconciliation;
using DLPMoneyTracker2.Config.AddEditBudgetPlans;
using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.Conversion;
using DLPMoneyTracker2.LedgerEntry;
using DLPMoneyTracker2.Main.AccountSummary;
using DLPMoneyTracker2.Main.BankReconciliation;
using DLPMoneyTracker2.Main.BudgetAnalysis;
using DLPMoneyTracker2.Main.ExpenseDetail;
using DLPMoneyTracker2.Main.ExpensePlanner;
using DLPMoneyTracker2.Main.TransactionList;
using DLPMoneyTracker2.Main.UpcomingReminders;
using DLPMoneyTracker2.Main.YTD;
using DLPMoneyTracker2.Reports;
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
            var source = App.Config["AppSettings:source"].ToDataSource();
            if (source == DLPDataSource.NotSet) throw new InvalidOperationException("Unable to read configuration file");

            // Repositories
            services.AddSingleton<IDLPConfig, DLPAppConfig>();
            services.AddSingleton<SQLBankReconciliationRepository>();
            services.AddSingleton<SQLLedgerAccountRepository>();
            services.AddSingleton<SQLBudgetPlanRepository>();
            services.AddSingleton<SQLTransactionRepository>();
            services.AddSingleton<JSONLedgerAccountRepository>();
            services.AddSingleton<JSONBudgetPlanRepository>();
            services.AddSingleton<JSONTransactionRepository>();
            services.AddSingleton<JSONBankReconciliationRepository>();

            // TODO: Create a JSON config file to hold WHICH repository the system should read from
            // TODO: Update this section based on the JSON config setting

            if (source == DLPDataSource.Database)
            {
                services.AddSingleton<ILedgerAccountRepository, SQLLedgerAccountRepository>();
                services.AddSingleton<IBudgetPlanRepository, SQLBudgetPlanRepository>();
                services.AddSingleton<ITransactionRepository, SQLTransactionRepository>();
                services.AddSingleton<IBankReconciliationRepository, SQLBankReconciliationRepository>();
            }
            else if (source == DLPDataSource.JSON)
            {
                services.AddSingleton<ILedgerAccountRepository, JSONLedgerAccountRepository>();
                services.AddSingleton<IBudgetPlanRepository, JSONBudgetPlanRepository>();
                services.AddSingleton<ITransactionRepository, JSONTransactionRepository>();
                services.AddSingleton<IBankReconciliationRepository, JSONBankReconciliationRepository>();
            }
            services.AddSingleton<NotificationSystem>();

            // Use Cases
            services.AddTransient<ISaveJournalAccountUseCase, SaveJournalAccountUseCase>();
            services.AddTransient<IGetJournalAccountListByTypesUseCase, GetJournalAccountListByTypesUseCase>();
            services.AddTransient<IGetNominalAccountsUseCase, GetNominalAccountsUseCase>();
            services.AddTransient<IGetMoneyAccountsUseCase, GetMoneyAccountsUseCase>();
            services.AddTransient<IGetJournalAccountByUIDUseCase, GetJournalAccountByUIDUseCase>();
            services.AddTransient<IDeleteJournalAccountUseCase, DeleteJournalAccountUseCase>();
            services.AddTransient<IGetBudgetPlanListUseCase, GetBudgetPlanListUseCase>();
            services.AddTransient<IDeleteBudgetPlanUseCase, DeleteBudgetPlanUseCase>();
            services.AddTransient<ISaveBudgetPlanUseCase, SaveBudgetPlanUseCase>();
            services.AddTransient<ISaveTransactionUseCase, SaveTransactionUseCase>();
            services.AddTransient<IGetJournalAccountCurrentMonthBalanceUseCase, GetJournalAccountCurrentMonthBalanceUseCase>();
            services.AddTransient<IGetUpcomingPlansForAccountUseCase, GetUpcomingPlansForAccountUseCase>();
            services.AddTransient<IFindTransactionForBudgetPlanUseCase, FindTransactionForBudgetPlanUseCase>();
            services.AddTransient<IGetTransactionsBySearchUseCase, GetTransactionsBySearchUseCase>();
            services.AddTransient<IGetAllCurrentMonthBudgetPlansForAccountUseCase, GetAllCurrentMonthBudgetPlansForAccountUseCase>();
            services.AddTransient<IGetBudgetPlanListByDateRangeUseCase, GetBudgetPlanListByDateRangeUseCase>();
            services.AddTransient<IGetBankReconciliationListUseCase, GetBankReconciliationListUseCase>();
            services.AddTransient<IGetReconciliationTransactionsUseCase, GetReconciliationTransactionsUseCase>();
            services.AddTransient<ISaveReconciliationUseCase, SaveReconciliationUseCase>();
            services.AddTransient<IGetJournalAccountBalanceByMonthUseCase, GetJournalAccountBalanceByMonthUseCase>();
            services.AddTransient<IGetJournalAccountYTDUseCase, GetJournalAccountYTDUseCase>();
            services.AddTransient<IGetPaymentAccountsUseCase, GetPaymentAccountsUseCase>();
            services.AddTransient<IGetBudgetTransactionBalanceForAccountUseCase, GetBudgetTransactionBalanceForAccountUseCase>();
            services.AddTransient<IGetBudgetAnalysisDataUseCase, GetBudgetAnalysisDataUseCase>();
            services.AddTransient<IGetBudgetPlanListByType, GetBudgetPlanListByType>();
            services.AddTransient<IGetSummaryAccountListByType, GetSummaryAccountListByType>();
            services.AddTransient<IGetNextUIDUseCase, GetNextUIDUseCase>();


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
            services.AddTransient<YTDAccountDetailVM>();

            services.AddTransient<ExpensePlannerUI>();
            services.AddTransient<ExpensePlannerVM>();

            services.AddTransient<CSVMappingUI>();
            services.AddTransient<CSVMappingVM>();

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

            // Reports
            services.AddTransient<ReportBudgetAnalysis>();

            // Other models
            services.AddTransient<LedgerAccountVM>();
            services.AddTransient<MoneyAccountVM>();

            services.AddTransient<JSONConversionVM>();
            services.AddTransient<JSONConversion>();

            services.AddTransient<CSVImport>();
            services.AddTransient<CSVImportVM>();




        }
    }
}