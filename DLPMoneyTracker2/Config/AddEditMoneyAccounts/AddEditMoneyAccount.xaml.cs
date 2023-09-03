using System.Windows;

namespace DLPMoneyTracker2.Config.AddEditMoneyAccounts
{
    /// <summary>
    /// Interaction logic for AddEditMoneyAccount.xaml
    /// </summary>
    public partial class AddEditMoneyAccount : Window
    {
        public AddEditMoneyAccount(AddEditMoneyAccountVM viewModel)
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