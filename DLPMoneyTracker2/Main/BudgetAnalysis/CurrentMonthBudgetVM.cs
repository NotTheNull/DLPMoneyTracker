using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker2.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.BudgetAnalysis
{
    public class CurrentMonthBudgetVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;

        public CurrentMonthBudgetVM(ITrackerConfig config)
        {
            _config = config;
        }


        // List of Receivable IJournalAccounts; NOT TO BE DISPLAYED
        private List<IJournalAccount> _listIncome = new List<IJournalAccount>();

        public decimal TotalBudgetIncome { get { return _listIncome?.Sum(s => s.MonthlyBudgetAmount) ?? decimal.Zero; } }

        // List of Payable IJournalAccounts WITH a Journal Plan
        private ObservableCollection<JournalAccountBudgetVM> _listFixed = new ObservableCollection<JournalAccountBudgetVM>();
        private ObservableCollection<JournalAccountBudgetVM> FixedExpenses { get { return _listFixed; } }
        public decimal FixedExpenseBudgetTotal { get { return this.FixedExpenses?.Sum(s => s.MonthlyBudget > s.CurrentMonthTotal ? s.MonthlyBudget : s.CurrentMonthTotal) ?? decimal.Zero; } }


        // The remaining Payable accounts
        private ObservableCollection<JournalAccountBudgetVM> _listVariable = new ObservableCollection<JournalAccountBudgetVM>();
        private ObservableCollection<JournalAccountBudgetVM> VariableExpenses { get { return _listVariable; } }
        public decimal VariableExpenseBudgetTotal { get { return this.VariableExpenses?.Sum(s => s.MonthlyBudget > s.CurrentMonthTotal ? s.MonthlyBudget : s.CurrentMonthTotal) ?? decimal.Zero; } }


        public decimal TotalExpenseBudget { get { return this.FixedExpenseBudgetTotal + this.VariableExpenseBudgetTotal; } }

        public decimal MonthlyBalance { get { return this.TotalBudgetIncome - this.TotalExpenseBudget; } }



        #region Commands
        private RelayCommand _cmdShowDetail;
        public RelayCommand CommandShowDetail
        {
            get
            {
                return _cmdShowDetail ?? (_cmdShowDetail = new RelayCommand((act) =>
                {
                    // TODO: Show transaction listing for this account
                }));
            }
        }
        #endregion



        public void Load()
        {
            _listIncome.Clear();
            _listFixed.Clear();
            _listVariable.Clear();

            if (_config.LedgerAccountsList?.Any() != true) return;

            foreach (var act in _config.LedgerAccountsList)
            {
                JournalAccountBudgetVM budget = UICore.DependencyHost.GetRequiredService<JournalAccountBudgetVM>();
                switch (act.JournalType)
                {
                    case JournalAccountType.Receivable:
                        if (act.DateClosedUTC is null)
                        {
                            _listIncome.Add(act);
                        }
                        break;
                    case JournalAccountType.Payable:
                        budget.Load(act);
                        if(budget.IsVisible)
                        {
                            if(budget.IsFixedExpense)
                            {
                                _listFixed.Add(budget);
                            }
                            else
                            {
                                _listVariable.Add(budget);
                            }
                        }
                        break;
                    case JournalAccountType.LiabilityLoan:
                        budget.Load(act);
                        if(budget.IsVisible)
                        {
                            _listFixed.Add(budget);
                        }
                        break;
                }
            }

        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(MonthlyBalance));
            NotifyPropertyChanged(nameof(TotalExpenseBudget));
            NotifyPropertyChanged(nameof(VariableExpenseBudgetTotal));
            NotifyPropertyChanged(nameof(FixedExpenseBudgetTotal));
            NotifyPropertyChanged(nameof(TotalBudgetIncome));
        }

    }
}
