using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker.ReportViews
{
    public class MoneyAccountOverviewVM : BaseViewModel
    {
        private readonly IMoneyPlanner _budget;
        private readonly ITrackerConfig _config;
        private readonly ILedger _ledger;

        private ObservableCollection<MoneyAccountSummaryVM> _listAcctSummary = new ObservableCollection<MoneyAccountSummaryVM>();
        public ObservableCollection<MoneyAccountSummaryVM> AccountSummaryList { get { return _listAcctSummary; } }

        public MoneyAccountOverviewVM(ITrackerConfig config, IMoneyPlanner budget, ILedger ledger)
        {
            _budget = budget;
            _config = config;
            _ledger = ledger;
            this.Load();
        }

        public void Load()
        {
            _listAcctSummary.Clear();
            foreach (var act in _config.AccountsList.Where(x => x.DateClosedUTC is null).OrderBy(o => o.OrderBy).ThenBy(o => o.Description))
            {
                _listAcctSummary.Add(new MoneyAccountSummaryVM(act, _ledger, _budget, _config));
            }
        }

        public void Refresh()
        {
            if (!_listAcctSummary.Any()) return;

            foreach (var summary in _listAcctSummary)
            {
                summary.Refresh();
            }

            // Checking for NEW accounts after refresh to avoid refreshing THESE accounts unnecessarily
            bool hasNEWAccounts(MoneyAccount act)
            {
                return !_listAcctSummary.Any(x => x.AccountID == act.ID);
            }
            if (_config.AccountsList.Any(hasNEWAccounts))
            {
                foreach (var act in _config.AccountsList.Where(hasNEWAccounts))
                {
                    _listAcctSummary.Add(new MoneyAccountSummaryVM(act, _ledger, _budget, _config));
                }
            }
        }
    }
}