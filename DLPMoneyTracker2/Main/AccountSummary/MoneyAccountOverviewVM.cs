using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class MoneyAccountOverviewVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;

        private readonly List<JournalAccountType> _listValidTypes = new List<JournalAccountType>()
        {
            JournalAccountType.Bank,
            JournalAccountType.LiabilityCard,
            JournalAccountType.LiabilityLoan
        };

        public MoneyAccountOverviewVM(ITrackerConfig config)
        {
            _config = config;
            this.Load();
        }

        private ObservableCollection<MoneyAccountSummaryVM> _listAcctSummary = new ObservableCollection<MoneyAccountSummaryVM>();
        public ObservableCollection<MoneyAccountSummaryVM> AccountSummaryList
        { get { return _listAcctSummary; } }

        public void Load()
        {
            _listAcctSummary.Clear();
            foreach (var act in _config.LedgerAccountsList.Where(x => _listValidTypes.Contains(x.JournalType) && x.DateClosedUTC is null).OrderBy(o => o.OrderBy).ThenBy(o => o.Description))
            {
                MoneyAccountSummaryVM summary = UICore.DependencyHost.GetRequiredService<MoneyAccountSummaryVM>();
                summary.LoadAccount(act);
                _listAcctSummary.Add(summary);
            }
        }

        public void Refresh()
        {
            if (!this.AccountSummaryList.Any()) return;

            foreach (var summary in this.AccountSummaryList)
            {
                summary.Refresh();
            }

            bool hasNEWAccounts(IJournalAccount act)
            {
                return _listValidTypes.Contains(act.JournalType)
                    && !this.AccountSummaryList.Any(x => x.AccountId == act.Id);
            }

            if (_config.LedgerAccountsList.Any(hasNEWAccounts))
            {
                foreach (var act in _config.LedgerAccountsList.Where(hasNEWAccounts))
                {
                    MoneyAccountSummaryVM summary = UICore.DependencyHost.GetRequiredService<MoneyAccountSummaryVM>();
                    summary.LoadAccount(act);
                    _listAcctSummary.Add(summary);
                }
            }
        }
    }
}