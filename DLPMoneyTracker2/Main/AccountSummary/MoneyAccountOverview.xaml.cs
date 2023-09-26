using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    /// <summary>
    /// Interaction logic for MoneyAccountOverview.xaml
    /// </summary>
    public partial class MoneyAccountOverview : UserControl
    {
        public MoneyAccountOverview(MoneyAccountOverviewVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}