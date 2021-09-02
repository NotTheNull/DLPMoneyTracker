using System.Windows.Controls;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    /// <summary>
    /// Interaction logic for MonthlyBudgetPlannerView.xaml
    /// </summary>
    public partial class MonthlyBudgetPlannerView : UserControl
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