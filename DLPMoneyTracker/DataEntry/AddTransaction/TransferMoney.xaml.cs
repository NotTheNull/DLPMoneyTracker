using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    /// <summary>
    /// Interaction logic for TransferMoney.xaml
    /// </summary>
    public partial class TransferMoney : Window
    {
        private TransferMoneyVM _viewModel;

        public TransferMoney(TransferMoneyVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveTransfer();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
            this.Close();
        }
    }
}