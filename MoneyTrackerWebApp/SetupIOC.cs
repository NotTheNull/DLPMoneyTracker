using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BankReconciliation;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Reports;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.JSON.Repositories;
using DLPMoneyTracker.Plugins.SQL.Repositories;
using MoneyTrackerWebApp.Models.Core;
using DLPMoneyTracker.BusinessLogic.Factories;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;

namespace MoneyTrackerWebApp
{
    internal static class SetupIOC
    {
        public static void Configure(WebApplicationBuilder builder)
        {
            ConfigureHelpers(builder);
            ConfigureRepositories(builder);
            ConfigureFactories(builder);
            ConfigureUseCases(builder);
            ConfigureUI(builder);
        }

        private static void ConfigureHelpers(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<NotificationSystem>();
            builder.Services.AddSingleton<NotificationHelper>();
            builder.Services.AddSingleton<StorageService<IJournalAccount>>();
            builder.Services.AddSingleton<StorageService<IBudgetPlan>>();
        }

        private static void ConfigureUI(WebApplicationBuilder builder)
        {

        }

        private static void ConfigureRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IDLPConfig, BlazorConfig>();
            builder.Services.AddSingleton<SQLBankReconciliationRepository>();
            builder.Services.AddSingleton<SQLLedgerAccountRepository>();
            builder.Services.AddSingleton<SQLBudgetPlanRepository>();
            builder.Services.AddSingleton<SQLTransactionRepository>();
            builder.Services.AddSingleton<JSONLedgerAccountRepository>();
            builder.Services.AddSingleton<JSONBudgetPlanRepository>();
            builder.Services.AddSingleton<JSONTransactionRepository>();
            builder.Services.AddSingleton<JSONBankReconciliationRepository>();

            BlazorConfig config = new BlazorConfig();
            if (config.DataSource == DLPDataSource.Database)
            {
                builder.Services.AddSingleton<ILedgerAccountRepository, SQLLedgerAccountRepository>();
                builder.Services.AddSingleton<IBudgetPlanRepository, SQLBudgetPlanRepository>();
                builder.Services.AddSingleton<ITransactionRepository, SQLTransactionRepository>();
                builder.Services.AddSingleton<IBankReconciliationRepository, SQLBankReconciliationRepository>();
            }
            else if (config.DataSource == DLPDataSource.JSON)
            {
                builder.Services.AddSingleton<ILedgerAccountRepository, JSONLedgerAccountRepository>();
                builder.Services.AddSingleton<IBudgetPlanRepository, JSONBudgetPlanRepository>();
                builder.Services.AddSingleton<ITransactionRepository, JSONTransactionRepository>();
                builder.Services.AddSingleton<IBankReconciliationRepository, JSONBankReconciliationRepository>();
            }
        }

        private static void ConfigureUseCases(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ISaveJournalAccountUseCase, SaveJournalAccountUseCase>();
            builder.Services.AddTransient<IGetJournalAccountListByTypesUseCase, GetJournalAccountListByTypesUseCase>();
            builder.Services.AddTransient<IGetNominalAccountsUseCase, GetNominalAccountsUseCase>();
            builder.Services.AddTransient<IGetMoneyAccountsUseCase, GetMoneyAccountsUseCase>();
            builder.Services.AddTransient<IGetJournalAccountByUIDUseCase, GetJournalAccountByUIDUseCase>();
            builder.Services.AddTransient<IDeleteJournalAccountUseCase, DeleteJournalAccountUseCase>();
            builder.Services.AddTransient<IGetBudgetPlanListUseCase, GetBudgetPlanListUseCase>();
            builder.Services.AddTransient<IDeleteBudgetPlanUseCase, DeleteBudgetPlanUseCase>();
            builder.Services.AddTransient<ISaveBudgetPlanUseCase, SaveBudgetPlanUseCase>();
            builder.Services.AddTransient<ISaveTransactionUseCase, SaveTransactionUseCase>();
            builder.Services.AddTransient<IGetJournalAccountCurrentMonthBalanceUseCase, GetJournalAccountCurrentMonthBalanceUseCase>();
            builder.Services.AddTransient<IGetUpcomingPlansForAccountUseCase, GetUpcomingPlansForAccountUseCase>();
            builder.Services.AddTransient<IFindTransactionForBudgetPlanUseCase, FindTransactionForBudgetPlanUseCase>();
            builder.Services.AddTransient<IGetTransactionsBySearchUseCase, GetTransactionsBySearchUseCase>();
            builder.Services.AddTransient<IGetAllCurrentMonthBudgetPlansForAccountUseCase, GetAllCurrentMonthBudgetPlansForAccountUseCase>();
            builder.Services.AddTransient<IGetBudgetPlanListByDateRangeUseCase, GetBudgetPlanListByDateRangeUseCase>();
            builder.Services.AddTransient<IGetBankReconciliationListUseCase, GetBankReconciliationListUseCase>();
            builder.Services.AddTransient<IGetReconciliationTransactionsUseCase, GetReconciliationTransactionsUseCase>();
            builder.Services.AddTransient<ISaveReconciliationUseCase, SaveReconciliationUseCase>();
            builder.Services.AddTransient<IGetJournalAccountBalanceByMonthUseCase, GetJournalAccountBalanceByMonthUseCase>();
            builder.Services.AddTransient<IGetJournalAccountYTDUseCase, GetJournalAccountYTDUseCase>();
            builder.Services.AddTransient<IGetPaymentAccountsUseCase, GetPaymentAccountsUseCase>();
            builder.Services.AddTransient<IGetBudgetTransactionBalanceForAccountUseCase, GetBudgetTransactionBalanceForAccountUseCase>();
            builder.Services.AddTransient<IGetBudgetAnalysisDataUseCase, GetBudgetAnalysisDataUseCase>();
            builder.Services.AddTransient<IGetBudgetPlanListByType, GetBudgetPlanListByType>();
            builder.Services.AddTransient<IGetSummaryAccountListByType, GetSummaryAccountListByType>();
            builder.Services.AddTransient<IGetNextUIDUseCase, GetNextUIDUseCase>();
        }

        private static void ConfigureFactories(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<JournalAccountFactory>();
            builder.Services.AddTransient<BudgetPlanFactory>();
            builder.Services.AddTransient<ScheduleRecurrenceFactory>();
        }
    }
}
