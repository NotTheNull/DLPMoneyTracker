using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.Ledger;
using DLPMoneyTracker2.Main.AccountSummary;
using DLPMoneyTracker2.Main.BudgetAnalysis;
using DLPMoneyTracker2.Main.TransactionList;
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
        private readonly MoneyAccountOverview _viewMoneyAccounts;
        private readonly TransactionDetail _viewTransactions;
        private readonly CurrentMonthBudget _viewBudgetAnalysis;

        public MainWindow(MoneyAccountOverview viewMain, CurrentMonthBudget viewBudget, TransactionDetail viewDetail)
        {
            InitializeComponent();

            panelAccountSummary.Children.Add(viewMain);
            _viewMoneyAccounts = viewMain;

            panelBudget.Children.Add(viewBudget);
            _viewBudgetAnalysis = viewBudget;

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
    }
}
