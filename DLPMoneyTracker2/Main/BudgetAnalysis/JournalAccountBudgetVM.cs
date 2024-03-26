


using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DLPMoneyTracker2.Main.BudgetAnalysis
{
    public class JournalAccountBudgetVM : BaseViewModel
    {
        private readonly IGetCurrentMonthBudgetPlansForAccountUseCase getCurrentMonthPlansUseCase;
        private readonly IGetJournalAccountCurrentMonthBalanceUseCase getCurrentMonthBalanceUseCase;
        private readonly NotificationSystem notifications;


        public JournalAccountBudgetVM(
            IGetCurrentMonthBudgetPlansForAccountUseCase getCurrentMonthPlansUseCase,
            IGetJournalAccountCurrentMonthBalanceUseCase getCurrentMonthBalanceUseCase,
            NotificationSystem notifications)
        {
            this.getCurrentMonthPlansUseCase = getCurrentMonthPlansUseCase;
            this.getCurrentMonthBalanceUseCase = getCurrentMonthBalanceUseCase;
            this.notifications = notifications;

            this.notifications.TransactionsModified += Notifications_TransactionsModified;
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            if (this.AccountId != debitAccountId && this.AccountId != creditAccountId) return;

            this.Refresh();
        }

        private List<IBudgetPlan> _listPlans = new List<IBudgetPlan>();

        private IJournalAccount _account;

        public IJournalAccount Account { get { return _account; } }
        public INominalAccount NominalAccount { get { return (INominalAccount)this.Account; } }

        public Guid AccountId { get { return _account.Id; } }

        public string AccountDesc { get { return _account.Description; } }

        public BudgetTrackingType BudgetType { get { return this.NominalAccount.BudgetType; } }

        public decimal MonthlyBudget
        {
            get
            {
                return _listPlans.Sum(s => s.ExpectedAmount);
            }
        }

        public bool IsFixedExpense
        {
            get
            {
                return this.BudgetType == BudgetTrackingType.Fixed;
            }
        }

        // NOTE: even if the account is closed, if there are transactions then it should be visible
        public bool IsVisible
        {
            get
            {
                return
                    (
                        this.CurrentMonthTotal != decimal.Zero ||
                        _account?.DateClosedUTC == null
                    );
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
                NotifyPropertyChanged(nameof(CurrentValueFontColor));
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
            this.CurrentMonthTotal = getCurrentMonthBalanceUseCase.Execute(this.AccountId);

            _listPlans.Clear();
            var listPlans = getCurrentMonthPlansUseCase.Execute(this.AccountId);
            if(listPlans?.Any() == true)
            {
                _listPlans.AddRange(listPlans);
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