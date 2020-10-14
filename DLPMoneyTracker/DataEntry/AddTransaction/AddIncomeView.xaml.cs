using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.AddTransaction
{
    /// <summary>
    /// Interaction logic for AddIncomeView.xaml
    /// </summary>
    public partial class AddIncomeView : Window
    {
        private AddIncomeVM _viewModel;

        public AddIncomeView(AddIncomeVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }

        public void CreateTransactionFromMoneyPlan(IMoneyPlan plan)
        {
            if (plan is null) throw new ArgumentNullException("Money Plan");

            _viewModel.BankAccountId = plan.AccountID;
            _viewModel.CategoryId = plan.CategoryID;
            _viewModel.Description = plan.Description;
            _viewModel.Amount = plan.ExpectedAmount;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveTransaction();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
            this.Close();
        }
    }
}