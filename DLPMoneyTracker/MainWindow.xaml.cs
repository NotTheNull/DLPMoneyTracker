using DLPMoneyTracker.DataEntry.AddEditCategories;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;
using DLPMoneyTracker.DataEntry.AddTransaction;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using DLPMoneyTracker.HTMLReports;
using DLPMoneyTracker.HTMLReports.MonthlyExpense;
using DLPMoneyTracker.ReportViews;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace DLPMoneyTracker
{
    // TODO: Add ability to backup/close Ledger and transfer balances to NEW ledger
    // TODO: Add Monthly Expense Report by Category

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MoneyAccountsOverview _uiAccountSummary;
        private LedgerDetailView _uiFullLedger;
        private MonthlyBudgetPlannerView _uiBudgetPlanner;

        public MainWindow(MoneyAccountsOverview uiAccountSummary, LedgerDetailView uiFullLedger, MonthlyBudgetPlannerView uiBudgetPlanner)
        {
            InitializeComponent();
            panelAccountSummary.Children.Add(uiAccountSummary);
            _uiAccountSummary = uiAccountSummary;

            uiFullLedger.ShowFullLedgerDetail();

            panelTransactions.Children.Add(uiFullLedger);
            _uiFullLedger = uiFullLedger;

            panelBudget.Children.Add(uiBudgetPlanner);
            _uiBudgetPlanner = uiBudgetPlanner;
        }

        private void Exit()
        {
            // Possible that I'll need to ask before exit at some point
            // Also making this a separate method in case I have multiple events to trigger it
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }

        private void MenuItemEditMoneyAccount_Click(object sender, RoutedEventArgs e)
        {
            AddEditMoneyAccount uiAddEditMoneyAccount = UICore.DependencyHost.GetService<AddEditMoneyAccount>();
            uiAddEditMoneyAccount.Show();
        }

        private void MenuItemEditCategories_Click(object sender, RoutedEventArgs e)
        {
            AddEditCategory uiAddEditCategory = UICore.DependencyHost.GetService<AddEditCategory>();
            uiAddEditCategory.Show();
        }

        private void MenuItemAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseView uiAddTransaction = UICore.DependencyHost.GetService<AddExpenseView>();
            uiAddTransaction.Show();
        }

        private void MenuItemAddDebtPayment_Click(object sender, RoutedEventArgs e)
        {
            AddDebtPayment uiAddDeptPayment = UICore.DependencyHost.GetService<AddDebtPayment>();
            uiAddDeptPayment.Show();
        }

        private void MenuItemTransferMoney_Click(object sender, RoutedEventArgs e)
        {
            TransferMoney uiTransferMoney = UICore.DependencyHost.GetService<TransferMoney>();
            uiTransferMoney.Show();
        }

        private void MenuItemAddIncome_Click(object sender, RoutedEventArgs e)
        {
            AddIncomeView uiAddIncome = UICore.DependencyHost.GetService<AddIncomeView>();
            uiAddIncome.Show();
        }

        private void MenuItemModifyBudget_Click(object sender, RoutedEventArgs e)
        {
            MoneyPlannerView uiModifyBudget = UICore.DependencyHost.GetService<MoneyPlannerView>();
            uiModifyBudget.Show();
        }

        private void MenuItemMonthlyExpenseReport_Click(object sender, RoutedEventArgs e)
        {
            MonthlyExpenseReportVM viewModel = UICore.DependencyHost.GetService<MonthlyExpenseReportVM>();
            WebReportViewer uiReport = new WebReportViewer(viewModel);
            uiReport.Show();
        }

        private void MenuItemCreateNewYear_Click(object sender, RoutedEventArgs e)
        {
            // Make sure that it's near the end of the year
            DateTime nextYear = new DateTime(DateTime.Today.Year + 1, 1, 1);
            TimeSpan timeToEndOfYear = nextYear - DateTime.Today;
            if (timeToEndOfYear.Days > 7 || timeToEndOfYear.Days < 0) return;

            DLPMoneyTracker.Data.NewYearBuilder.Execute(nextYear.Year);
        }
    }
}