using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DLPMoneyTracker2.Main.BudgetAnalysis
{
    public class JournalAccountBudgetVM : BaseViewModel
    {
        private readonly IJournal _journal;
        private readonly IJournalPlanner _planner;
        private readonly ITrackerConfig _config;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public JournalAccountBudgetVM(ITrackerConfig config, IJournalPlanner planner, IJournal journal)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _journal = journal;
            _planner = planner;
            _config = config;

            _currMon = decimal.Zero;
            _journal.JournalModified += _journal_JournalModified;
        }

        private void _journal_JournalModified()
        {
            this.Refresh();
        }

        private List<IJournalPlan> _listPlans = new List<IJournalPlan>();

        private IJournalAccount _account;

        public IJournalAccount Account
        { get { return _account; } }

        public Guid AccountId
        { get { return _account.Id; } }

        public string AccountDesc
        { get { return _account.Description; } }

        public decimal MonthlyBudget
        {
            get
            {
                if (_listPlans.Count == 0)
                {
                    if (this.Account is ILedgerAccount ledger) return ledger.MonthlyBudgetAmount;
                }

                return _listPlans.Sum(s => s.ExpectedAmount);
            }
        }

        public bool IsFixedExpense
        {
            get
            {
                return _listPlans.Count > 0;
            }
        }

        // NOTE: even if the account is closed, if there are transactions then it should be visible
        public bool IsVisible
        {
            get
            {
                return this.CurrentMonthTotal != decimal.Zero || _account?.DateClosedUTC == null;
            }
        }

        private decimal _currMon;

        public decimal CurrentMonthTotal
        {
            get { return _currMon; }
            set
            {
                _currMon = value;
                NotifyPropertyChanged(nameof(CurrentMonthTotal));
            }
        }

        public SolidColorBrush CurrentValueFontColor
        {
            get
            {
                if (this.CurrentMonthTotal > this.MonthlyBudget)
                {
                    return new SolidColorBrush(Colors.Red);
                }

                return new SolidColorBrush(Colors.Black);
            }
        }

        public void Load(IJournalAccount account)
        {
            _account = account;
            this.Refresh();
        }

        public void Refresh()
        {
            this.CurrentMonthTotal = _journal.GetAccountBalance_CurrentMonth(this.AccountId, true);

            if (_planner.JournalPlanList.Any(x => x.DebitAccountId == _account.Id || x.CreditAccountId == _account.Id))
            {
                _listPlans.AddRange(_planner.JournalPlanList.Where(x => x.DebitAccountId == _account.Id || x.CreditAccountId == _account.Id));
            }
            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(AccountDesc));
            NotifyPropertyChanged(nameof(IsVisible));
            NotifyPropertyChanged(nameof(MonthlyBudget));
            NotifyPropertyChanged(nameof(IsFixedExpense));
            NotifyPropertyChanged(nameof(CurrentValueFontColor));
        }
    }
}