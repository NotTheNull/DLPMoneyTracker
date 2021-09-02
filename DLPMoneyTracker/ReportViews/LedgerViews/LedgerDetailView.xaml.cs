using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
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
        // TODO: Add filter controls

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

        public void ShowCategoryDetail(TransactionCategory cat, DateRange dates = null)
        {
            _viewModel = new TransactionCategoryLedgerDetailVM(cat, dates, _ledger, _config);
            this.DataContext = _viewModel;
        }

        public void ShowFullLedgerDetail()
        {
            _viewModel = new StandardLedgerDetailVM(_ledger, _config);
            this.DataContext = _viewModel;
        }

        public void ShowFilteredLedger(LedgerDetailFilter filter)
        {
            _viewModel = new StandardLedgerDetailVM(filter, _ledger, _config);
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