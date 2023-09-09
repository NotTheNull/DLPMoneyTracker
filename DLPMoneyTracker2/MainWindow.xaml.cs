using DLPMoneyTracker.Data;
using DLPMoneyTracker2.Config.AddEditBudgetPlans;
using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.LedgerEntry;
using DLPMoneyTracker2.Main.AccountSummary;
using DLPMoneyTracker2.Main.BudgetAnalysis;
using DLPMoneyTracker2.Main.TransactionList;
using DLPMoneyTracker2.Main.UpcomingReminders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DLPMoneyTracker2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: Refresh the tabs' UI when they are focused/clicked on

        private readonly MoneyAccountOverview _viewMoneyAccounts;
        private readonly TransactionDetail _viewTransactions;
        private readonly CurrentMonthBudget _viewBudgetAnalysis;
        private readonly RemindersUI _viewBillReminders;

        public MainWindow(MoneyAccountOverview viewMain, CurrentMonthBudget viewBudget, TransactionDetail viewDetail, RemindersUI viewBills)
        {
            InitializeComponent();

            panelAccountSummary.Children.Add(viewMain);
            _viewMoneyAccounts = viewMain;

            panelBudget.Children.Add(viewBudget);
            _viewBudgetAnalysis = viewBudget;

            panelBillReminers.Children.Add(viewBills);
            _viewBillReminders = viewBills;

            panelTransactions.Children.Add(viewDetail);
            _viewTransactions = viewDetail;

        }

        private void Exit()
        {
            // Possible that I'll need to ask before exit at some point
            // Also making this a separate method in case I have multiple events to trigger it
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }

        private void MenuEditMoneyAccounts_Click(object sender, RoutedEventArgs e)
        {
            AddEditMoneyAccount window = UICore.DependencyHost.GetRequiredService<AddEditMoneyAccount>();
            window.Show();            
        }

        private void MenuEditLedgerAccounts_Click(object sender, RoutedEventArgs e)
        {
            AddEditLedgerAccount window = UICore.DependencyHost.GetRequiredService<AddEditLedgerAccount>();
            window.Show();
        }

        private void MenuRecordIncome_Click(object sender, RoutedEventArgs e)
        {
            IncomeJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<IncomeJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }

        private void MenuRecordExpense_Click(object sender, RoutedEventArgs e)
        {
            ExpenseJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<ExpenseJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }

        private void MenuRecordLiabilityPayment_Click(object sender, RoutedEventArgs e)
        {
            DebtPaymentJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<DebtPaymentJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }

        private void MenuRecordBankTransfer_Click(object sender, RoutedEventArgs e)
        {
            TransferJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<TransferJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }

        private void MenuAccountCorrection_Click(object sender, RoutedEventArgs e)
        {
            CorrectionJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<CorrectionJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }

        private void MenuEditBudgetPlans_Click(object sender, RoutedEventArgs e)
        {
            AddEditBudgetPlan window = UICore.DependencyHost.GetRequiredService<AddEditBudgetPlan>();
            window.Show();
        }

        /// <summary>
        /// If its December, build the New year.  If it's January, rebuild the curren tyear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNewYearSetup_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Today.Month == 12) NewYearBuilder.SetupNewYear();
            else if (DateTime.Today.Month == 1) NewYearBuilder.RebuildCurrentYear();

        }

        private void MenuRecordDebtAdjustment_Click(object sender, RoutedEventArgs e)
        {
            DebtAdjustmentJournalEntryVM viewModel = UICore.DependencyHost.GetRequiredService<DebtAdjustmentJournalEntryVM>();
            RecordJournalEntry window = new RecordJournalEntry(viewModel);
            window.Show();
        }
    }
}
