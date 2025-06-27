using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
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
        private readonly IGetAllCurrentMonthBudgetPlansForAccountUseCase getCurrentMonthPlansUseCase;
        private readonly IGetBudgetTransactionBalanceForAccountUseCase getBudgetBalanceForAccountUseCase;
        private readonly ISaveJournalAccountUseCase saveNominalAccountUseCase;
        private readonly NotificationSystem notifications;

        public JournalAccountBudgetVM(
            IGetAllCurrentMonthBudgetPlansForAccountUseCase getCurrentMonthPlansUseCase,
            IGetBudgetTransactionBalanceForAccountUseCase getBudgetBalanceForAccountUseCase,
            ISaveJournalAccountUseCase saveNominalAccountUseCase,
            NotificationSystem notifications)
        {
            this.getCurrentMonthPlansUseCase = getCurrentMonthPlansUseCase;
            this.getBudgetBalanceForAccountUseCase = getBudgetBalanceForAccountUseCase;
            this.saveNominalAccountUseCase = saveNominalAccountUseCase;
            this.notifications = notifications;

            this.notifications.TransactionsModified += Notifications_TransactionsModified;
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            if (this.AccountId != debitAccountId && this.AccountId != creditAccountId) return;

            this.Refresh();
        }

        #region Properties

        private readonly List<IBudgetPlan> _listPlans = [];

        private IJournalAccount _account = null!;
        public IJournalAccount Account => _account;
        public INominalAccount NominalAccount => (INominalAccount)this.Account;
        public Guid AccountId => _account.Id;
        public string AccountDesc => _account.Description;

        public BudgetTrackingType BudgetType => (this.Account.JournalType == LedgerType.LiabilityLoan) ? BudgetTrackingType.Fixed : this.NominalAccount.BudgetType;

        public decimal MonthlyBudgetAmount
        {
            get
            {
                if (this.IsFixedExpense) return decimal.Zero;

                return this.NominalAccount.CurrentBudgetAmount;
            }
            set
            {
                if (this.IsFixedExpense) return;
                if (this.NominalAccount is PayableAccount payable)
                {
                    payable.CurrentBudgetAmount = value;
                    saveNominalAccountUseCase.Execute(_account);
                    notifications.TriggerBudgetAmountChanged(this.NominalAccount.Id);
                }
            }
        }

        public decimal MonthlyBudget => this.IsFixedExpense ? _listPlans.Sum(s => s.ExpectedAmount) : this.MonthlyBudgetAmount;
        public bool IsFixedExpense => this.BudgetType == BudgetTrackingType.Fixed || this.Account.JournalType == LedgerType.LiabilityLoan;

        // NOTE: even if the account is closed, if there are transactions then it should be visible
        public bool IsVisible => this.CurrentMonthTotal != decimal.Zero || _account?.DateClosedUTC == null;

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

        public SolidColorBrush CurrentValueFontColor => Math.Abs(this.CurrentMonthTotal) > Math.Abs(this.MonthlyBudget) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);

        #endregion Properties

        public void Load(IJournalAccount account)
        {
            _account = account;
            this.Refresh();
        }

        public void Refresh()
        {
            this.CurrentMonthTotal = getBudgetBalanceForAccountUseCase.Execute(this.AccountId);

            _listPlans.Clear();
            var listPlans = getCurrentMonthPlansUseCase.Execute(this.AccountId);
            if (listPlans?.Any() == true)
            {
                // Need to make sure they apply to the current month
                foreach (var plan in listPlans)
                {
                    bool addPlanToList = plan.Recurrence.Frequency switch
                    {
                        DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Annual => plan.Recurrence.StartDate.Month == DateTime.Today.Month,
                        DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.SemiAnnual => plan.Recurrence.StartDate.Month == DateTime.Today.Month || plan.Recurrence.StartDate.AddMonths(6).Month == DateTime.Today.Month,
                        DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly => true,
                        _ => false
                    };

                    if (addPlanToList)
                    {
                        _listPlans.Add(plan);
                    }
                }
            }

            this.NotifyAll();
        }

        public void ResetBudget()
        {
            this.MonthlyBudgetAmount = this.NominalAccount.DefaultMonthlyBudgetAmount;
            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(AccountDesc));
            NotifyPropertyChanged(nameof(IsVisible));
            NotifyPropertyChanged(nameof(MonthlyBudget));
            NotifyPropertyChanged(nameof(IsFixedExpense));
            NotifyPropertyChanged(nameof(CurrentValueFontColor));
            NotifyPropertyChanged(nameof(MonthlyBudgetAmount));
        }
    }
}