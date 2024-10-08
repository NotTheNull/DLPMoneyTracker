﻿

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
        private List<JournalAccountBudgetVM> _listIncome = new List<JournalAccountBudgetVM>();

        public decimal TotalBudgetIncome 
        { 
            get 
            { 
                return _listIncome.Where(x => x.IsFixedExpense).Sum(s => s.MonthlyBudget) +
                    _listIncome.Where(x => !x.IsFixedExpense).Sum(s => s.MonthlyBudgetAmount); 
            } 
        }

        // List of Payable IJournalAccounts WITH a Journal Plan
        private ObservableCollection<JournalAccountBudgetVM> _listFixed = new ObservableCollection<JournalAccountBudgetVM>();

        public ObservableCollection<JournalAccountBudgetVM> FixedExpenses { get { return _listFixed; } }

        public decimal FixedExpenseBudgetTotal { get { return this.FixedExpenses?.Sum(s => s.MonthlyBudget) ?? decimal.Zero; } }
        public decimal FixedExpenseCurrent { get { return this.FixedExpenses?.Sum(s => s.CurrentMonthTotal) ?? decimal.Zero; } }



        // The remaining Payable accounts
        private ObservableCollection<JournalAccountBudgetVM> _listVariable = new ObservableCollection<JournalAccountBudgetVM>();
        public ObservableCollection<JournalAccountBudgetVM> VariableExpenses { get { return _listVariable; } }

        public decimal VariableExpenseBudgetTotal { get { return this.VariableExpenses?.Sum(s => s.MonthlyBudget) ?? decimal.Zero; } }
        public decimal VariableExpenseCurrent { get { return this.VariableExpenses?.Sum(s => s.CurrentMonthTotal) ?? decimal.Zero; } }




        public decimal TotalExpenseBudget { get { return this.FixedExpenseBudgetTotal + this.VariableExpenseBudgetTotal; } }
        public decimal UnallocatedBudget { get { return this.TotalBudgetIncome - this.TotalExpenseBudget; } }
        public decimal TotalCurrentExpense { get { return this.FixedExpenseCurrent + this.VariableExpenseCurrent; } }




        public decimal MonthlyBalance { get { return this.TotalBudgetIncome - this.TotalCurrentExpense; } }

        #region Commands

        private RelayCommand _cmdShowDetail;

        public RelayCommand CommandShowDetail
        {
            get
            {
                return _cmdShowDetail ?? (_cmdShowDetail = new RelayCommand((act) =>
                {
                    if (act is IJournalAccount ja)
                    {
                        DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        DateTime end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                        TransDetailFilter filter = new TransDetailFilter()
                        {
                            Account = ja,
                            FilterDates = new DateRange(start, end),
                            AreFilterControlsVisible = false,
                            UseBudgetLogic = true
                        };
                        AccountTransactionDetail window = new AccountTransactionDetail(filter);
                        window.Show();
                    }
                }));
            }
        }


        private RelayCommand _cmdResetBudgets;
        public RelayCommand CommandResetBudgets
        {
            get
            {
                return _cmdResetBudgets ?? (_cmdResetBudgets = new RelayCommand((o) =>
                {
                    if (this.VariableExpenses.Any() != true) return;
                    foreach (var account in this.VariableExpenses)
                    {
                        account.ResetBudget();
                    }
                }));
            }
        }

        #endregion Commands

        private List<LedgerType> ValidBudgetTypes = new List<LedgerType>()
        {
            LedgerType.Receivable,
            LedgerType.Payable,
            LedgerType.LiabilityLoan
        };


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