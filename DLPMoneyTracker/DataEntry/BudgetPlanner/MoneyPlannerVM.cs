using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.ScheduleRecurrence;
using DLPMoneyTracker.Data.TransactionModels.BillPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker.DataEntry.BudgetPlanner
{
    /// <summary>
    /// A Money Plan is a fixed recurring expense or income that has, hopefully, limited variability.
    /// Such items include Utility Bills, Subscriptions, Paychecks, etc
    /// </summary>

    public class MoneyPlannerVM : BaseViewModel, IDisposable
    {
        private ITrackerConfig _config;
        private IMoneyPlanner _moneyPlanner;

        #region Editing Control Properties

        private MoneyPlanRecordVM _record;

        private MoneyPlanRecordVM Source
        {
            get { return _record; }
            set
            {
                _record = value;
                if (_record?.Category?.CategoryType is null)
                {
                    this.IsExpense = true;
                }
                else
                {
                    this.IsExpense = _record.Category.CategoryType == CategoryType.Expense;
                }

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
                if (this.Source is null) return;
                this.Source.Category = value;
                NotifyPropertyChanged(nameof(this.SelectedCategory));
            }
        }

        public MoneyAccount SelectedAccount
        {
            get { return this.Source?.Account; }
            set
            {
                if (this.Source is null) return;
                this.Source.Account = value;
                NotifyPropertyChanged(nameof(this.SelectedAccount));
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

        #endregion Editing Control Properties

        private ObservableCollection<MoneyPlanRecordVM> _listExpenses = new ObservableCollection<MoneyPlanRecordVM>();
        public ObservableCollection<MoneyPlanRecordVM> ExpenseList { get { return _listExpenses; } }

        private ObservableCollection<MoneyPlanRecordVM> _listIncome = new ObservableCollection<MoneyPlanRecordVM>();
        public ObservableCollection<MoneyPlanRecordVM> IncomeList { get { return _listIncome; } }

        private ObservableCollection<SpecialDropListItem<TransactionCategory>> _listCategories = new ObservableCollection<SpecialDropListItem<TransactionCategory>>();
        public ObservableCollection<SpecialDropListItem<TransactionCategory>> CategoryList { get { return _listCategories; } }

        private ObservableCollection<SpecialDropListItem<MoneyAccount>> _listAccounts = new ObservableCollection<SpecialDropListItem<MoneyAccount>>();
        public ObservableCollection<SpecialDropListItem<MoneyAccount>> AccountList { get { return _listAccounts; } }

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
                    if (record is MoneyPlanRecordVM vm)
                    {
                        this.Source = vm;
                    }
                }));
            }
        }

        private RelayCommand _cmdAddRecord;

        public RelayCommand CommandAddRecord
        {
            get
            {
                return _cmdAddRecord ?? (_cmdAddRecord = new RelayCommand((o) =>
                {
                    this.AddMoneyPlanRecord();
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
                    if (record is MoneyPlanRecordVM vm)
                    {
                        this.RemoveMoneyPlanRecord(vm);
                    }
                }));
            }
        }

        private RelayCommand _cmdClear;

        public RelayCommand CommandClear
        {
            get
            {
                return _cmdClear ?? (_cmdClear = new RelayCommand((o) =>
                {
                    this.Clear();
                }));
            }
        }

        #endregion Commands

        public MoneyPlannerVM(ITrackerConfig config, IMoneyPlanner planner) : base()
        {
            _moneyPlanner = planner;
            _config = config;

            this.LoadAccounts();
            this.LoadMoneyPlan();
            this.Clear();
        }

        ~MoneyPlannerVM()
        {
            this.Dispose();
        }

        private void Clear()
        {
            this.Source = new MoneyPlanRecordVM(_config);
        }

        private void LoadMoneyPlan()
        {
            this.ExpenseList.Clear();
            this.IncomeList.Clear();

            if (!_moneyPlanner.MoneyPlanList.Any()) return;
            foreach (var record in _moneyPlanner.MoneyPlanList)
            {
                if (record is IncomePlan income)
                {
                    this.IncomeList.Add(new MoneyPlanRecordVM(_config, income));
                }
                else if (record is ExpensePlan expense)
                {
                    this.ExpenseList.Add(new MoneyPlanRecordVM(_config, expense));
                }
            }
        }

        private void LoadAccounts()
        {
            List<MoneyAccountType> allowedAccounts = new List<MoneyAccountType>() { MoneyAccountType.Checking, MoneyAccountType.Savings, MoneyAccountType.CreditCard };
            this.AccountList.Clear();
            if (_config.AccountsList.Any(x => allowedAccounts.Contains(x.AccountType)))
            {
                foreach (var act in _config.AccountsList.Where(x => allowedAccounts.Contains(x.AccountType)))
                {
                    this.AccountList.Add(new SpecialDropListItem<MoneyAccount>(act.Description, act));
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
            _moneyPlanner.ClearRecordList();
            if (this.ExpenseList.Any())
            {
                foreach (var exp in this.ExpenseList)
                {
                    _moneyPlanner.AddMoneyPlan(exp.GetSource());
                }
            }

            if (this.IncomeList.Any())
            {
                foreach (var inc in this.IncomeList)
                {
                    _moneyPlanner.AddMoneyPlan(inc.GetSource());
                }
            }

            _moneyPlanner.SaveToFile();
        }

        private void DiscardChanges()
        {
            this.LoadMoneyPlan();
        }

        private void AddMoneyPlanRecord()
        {
            if (this.IsExpense)
            {
                if (!this.ExpenseList.Contains(this.Source))
                {
                    this.ExpenseList.Add(this.Source);
                }
            }
            else
            {
                if (!this.IncomeList.Contains(this.Source))
                {
                    this.IncomeList.Add(this.Source);
                }
            }
        }

        private void RemoveMoneyPlanRecord(MoneyPlanRecordVM record)
        {
            if (record is null) throw new ArgumentNullException("Budget Record VM");
            if (this.ExpenseList.Contains(record))
            {
                this.ExpenseList.Remove(record);
            }
            else if (this.IncomeList.Contains(record))
            {
                this.IncomeList.Remove(record);
            }
        }

        public void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.IsExpense));
            NotifyPropertyChanged(nameof(this.SelectedCategory));
            NotifyPropertyChanged(nameof(this.SelectedAccount));
            NotifyPropertyChanged(nameof(this.Description));
            NotifyPropertyChanged(nameof(this.Amount));
            NotifyPropertyChanged(nameof(this.Recurrence));
            NotifyPropertyChanged(nameof(this.RecurrenceType));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (!(_listCategories is null))
            {
                _listCategories.Clear();
                _listCategories = null;
            }

            if (!(_listExpenses is null))
            {
                _listExpenses.Clear();
                _listExpenses = null;
            }

            if (!(_listIncome is null))
            {
                _listIncome.Clear();
                _listIncome = null;
            }

            _record = null;
            _config = null;
            _moneyPlanner = null;
        }
    }
}