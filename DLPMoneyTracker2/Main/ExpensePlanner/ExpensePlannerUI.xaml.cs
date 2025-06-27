using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.ExpensePlanner
{
    /// <summary>
    /// Interaction logic for ExpensePlannerUI.xaml
    /// </summary>
    public partial class ExpensePlannerUI : UserControl
    {
        public ExpensePlannerUI(ExpensePlannerVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}