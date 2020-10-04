using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.DataEntry.AddEditCategories;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;
using DLPMoneyTracker.DataEntry.AddTransaction;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using DLPMoneyTracker.DataEntry.ScheduleRecurrence;
using DLPMoneyTracker.ReportViews;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

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
            services.AddSingleton<ILedger, Ledger>();
            services.AddSingleton<IBudgetPlanner, BudgetPlanner>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MoneyAccountsOverview>();
            services.AddSingleton<MoneyAccountOverviewVM>();

            services.AddTransient<AddEditMoneyAccount>();
            services.AddTransient<AddEditMoneyAccountVM>();

            services.AddTransient<AddEditCategory>();
            services.AddTransient<AddEditCategoryVM>();

            services.AddTransient<AddIncomeView>();
            services.AddTransient<AddIncomeVM>();

            services.AddTransient<AddExpenseView>();
            services.AddTransient<AddExpenseVM>();

            services.AddTransient<AddDebtPayment>();
            services.AddTransient<AddDebtPaymentVM>();

            services.AddTransient<TransferMoney>();
            services.AddTransient<TransferMoneyVM>();

            services.AddTransient<LedgerDetailView>();
            services.AddTransient<LedgerDetailVM>();

            services.AddTransient<RecurrenceEditorView>();
            services.AddTransient<RecurrenceEditorVM>();

            services.AddTransient<BudgetPlannerView>();
            services.AddTransient<BudgetPlannerVM>();
            
        }
    }

    public static class StringExtensions
    {
        public static string ToDisplayText(this MoneyAccountType actType)
        {
            switch(actType)
            {
                case MoneyAccountType.Checking:
                    return "Checking";
                case MoneyAccountType.CreditCard:
                    return "Credit Card";
                case MoneyAccountType.Loan:
                    return "Loan";
                case MoneyAccountType.Savings:
                    return "Savings";
                default:
                    return "*N/A*";
            }
        }


        public static string ToDisplayText(this CategoryType catType)
        {
            switch(catType)
            {
                case CategoryType.Expense:
                    return "Expense";
                case CategoryType.Income:
                    return "Income";
                case CategoryType.UntrackedAdjustment:
                    return "Adjustment";
                default:
                    return "*N/A*";
            }
        }

        public static string ToDisplayText(this RecurrenceFrequency recurType)
        {
            switch(recurType)
            {
                case RecurrenceFrequency.Annual:
                    return "Annual";
                case RecurrenceFrequency.SemiAnnual:
                    return "Semi-Annual";
                case RecurrenceFrequency.Monthly:
                    return "Monthly";
                default:
                    return "*N/A*";
            }
        }
    }


}
