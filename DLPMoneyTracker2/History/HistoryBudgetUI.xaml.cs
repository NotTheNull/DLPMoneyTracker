using System.Windows;

namespace DLPMoneyTracker2.History
{
    /// <summary>
    /// Interaction logic for HistoryBudgetUI.xaml
    /// </summary>
    public partial class HistoryBudgetUI : Window
    {
        public HistoryBudgetUI(HistoryBudgetVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}