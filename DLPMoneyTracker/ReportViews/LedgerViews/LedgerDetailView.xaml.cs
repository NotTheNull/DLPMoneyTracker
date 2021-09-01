using DLPMoneyTracker.Data;
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
        //       Filtering added to View Models; still need UI Controls

        private readonly ILedger _ledger;
        private readonly ITrackerConfig _config;
        private LedgerDetailVM _viewModel;
        
        public string LedgerPath { get { return _viewModel?.LedgerPath ?? string.Empty; } }

        public LedgerDetailView(ILedger ledger, ITrackerConfig config)
        {
            InitializeComponent();
            _ledger = ledger;
            _config = config;
        }

        public void ShowAccountDetail(MoneyAccount act)
        {
            _viewModel = new MoneyAccountLedgerDetailVM(act, _ledger, _config);
            this.DataContext = _viewModel;
        }

        public void ShowCategoryDetail(TransactionCategory cat)
        {
            _viewModel = new TransactionCategoryLedgerDetailVM(cat, _ledger, _config);
            this.DataContext = _viewModel;
        }

        public void ShowFullLedgerDetail()
        {
            _viewModel = new StandardLedgerDetailVM(_ledger, _config);
            this.DataContext = _viewModel;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsCloseButtonVisible) return;
            var win = Window.GetWindow(this);
            win.Close();
        }
    }
}