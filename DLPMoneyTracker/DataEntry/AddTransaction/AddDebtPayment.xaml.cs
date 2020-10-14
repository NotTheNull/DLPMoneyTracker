using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    /// <summary>
    /// Interaction logic for AddDebtPayment.xaml
    /// </summary>
    public partial class AddDebtPayment : Window
    {
        private AddDebtPaymentVM _viewModel;

        public AddDebtPayment(AddDebtPaymentVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SavePayment();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
            this.Close();
        }
    }
}