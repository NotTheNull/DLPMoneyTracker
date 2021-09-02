using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using DLPMoneyTracker.ReportViews.LedgerViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class MonthlyBudgetPlannerVM : BaseViewModel
    {
        private ITrackerConfig _config;
        private IBudgetTracker _budget;
        private IMoneyPlanner _moneyPlanner;
        private ILedger _ledger;

        private decimal _incomeTotal;

        public decimal MonthlyIncomeTotal
        {
            get { return _incomeTotal; }
            set
            {
                _incomeTotal = value;
                NotifyPropertyChanged(nameof(this.MonthlyIncomeTotal));
            }
        }

        private List<MonthlyBudgetRecordVM> _listFixed = new List<MonthlyBudgetRecordVM>();
        public ReadOnlyCollection<MonthlyBudgetRecordVM> FixedExpenseList { get { return _listFixed.AsReadOnly(); } }

        private List<MonthlyBudgetRecordVM> _listBudget = new List<MonthlyBudgetRecordVM>();
        public ReadOnlyCollection<MonthlyBudgetRecordVM> VariableExpenseList { get { return _listBudget.AsReadOnly(); } }

        public decimal FixedExpenseTotal { get { return this.FixedExpenseList?.Sum(s => s.BudgetAmount) ?? decimal.Zero; } }

        public decimal VariableExpenseTotal { get { return this.VariableExpenseList?.Sum(s => s.BudgetAmount) ?? decimal.Zero; } }

        public decimal GrandExpenseTotal { get { return this.FixedExpenseTotal + this.VariableExpenseTotal; } }

        public decimal OverBudgetAmount
        {
            get
            {
                decimal overBudget = decimal.Zero;
                if (_listFixed.Any(x => x.RemainingBudgetAmount < decimal.Zero))
                {
                    overBudget += _listFixed.Where(x => x.RemainingBudgetAmount < decimal.Zero).Sum(s => s.RemainingBudgetAmount);
                }

                if (_listBudget.Any(x => x.RemainingBudgetAmount < decimal.Zero))
                {
                    overBudget += _listBudget.Where(x => x.RemainingBudgetAmount < decimal.Zero).Sum(s => s.RemainingBudgetAmount);
                }

                // Value will be negative; negate it so that the UI shows it as a positive value
                return overBudget * -1;
            }
        }

        public bool HasCategoriesOverBudget { get { return this.OverBudgetAmount > decimal.Zero; } }

        public decimal MonthlyBalance { get { return this.MonthlyIncomeTotal - this.GrandExpenseTotal - this.OverBudgetAmount; } }

        #region Commands

        private RelayCommand _cmdSave;

        public RelayCommand SaveChanges
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
                {
                    this.CommitChanges();
                }));
            }
        }


        private RelayCommand _cmdShowDetail;
        public RelayCommand CommandShowDetail
        {
            get
            {
                return _cmdShowDetail ?? (_cmdShowDetail = new RelayCommand((objCategory) =>
                {
                    if (objCategory is TransactionCategory cat)
                    {
                        DateRange monthRange = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)));
                        
                        LedgerDetailView uiAccountLedger = UICore.DependencyHost.GetService<LedgerDetailView>();
                        uiAccountLedger.ShowCategoryDetail(cat, monthRange);
                        Window windowLedger = new Window()
                        {
                            Content = uiAccountLedger,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Title = "Ledger Detail",
                            Width = 750,
                            Height = 500
                        };
                        windowLedger.Show();
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Type {0} is not supported for Budget Planner Detail", objCategory.GetType().FullName));
                    }
                }));
            }
        }

        #endregion Commands

        public MonthlyBudgetPlannerVM(ITrackerConfig config, IBudgetTracker budget, IMoneyPlanner planner, ILedger ledger)
        {
            _config = config;
            _budget = budget;
            _moneyPlanner = planner;
            _ledger = ledger;
            _ledger.LedgerModified += _ledger_LedgerModified;

            this.LoadBudgetList();
            this.CalcIncome();
        }

        private void _ledger_LedgerModified()
        {
            this.LoadCurrentValues();
            this.NotifyAll();
        }

        private void LoadCurrentValues()
        {
            foreach (var budget in _listFixed)
            {
                budget.CurrentValue = _ledger.GetCategoryTotal_CurrentMonth(budget.Category);
            }

            foreach (var budget in _listBudget)
            {
                budget.CurrentValue = _ledger.GetCategoryTotal_CurrentMonth(budget.Category);
            }
        }

        private void LoadBudgetList()
        {
            _listBudget.Clear();
            _listFixed.Clear();
            foreach (TransactionCategory cat in _config.CategoryList.Where(x => x.CategoryType == CategoryType.Expense && !x.ExcludeFromBudget))
            {
                bool isMonthlyFixedExpense(IMoneyPlan plan)
                {
                    return plan.CategoryID == cat.ID &&
                         (
                            plan.Frequency == Data.ScheduleRecurrence.RecurrenceFrequency.Monthly ||
                            plan.NextOccurrence.Month == DateTime.Today.Month
                         );
                }

                MonthlyBudgetRecordVM record = new MonthlyBudgetRecordVM(_config)
                {
                    Category = cat,
                    IsFixedExpense = false,
                    BudgetAmount = decimal.Zero
                };

                if (_moneyPlanner.MoneyPlanList.Any(isMonthlyFixedExpense))
                {
                    // Make certain to set the amount BEFORE updating the Fixed Expense flag since it prevents further modification
                    record.BudgetAmount = _moneyPlanner.MoneyPlanList.Where(isMonthlyFixedExpense).Sum(s => s.ExpectedAmount);
                    record.IsFixedExpense = true;
                    _listFixed.Add(record);
                }
                else
                {
                    record.BudgetAmount = _budget.GetBudgetAmount(cat.ID);
                    record.BudgetAmountModified += Record_BudgetAmountModified;
                    _listBudget.Add(record);
                }
            }

            this.LoadCurrentValues();
            this.NotifyAll();
        }

        private void Record_BudgetAmountModified()
        {
            this.CommitChanges();
            this.NotifyAll();
        }

        private void CalcIncome()
        {
            this.MonthlyIncomeTotal = _moneyPlanner.MoneyPlanList
                .Where(x =>
                    x.PlanType == Data.TransactionModels.BillPlan.MoneyPlanType.Income
                    && x.Frequency == Data.ScheduleRecurrence.RecurrenceFrequency.Monthly
                    && !x.ExcludeFromBudgetPlanner
                ).Sum(s => s.ExpectedAmount);
        }

        private void CommitChanges()
        {
            _budget.ClearBudget();
            if (this.VariableExpenseList.Any())
            {
                foreach (var budget in this.VariableExpenseList)
                {
                    _budget.AddBudget(budget.GetSource());
                }
            }
            _budget.SaveToFile();
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.MonthlyIncomeTotal));
            NotifyPropertyChanged(nameof(this.FixedExpenseList));
            NotifyPropertyChanged(nameof(this.FixedExpenseTotal));
            NotifyPropertyChanged(nameof(this.VariableExpenseList));
            NotifyPropertyChanged(nameof(this.VariableExpenseTotal));
            NotifyPropertyChanged(nameof(this.OverBudgetAmount));
            NotifyPropertyChanged(nameof(this.HasCategoriesOverBudget));
            NotifyPropertyChanged(nameof(this.GrandExpenseTotal));
            NotifyPropertyChanged(nameof(this.MonthlyBalance));
        }
    }
}