using System.Windows;

namespace DLPMoneyTracker2.Config.AddEditLedgerAccounts
{
    /// <summary>
    /// Interaction logic for AddEditLedgerAccount.xaml
    /// </summary>
    public partial class AddEditLedgerAccount : Window
    {
        public AddEditLedgerAccount(AddEditLedgerAccountVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}