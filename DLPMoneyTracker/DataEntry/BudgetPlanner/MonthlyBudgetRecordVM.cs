using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels.Budget;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class MonthlyBudgetRecordVM : BaseViewModel, ILinkDataModelToViewModel<IBudget>
    {
        ITrackerConfig _config;


        public Guid UID { get { return this.Category?.ID ?? Guid.Empty; } }



        private TransactionCategory _cat;

        public TransactionCategory Category
        {
            get { return _cat; }
            set
            {
                _cat = value;
                NotifyPropertyChanged(nameof(this.Category));
                NotifyPropertyChanged(nameof(this.CategoryName));
            }
        }

        public string CategoryName { get { return this.Category?.Name ?? string.Empty; } }


        private decimal _amt;

        public decimal BudgetAmount
        {
            get { return _amt; }
            set
            {
                _amt = value;
                NotifyPropertyChanged(nameof(this.BudgetAmount));
            }
        }



        private bool _isFixed;

        public bool IsFixedExpense
        {
            get { return _isFixed; }
            set
            {
                _isFixed = value;
                NotifyPropertyChanged(nameof(this.IsFixedExpense));
            }
        }





        public MonthlyBudgetRecordVM(ITrackerConfig config)
        {
            _config = config;
        }






        public IBudget GetSource()
        {
            if (this.IsFixedExpense) return null;
            return new MonthlyBudget()
            {
                Category = this.Category,
                BudgetAmount = this.BudgetAmount
            };
        }

        public void LoadSource(IBudget src)
        {
            if (src is null) throw new ArgumentNullException("Budget Source");

            this.Category = _config.GetCategory(src.CategoryId);
            this.BudgetAmount = src.BudgetAmount;
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.Category));
            NotifyPropertyChanged(nameof(this.CategoryName));
            NotifyPropertyChanged(nameof(this.IsFixedExpense));
            NotifyPropertyChanged(nameof(this.BudgetAmount));
        }
    }
}
