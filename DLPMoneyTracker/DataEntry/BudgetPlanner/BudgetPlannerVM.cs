using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    public class BudgetPlannerVM : BaseViewModel, IDisposable
    {
        private ITrackerConfig _config;
        private IBudgetPlanner _budget;

        #region Editing Control Properties

        private BudgetRecordVM _record;
        private BudgetRecordVM Source
        {
            get { return _record; }
            set
            {
                _record = value;
                this.IsExpense = _record.Category.CategoryType == CategoryType.Expense;
                this.NotifyAll();
            }
        }


        public Guid UID
        {
            get { return _record?.UID ?? Guid.Empty; }
        }


        private bool _isExp;

        public bool IsExpense
        {
            get { return _isExp; }
            set
            {
                if (value != _isExp)
                {
                    _isExp = value;
                    NotifyPropertyChanged(nameof(this.IsExpense));
                    this.LoadCategories();
                }
            }
        }




        public TransactionCategory SelectedCategory
        {
            get { return this.Source?.Category; }
            set
            {
                this.Source.Category = value;
                NotifyPropertyChanged(nameof(this.SelectedCategory));
            }
        }



        public string Description
        {
            get { return this.Source?.Description ?? string.Empty; }
            set
            {
                if (this.Source is null) return;
                this.Source.Description = value;
                NotifyPropertyChanged(nameof(this.Description));
            }
        }




        public decimal Amount
        {
            get { return this.Source?.Amount ?? decimal.Zero; }
            set
            {
                if (this.Source is null) return;
                this.Source.Amount = value;
                NotifyPropertyChanged(nameof(this.Amount));
            }
        }



        public IScheduleRecurrence Recurrence
        {
            get { return this.Source?.Recurrence; }
            set
            {
                if (this.Source is null) return;
                this.Source.Recurrence = value;
                NotifyPropertyChanged(nameof(this.Recurrence));
                NotifyPropertyChanged(nameof(this.RecurrenceType));
            }
        }

        public string RecurrenceType
        {
            get
            {
                return this.Recurrence?.Frequency.ToDisplayText() ?? string.Empty;
            }
        }


        #endregion




        private ObservableCollection<BudgetRecordVM> _listExpenses = new ObservableCollection<BudgetRecordVM>();
        public ObservableCollection<BudgetRecordVM> ExpenseBudgetList { get { return _listExpenses; } }

        private ObservableCollection<BudgetRecordVM> _listIncome = new ObservableCollection<BudgetRecordVM>();
        public ObservableCollection<BudgetRecordVM> IncomeBudgetList { get { return _listIncome; } }



        ObservableCollection<SpecialDropListItem<TransactionCategory>> _listCategories = new ObservableCollection<SpecialDropListItem<TransactionCategory>>();
        public ObservableCollection<SpecialDropListItem<TransactionCategory>> CategoryList { get { return _listCategories; } }





        #region Commands

        private RelayCommand _cmdAddNew;
        public RelayCommand CommandAddNew
        {
            get
            {
                return _cmdAddNew ?? (_cmdAddNew = new RelayCommand((o) =>
                {
                    this.Clear();
                }));
            }
        }


        private RelayCommand _cmdSave;
        public RelayCommand CommandSaveChanges
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new RelayCommand((o) =>
                {
                    this.CommitChanges();
                }));
            }
        }

        private RelayCommand _cmdDiscard;
        public RelayCommand CommandDiscardChanges
        {
            get
            {
                return _cmdDiscard ?? (_cmdDiscard = new RelayCommand((o) =>
                {
                    this.DiscardChanges();
                }));
            }
        }

        private RelayCommand _cmdEditRecord;
        public RelayCommand CommandEditRecord
        {
            get
            {
                return _cmdEditRecord ?? (_cmdEditRecord = new RelayCommand((record) =>
                {
                    if(record is BudgetRecordVM vm)
                    {
                        this.Source = vm;
                    }
                }));
            }
        }

        private RelayCommand _cmdDeleteRecord;
        public RelayCommand CommandDeleteRecord
        {
            get
            {
                return _cmdDeleteRecord ?? (_cmdDeleteRecord = new RelayCommand((record) =>
                {
                    if(record is BudgetRecordVM vm)
                    {
                        this.RemoveBudgetRecord(vm);
                    }
                }));
            }
        }

        #endregion








        public BudgetPlannerVM(ITrackerConfig config, IBudgetPlanner budget) : base()
        {
            _budget = budget;
            _config = config;

            this.LoadBudget();
            this.Clear();
        }
        ~BudgetPlannerVM() { this.Dispose(); }


        private void Clear()
        {
            this.Source = new BudgetRecordVM(_config);
        }

        private void LoadBudget()
        {
            this.ExpenseBudgetList.Clear();
            this.IncomeBudgetList.Clear();

            if (!_budget.BudgetRecordList.Any()) return;
            foreach (var record in _budget.BudgetRecordList)
            {
                if (record is BudgetRecord data)
                {
                    if (data.Category.CategoryType == CategoryType.Expense)
                    {
                        this.ExpenseBudgetList.Add(new BudgetRecordVM(_config, data));
                    }
                    else
                    {
                        this.IncomeBudgetList.Add(new BudgetRecordVM(_config, data));
                    }
                }
            }
        }

        private void LoadCategories()
        {
            this.CategoryList.Clear();
            List<TransactionCategory> categories = null;
            if (this.IsExpense)
            {
                categories = _config.CategoryList.Where(x => x.CategoryType == CategoryType.Expense).ToList();
            }
            else
            {
                categories = _config.CategoryList.Where(x => x.CategoryType == CategoryType.Income).ToList();
            }
            if (categories is null || !categories.Any()) return;

            foreach (var cat in categories)
            {
                this.CategoryList.Add(new SpecialDropListItem<TransactionCategory>(cat.Name, cat));
            }
        }

        private void CommitChanges()
        {
            _budget.ClearBudget();
            if (this.ExpenseBudgetList.Any())
            {
                foreach (var exp in this.ExpenseBudgetList)
                {
                    _budget.AddBudget(exp.GetSource());
                }
            }

            if (this.IncomeBudgetList.Any())
            {
                foreach (var inc in this.IncomeBudgetList)
                {
                    _budget.AddBudget(inc.GetSource());
                }
            }

            _budget.SaveToFile();
        }

        private void DiscardChanges()
        {
            this.LoadBudget();
        }

        private void RemoveBudgetRecord(BudgetRecordVM record)
        {
            if (record is null) throw new ArgumentNullException("Budget Record VM");
            if (this.ExpenseBudgetList.Contains(record))
            {
                this.ExpenseBudgetList.Remove(record);
            }
            else if (this.IncomeBudgetList.Contains(record))
            {
                this.IncomeBudgetList.Remove(record);
            }
        }




        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.IsExpense));
            NotifyPropertyChanged(nameof(this.SelectedCategory));
            NotifyPropertyChanged(nameof(this.Description));
            NotifyPropertyChanged(nameof(this.Amount));
            NotifyPropertyChanged(nameof(this.Recurrence));
            NotifyPropertyChanged(nameof(this.RecurrenceType));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if(!(_listCategories is null))
            {
                _listCategories.Clear();
                _listCategories = null;
            }

            if(!(_listExpenses is null))
            {
                _listExpenses.Clear();
                _listExpenses = null;
            }

            if(!(_listIncome is null))
            {
                _listIncome.Clear();
                _listIncome = null;
            }

            _record = null;
            _config = null;
            _budget = null;
        }
    }
}
