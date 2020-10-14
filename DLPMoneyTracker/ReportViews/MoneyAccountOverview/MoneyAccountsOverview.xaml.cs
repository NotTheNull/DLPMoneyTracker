using System.Windows.Controls;

namespace DLPMoneyTracker.ReportViews
{
    /// <summary>
    /// Interaction logic for MoneyAccountsOverview.xaml
    /// </summary>
    public partial class MoneyAccountsOverview : UserControl
    {
        private MoneyAccountOverviewVM ViewModel { get { return this.DataContext as MoneyAccountOverviewVM; } }

        public MoneyAccountsOverview(MoneyAccountOverviewVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        public void Refresh()
        {
            this.ViewModel.Refresh();
        }
    }
}