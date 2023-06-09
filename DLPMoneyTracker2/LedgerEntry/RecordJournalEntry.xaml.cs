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

namespace DLPMoneyTracker2.LedgerEntry
{
    /// <summary>
    /// Interaction logic for RecordJournalEntry.xaml
    /// </summary>
    public partial class RecordJournalEntry : Window
    {
        private BaseRecordJournalEntryVM _viewModel;

        public RecordJournalEntry(BaseRecordJournalEntryVM viewModel)
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
