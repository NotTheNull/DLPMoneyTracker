using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    /// <summary>
    /// Interaction logic for AddTransaction.xaml
    /// </summary>
    public partial class AddExpenseView : Window
    {
        private AddExpenseVM _viewModel;

        public AddExpenseView(AddExpenseVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        public void CreateTransactionFromMoneyPlan(IMoneyPlan plan)
        {
            if (plan is null) throw new ArgumentNullException("Money Plan");

            _viewModel.SelectedAccountId = plan.AccountID;
            _viewModel.SelectedCategoryId = plan.CategoryID;
            _viewModel.Description = plan.Description;
            _viewModel.Amount = plan.ExpectedAmount;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveTransaction();
            this.Close();
        }
    }
}
