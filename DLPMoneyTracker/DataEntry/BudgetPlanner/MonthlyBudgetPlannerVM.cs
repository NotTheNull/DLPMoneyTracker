using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using DLPMoneyTracker.Data.TransactionModels.Budget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class MonthlyBudgetPlannerVM : BaseViewModel
    {
        ITrackerConfig _config;
        IBudgetTracker _budget;
        IMoneyPlanner _moneyPlanner;


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



        ObservableCollection<MonthlyBudgetRecordVM> _listBudgets = new ObservableCollection<MonthlyBudgetRecordVM>();
        public ObservableCollection<MonthlyBudgetRecordVM> BudgetList { get { return _listBudgets; } }



        public MonthlyBudgetPlannerVM(ITrackerConfig config, IBudgetTracker budget, IMoneyPlanner planner)
        {
            _config = config;
            _budget = budget;
            _moneyPlanner = planner;

            this.LoadBudgetList();
            this.CalcIncome();
        }


        private void LoadBudgetList()
        {
            this.BudgetList.Clear();
            foreach(TransactionCategory cat in _config.CategoryList.Where(x => x.CategoryType == CategoryType.Expense))
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

                if(_moneyPlanner.MoneyPlanList.Any(isMonthlyFixedExpense))
                {
                    record.IsFixedExpense = true;
                    record.BudgetAmount = _moneyPlanner.MoneyPlanList.Where(isMonthlyFixedExpense).Sum(s => s.ExpectedAmount);
                }
                else 
                {
                    record.BudgetAmount = _budget.GetBudgetAmount(cat.ID);
                }

                this.BudgetList.Add(record);
            }
        }

        private void CalcIncome()
        {
            this.MonthlyIncomeTotal = _moneyPlanner.MoneyPlanList.Where(x => x.PlanType == Data.TransactionModels.BillPlan.MoneyPlanType.Income && x.Frequency == Data.ScheduleRecurrence.RecurrenceFrequency.Monthly).Sum(s => s.ExpectedAmount);
        }


        private void CommitChanges()
        {
            _budget.ClearBudget();
            if (this.BudgetList.Any(x => !x.IsFixedExpense))
            {
                foreach(var budget in this.BudgetList.Where(x => !x.IsFixedExpense))
                {
                    _budget.AddBudget(budget.GetSource());
                }
            }
            _budget.SaveToFile();
        }



    }
}
