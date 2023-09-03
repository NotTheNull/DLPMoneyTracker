using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.BudgetAnalysis
{
    /// <summary>
    /// Interaction logic for CurrentMonthBudget.xaml
    /// </summary>
    public partial class CurrentMonthBudget : UserControl
    {
        public CurrentMonthBudget(CurrentMonthBudgetVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}