using Microsoft.Extensions.DependencyInjection;
using System.Windows;

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