
using System.Windows;

namespace DLPMoneyTracker2.LedgerEntry
{
    /// <summary>
    /// Interaction logic for RecordJournalEntry.xaml
    /// </summary>
    public partial class RecordJournalEntry : Window
    {
        private IJournalEntryVM _viewModel;

        public RecordJournalEntry(IJournalEntryVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsValidTransaction)
            {
                _viewModel.SaveTransaction();
                this.Close();
            }
            else
            {
                MessageBox.Show("Transaction not valid");
            }
        }
    }
}