using DLPMoneyTracker.Data.LedgerAccounts;
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
using System.Windows.Shapes;

namespace DLPMoneyTracker2.Main.TransactionList
{
    /// <summary>
    /// Interaction logic for AccountTransactionDetail.xaml
    /// </summary>
    public partial class AccountTransactionDetail : Window
    {
        public AccountTransactionDetail(TransDetailFilter filter)
        {
            InitializeComponent();
            AccountTransactionDetailVM viewModel = UICore.DependencyHost.GetRequiredService<AccountTransactionDetailVM>();
            viewModel.ApplyFilters(filter);
            this.DataContext = viewModel;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
