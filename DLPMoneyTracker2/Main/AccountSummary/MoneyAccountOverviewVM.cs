

using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.AccountSummary
{
    public class MoneyAccountOverviewVM : BaseViewModel
    {

        private readonly IGetMoneyAccountsUseCase getMoneyAccountsUseCase;

        public MoneyAccountOverviewVM(IGetMoneyAccountsUseCase getMoneyAccountsUseCase)
        {
            this.getMoneyAccountsUseCase = getMoneyAccountsUseCase;

            this.Load();
        }

        private ObservableCollection<MoneyAccountSummaryVM> _listAcctSummary = new ObservableCollection<MoneyAccountSummaryVM>();

        public ObservableCollection<MoneyAccountSummaryVM> AccountSummaryList
        { get { return _listAcctSummary; } }

        public void Load()
        {
            _listAcctSummary.Clear();
            var listAccounts = getMoneyAccountsUseCase.Execute(false);
            foreach (var act in listAccounts.OrderBy(o => o.OrderBy).ThenBy(o => o.Description))
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
                return !this.AccountSummaryList.Any(x => x.AccountId == act.Id);
            }

            var listAccounts = getMoneyAccountsUseCase.Execute(false);
            if (listAccounts.Any(hasNEWAccounts))
            {
                foreach (var act in listAccounts.Where(hasNEWAccounts))
                {
                    MoneyAccountSummaryVM summary = UICore.DependencyHost.GetRequiredService<MoneyAccountSummaryVM>();
                    summary.LoadAccount(act);
                    _listAcctSummary.Add(summary);
                }
            }
        }
    }
}