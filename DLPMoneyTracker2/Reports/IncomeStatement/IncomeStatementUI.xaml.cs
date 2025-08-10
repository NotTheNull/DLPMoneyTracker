using System.Windows;

namespace DLPMoneyTracker2.Reports.IncomeStatement;
public partial class IncomeStatementUI : Window
{
    public IncomeStatementUI(IncomeStatementVM viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }

    private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
