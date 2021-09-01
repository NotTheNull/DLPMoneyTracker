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

        // TODO: Add button within each grid that will display a list of the transactions for that category for the Month
    }
}