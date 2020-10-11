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

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    /// <summary>
    /// Interaction logic for MonthlyBudgetPlannerView.xaml
    /// </summary>
    public partial class MonthlyBudgetPlannerView : Window
    {
        private MonthlyBudgetPlannerVM _viewModel;

        public MonthlyBudgetPlannerView(MonthlyBudgetPlannerVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
