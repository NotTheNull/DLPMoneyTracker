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

namespace DLPMoneyTracker.ReportViews.HistoricalViews
{
    public class MonthlyHistoricalVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly ILedger _ledger;


        public MonthlyHistoricalVM(ITrackerConfig config, ILedger ledger)
        {
            _config = config;
            _ledger = ledger;

            this.Init();
        }

        #region Properties

        private int _year;

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                NotifyPropertyChanged(nameof(this.Year));
            }
        }

        private int _month;

        public int Month
        {
            get { return _month; }
            set
            {
                _month = value;
                NotifyPropertyChanged(nameof(this.Month));
            }
        }

        public decimal TotalIncome { get { return _incomes.Count > 0 ? _incomes.Sum(s => s.MonthlyTotal) : decimal.Zero; } }
        public decimal TotalExpense { get { return _expenses.Count > 0 ? _expenses.Sum(s => s.MonthlyTotal) : decimal.Zero; } }
        public decimal Balance { get { return this.TotalIncome - this.TotalExpense; } }

        private List<SpecialDropListItem<int>> _months;
        public ReadOnlyCollection<SpecialDropListItem<int>> CalendarMonths { get { return _months.AsReadOnly(); } }

        private List<MonthlyHistoricalRecord> _incomes = new List<MonthlyHistoricalRecord>();
        public ReadOnlyCollection<MonthlyHistoricalRecord> IncomeRecords { get { return _incomes.AsReadOnly(); } }

        private List<MonthlyHistoricalRecord> _expenses = new List<MonthlyHistoricalRecord>();
        public ReadOnlyCollection<MonthlyHistoricalRecord> ExpenseRecords { get { return _expenses.AsReadOnly(); } }

        #endregion


        #region Commands
        private RelayCommand _cmdRefresh;
        public RelayCommand CommandRefresh
        {
            get
            {
                return _cmdRefresh ?? (_cmdRefresh = new RelayCommand((o) =>
                {
                    this.Load();
                }));
            }
        }
        #endregion

        private void Init()
        {
            _months.Add(new SpecialDropListItem<int>("January", 1));
            _months.Add(new SpecialDropListItem<int>("February", 2));
            _months.Add(new SpecialDropListItem<int>("March", 3));
            _months.Add(new SpecialDropListItem<int>("April", 4));
            _months.Add(new SpecialDropListItem<int>("May", 5));
            _months.Add(new SpecialDropListItem<int>("June", 6));
            _months.Add(new SpecialDropListItem<int>("July", 7));
            _months.Add(new SpecialDropListItem<int>("August", 8));
            _months.Add(new SpecialDropListItem<int>("September", 9));
            _months.Add(new SpecialDropListItem<int>("October", 10));
            _months.Add(new SpecialDropListItem<int>("November", 11));
            _months.Add(new SpecialDropListItem<int>("December", 12));

        }

        private void Load()
        {
            if (this.Year > DateTime.Today.Year) throw new InvalidOperationException("Cannot use a future year");
            if (this.Year == DateTime.Today.Year && this.Month > DateTime.Today.Month) throw new InvalidOperationException("Cannot use a future Month");

            _config.LoadFromFile(this.Year);
            _ledger.LoadFromFile(this.Year);

            _incomes.Clear();
            foreach(var cat in _config.CategoryList.Where(x => x.CategoryType == CategoryType.Income))
            {
                MonthlyHistoricalRecord record = new MonthlyHistoricalRecord(_config, _ledger);
                record.LoadCategory(cat, this.Month);
                _incomes.Add(record);
            }

            _expenses.Clear();
            foreach(var cat in _config.CategoryList.Where(x => x.CategoryType == CategoryType.Expense || x.CategoryType == CategoryType.Payment))
            {
                MonthlyHistoricalRecord record = new MonthlyHistoricalRecord(_config, _ledger);
                record.LoadCategory(cat, this.Month);
                _expenses.Add(record);
            }

            this.NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyPropertyChanged(nameof(this.Year));
            NotifyPropertyChanged(nameof(this.Month));
            NotifyPropertyChanged(nameof(this.IncomeRecords));
            NotifyPropertyChanged(nameof(this.ExpenseRecords));
            NotifyPropertyChanged(nameof(this.TotalIncome));
            NotifyPropertyChanged(nameof(this.TotalExpense));
            NotifyPropertyChanged(nameof(this.Balance));
        }

    }
}
