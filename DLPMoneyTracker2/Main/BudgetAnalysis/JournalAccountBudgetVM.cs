


using DLPMoneyTracker.BusinessLogic.UseCases.BudgetPlans.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
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

        private List<IBudgetPlan> _listPlans = new List<IBudgetPlan>();

        private IJournalAccount _account;

        public IJournalAccount Account { get { return _account; } }
        public INominalAccount NominalAccount { get { return (INominalAccount)this.Account; } }

        public Guid AccountId { get { return _account.Id; } }

        public string AccountDesc { get { return _account.Description; } }

        public BudgetTrackingType BudgetType { get { return this.NominalAccount.BudgetType; } }
        public decimal MonthlyBudgetAmount
        {
            get { return this.NominalAccount.CurrentBudgetAmount; }
            set
            {
                if (this.IsFixedExpense) return;
                if(this.NominalAccount is PayableAccount payable)
                {
                    payable.CurrentBudgetAmount = value;
                    saveNominalAccountUseCase.Execute(_account);
                }
            }
        }

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
            this.CurrentMonthTotal = getBudgetBalanceForAccountUseCase.Execute(this.AccountId);

            _listPlans.Clear();
            var listPlans = getCurrentMonthPlansUseCase.Execute(this.AccountId);
            if (listPlans?.Any() == true)
            {
                // Need to make sure they apply to the current month
                foreach (var plan in listPlans)
                {
                    bool addPlanToList = false;
                    switch (plan.Recurrence.Frequency)
                    {
                        case DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Annual:
                            addPlanToList = plan.Recurrence.StartDate.Month == DateTime.Today.Month;
                            break;
                        case DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.SemiAnnual:
                            addPlanToList = plan.Recurrence.StartDate.Month == DateTime.Today.Month || plan.Recurrence.StartDate.AddMonths(6).Month == DateTime.Today.Month;
                            break;
                        case DLPMoneyTracker.Core.Models.ScheduleRecurrence.RecurrenceFrequency.Monthly:
                            addPlanToList = true;
                            break;
                    }

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