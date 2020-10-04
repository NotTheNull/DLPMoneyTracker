using DLPMoneyTracker.DataEntry.AddEditCategories;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;
using DLPMoneyTracker.DataEntry.AddTransaction;
using DLPMoneyTracker.DataEntry.BudgetPlanner;
using DLPMoneyTracker.ReportViews;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace DLPMoneyTracker
{
    // TODO: Add UI to maintain a Budget with each Transaction Category
    // TODO: Add UI to plan for upcoming Bills
    // TODO: Create VM and UI to display summary of Categories that include current totals and budget expectations
    // TODO: Modify Main Window to show tabbed interface.  Main tab is summary of Money Accounts.  Second tab would be summary of Categories. Third tab could be Full Ledger.



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MoneyAccountsOverview _uiAccountSummary;
        private LedgerDetailView _uiFullLedger;

        public MainWindow(MoneyAccountsOverview uiAccountSummary, LedgerDetailView uiFullLedger)
        {
            InitializeComponent();
            panelAccountSummary.Children.Add(uiAccountSummary);
            _uiAccountSummary = uiAccountSummary;

            uiFullLedger.ShowFullLedgerDetail();
            
            panelTransactions.Children.Add(uiFullLedger);
            _uiFullLedger = uiFullLedger;
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
            BudgetPlannerView uiModifyBudget = UICore.DependencyHost.GetService<BudgetPlannerView>();
            uiModifyBudget.Show();
        }
    }
}
