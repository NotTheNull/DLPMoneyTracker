using DLPMoneyTracker.Data;
using DLPMoneyTracker.DataEntry.AddEditCategories;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;
using DLPMoneyTracker.ReportViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DLPMoneyTracker
{
    // TODO: Add UI to display summary of Money Accounts
    // TODO: Add ability to create transactions
    // TODO: Add Window to display Money Account detail
    // TODO: Create VM and UI to display summary of Categories that include current totals and budget expectations
    // TODO: Modify Main Window to show tabbed interface.  Main tab is summary of Money Accounts.  Second tab would be summary of Categories. Third tab could be Full Ledger.



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MoneyAccountsOverview _uiAccountSummary;

        public MainWindow(MoneyAccountsOverview uiAccountSummary)
        {
            InitializeComponent();
            panelAccountSummary.Children.Add(uiAccountSummary);
            _uiAccountSummary = uiAccountSummary;
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
    }
}
