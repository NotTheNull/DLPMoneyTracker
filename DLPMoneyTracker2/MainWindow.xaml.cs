using DLPMoneyTracker2.Config.AddEditLedgerAccounts;
using DLPMoneyTracker2.Config.AddEditMoneyAccounts;
using DLPMoneyTracker2.Main.AccountSummary;
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

        public MainWindow(MoneyAccountOverview viewMain, TransactionDetail viewDetail)
        {
            InitializeComponent();

            panelAccountSummary.Children.Add(viewMain);
            _viewMoneyAccounts = viewMain;

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
    }
}
