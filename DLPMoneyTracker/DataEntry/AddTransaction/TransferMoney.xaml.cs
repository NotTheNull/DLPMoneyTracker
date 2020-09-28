using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
