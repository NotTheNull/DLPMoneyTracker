using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DLPMoneyTracker.ReportViews.JournalViews
{
    /// <summary>
    /// Interaction logic for JournalDetailView.xaml
    /// </summary>
    public partial class JournalDetailView : UserControl
    {
        private readonly IJournal _journal;
        private readonly ITrackerConfig _config;
        private JournalDetailVM _viewModel;


        public JournalDetailView(IJournal journal, ITrackerConfig config, JournalDetailVM viewModel)
        {
            InitializeComponent();
            _journal = journal;
            _config = config;
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        public void ShowAccountDetail(IJournalAccount act)
        {
            _viewModel.FilterAccount = act;
            _viewModel.IsFiltersVisible = true;
            _viewModel.IsCloseButtonVisible = true;
        }

        public void ShowFilteredLedger(JournalDetailFilter filter)
        {
            _viewModel.ApplyFilters(filter);
            _viewModel.IsFiltersVisible = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsCloseButtonVisible) return;
            var win = Window.GetWindow(this);
            win.Close();
        }
    }
}
