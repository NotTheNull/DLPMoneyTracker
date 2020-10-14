using DLPMoneyTracker.Data.ConfigModels;
using System.Windows;
using System.Windows.Controls;

namespace DLPMoneyTracker.ReportViews.LedgerViews
{
    /// <summary>
    /// Interaction logic for LedgerDetailView.xaml
    /// </summary>
    public partial class LedgerDetailView : UserControl
    {
        // TODO: Consider adding filtering controls, at least for the Main Ledger

        private LedgerDetailVM _viewModel;
        private bool _canClose;

        public LedgerDetailView(LedgerDetailVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _viewModel = viewModel;
            _canClose = true;
        }

        public void ShowAccountDetail(MoneyAccount act)
        {
            _viewModel.ShowAccountDetail(act);
        }

        public void ShowCategoryDetail(TransactionCategory cat)
        {
            _viewModel.ShowCategoryDetail(cat);
        }

        public void ShowFullLedgerDetail()
        {
            _viewModel.ShowFullLedgerDetail();
            _canClose = false;
            btnClose.Visibility = Visibility.Collapsed;
            btnClose.IsEnabled = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_canClose) return;
            var win = Window.GetWindow(this);
            win.Close();
        }
    }
}