using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker2.Core;
using DLPMoneyTracker2.Main.TransactionList;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.BudgetAnalysis
{
    public class CurrentMonthBudgetVM : BaseViewModel
    {
        private readonly IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase;
        private readonly NotificationSystem notifications;

        public CurrentMonthBudgetVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            NotificationSystem notifications)
        {
            this.getAccountsByTypeUseCase = getAccountsByTypeUseCase;
            this.notifications = notifications;
            this.notifications.TransactionsModified += Notifications_TransactionsModified;
            this.notifications.BudgetAmountChanged += Notifications_BudgetAmountChanged;

            this.Load();
        }

        private void Notifications_BudgetAmountChanged(Guid accountUID)
        {
            this.Load();
        }

        private void Notifications_TransactionsModified(Guid debitAccountId, Guid creditAccountId)
        {
            this.Load();
        }

        // List of Receivable IJournalAccounts; NOT TO BE DISPLAYED
        private readonly List<JournalAccountBudgetVM> _listIncome = [];

        public decimal TotalBudgetIncome =>
            _listIncome.Where(x => x.IsFixedExpense).Sum(s => s.MonthlyBudget) +
            _listIncome.Where(x => !x.IsFixedExpense).Sum(s => s.MonthlyBudgetAmount);

        // List of Payable IJournalAccounts WITH a Journal Plan
        private readonly ObservableCollection<JournalAccountBudgetVM> _listFixed = [];

        public ObservableCollection<JournalAccountBudgetVM> FixedExpenses => _listFixed;

        public decimal FixedExpenseBudgetTotal => this.FixedExpenses?.Sum(s => s.MonthlyBudget) ?? decimal.Zero;
        public decimal FixedExpenseCurrent => this.FixedExpenses?.Sum(s => s.CurrentMonthTotal) ?? decimal.Zero;

        // The remaining Payable accounts
        private readonly ObservableCollection<JournalAccountBudgetVM> _listVariable = [];

        public ObservableCollection<JournalAccountBudgetVM> VariableExpenses => _listVariable;

        public decimal VariableExpenseBudgetTotal => this.VariableExpenses?.Sum(s => s.MonthlyBudget) ?? decimal.Zero;
        public decimal VariableExpenseCurrent => this.VariableExpenses?.Sum(s => s.CurrentMonthTotal) ?? decimal.Zero;
        public decimal TotalExpenseBudget => this.FixedExpenseBudgetTotal + this.VariableExpenseBudgetTotal;
        public decimal UnallocatedBudget => this.TotalBudgetIncome - this.TotalExpenseBudget;
        public decimal TotalCurrentExpense => this.FixedExpenseCurrent + this.VariableExpenseCurrent;
        public decimal MonthlyBalance => this.TotalBudgetIncome - this.TotalCurrentExpense;

        #region Commands

        public RelayCommand CommandShowDetail =>
            new((act) =>
            {
                if (act is IJournalAccount ja)
                {
                    DateTime start = new(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTime end = new(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                    TransDetailFilter filter = new()
                    {
                        Account = ja,
                        FilterDates = new DateRange(start, end),
                        AreFilterControlsVisible = false,
                        UseBudgetLogic = true
                    };
                    AccountTransactionDetail window = new(filter);
                    window.Show();
                }
            });

        public RelayCommand CommandResetBudgets =>
            new((o) =>
            {
                if (this.VariableExpenses.Any() != true) return;
                foreach (var account in this.VariableExpenses)
                {
                    account.ResetBudget();
                }
            });

        #endregion Commands

        private readonly List<LedgerType> ValidBudgetTypes =
        [
            LedgerType.Receivable,
            LedgerType.Payable,
            LedgerType.LiabilityLoan
        ];

        public void Load()
        {
            _listIncome.Clear();
            _listFixed.Clear();
            _listVariable.Clear();

            var listAccounts = getAccountsByTypeUseCase.Execute(ValidBudgetTypes);
            if (listAccounts?.Any() != true) return;

            foreach (var act in listAccounts)
            {
                JournalAccountBudgetVM budget = UICore.DependencyHost.GetRequiredService<JournalAccountBudgetVM>();
                budget.Load(act);
                switch (act.JournalType)
                {
                    case LedgerType.Receivable:
                        if (act.DateClosedUTC is null && budget.BudgetType != BudgetTrackingType.DO_NOT_TRACK)
                        {
                            _listIncome.Add(budget);
                        }
                        break;

                    case LedgerType.Payable:
                        if (budget.IsVisible && budget.BudgetType != BudgetTrackingType.DO_NOT_TRACK)
                        {
                            if (budget.IsFixedExpense)
                            {
                                this.FixedExpenses.Add(budget);
                            }
                            else
                            {
                                this.VariableExpenses.Add(budget);
                            }
                        }
                        break;

                    case LedgerType.LiabilityLoan:
                        if (budget.IsVisible)
                        {
                            this.FixedExpenses.Add(budget);
                        }
                        break;
                }
            }
            this.NotifyAll();
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(FixedExpenses));
            NotifyPropertyChanged(nameof(VariableExpenses));
            NotifyPropertyChanged(nameof(MonthlyBalance));
            NotifyPropertyChanged(nameof(TotalExpenseBudget));
            NotifyPropertyChanged(nameof(TotalCurrentExpense));
            NotifyPropertyChanged(nameof(VariableExpenseBudgetTotal));
            NotifyPropertyChanged(nameof(FixedExpenseBudgetTotal));
            NotifyPropertyChanged(nameof(TotalBudgetIncome));
            NotifyPropertyChanged(nameof(UnallocatedBudget));
            NotifyPropertyChanged(nameof(FixedExpenseCurrent));
            NotifyPropertyChanged(nameof(VariableExpenseCurrent));
        }
    }
}