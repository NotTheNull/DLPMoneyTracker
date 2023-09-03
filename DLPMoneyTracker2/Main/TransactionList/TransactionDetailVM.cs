using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.Main.TransactionList
{
    public class TransactionDetailVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public TransactionDetailVM(ITrackerConfig config, IJournal journal) : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _config = config;
            _journal = journal;

            _journal.JournalModified += _journal_JournalModified;
            this.Reload();
        }

        private ObservableCollection<IJournalEntry> _listRecords = new ObservableCollection<IJournalEntry>();
        public ObservableCollection<IJournalEntry> DisplayRecordsList
        { get { return _listRecords; } }

        #region Filter Related

        private TransDetailFilter _filter = new TransDetailFilter();

        public IJournalAccount? FilterAccount
        {
            get { return _filter.Account; }
        }

        public DateTime FilterBeginDate
        {
            get { return _filter.FilterDates.Begin; }
            set
            {
                _filter.FilterDates.Begin = value;
                NotifyPropertyChanged(nameof(FilterBeginDate));
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filter.FilterDates.End; }
            set
            {
                _filter.FilterDates.End = value;
                NotifyPropertyChanged(nameof(FilterEndDate));
            }
        }

        public string FilterText
        {
            get { return _filter.SearchText; }
            set
            {
                _filter.SearchText = value;
                NotifyPropertyChanged(nameof(FilterText));
            }
        }

        private bool _isCloseVis;

        public bool IsCloseButtonVisible
        {
            get { return _isCloseVis; }
            set
            {
                _isCloseVis = value;
                NotifyPropertyChanged(nameof(IsCloseButtonVisible));
            }
        }

        private bool _isFiltered;

        public bool IsFiltersVisible
        {
            get { return _isFiltered; }
            set
            {
                _isFiltered = value;
                NotifyPropertyChanged(nameof(IsFiltersVisible));
            }
        }

        #endregion Filter Related

        #region Commands

        private RelayCommand _cmdRefresh;

        public RelayCommand CommandRefresh
        {
            get { return _cmdRefresh ?? (_cmdRefresh = new RelayCommand((o) => this.Reload())); }
        }

        private RelayCommand _cmdFilter;

        public RelayCommand CommandSearch
        {
            get { return _cmdFilter ?? (_cmdFilter = new RelayCommand((o) => this.Reload())); }
        }

        private RelayCommand _cmdResetFilter;

        public RelayCommand CommandResetFilter
        {
            get
            {
                return _cmdResetFilter ??
                    (
                    _cmdResetFilter = new RelayCommand((o) =>
                    {
                        _filter ??= new TransDetailFilter();
                        _filter.Clear();
                    }

                    ));
            }
        }

        #endregion Commands

        /// <summary>
        /// Copies the filter options into the model's filter.
        /// By setting the values in this manner, we avoid the issue of zombie memory objects
        /// with classes being passed by reference.
        /// </summary>
        /// <param name="filter"></param>
        public void ApplyFilters(TransDetailFilter filter)
        {
            _filter.Account = filter.Account;
            this.FilterBeginDate = filter.FilterDates?.Begin ?? DateTime.MinValue;
            this.FilterEndDate = filter.FilterDates?.End ?? DateTime.MaxValue;
            this.FilterText = filter.SearchText?.Trim() ?? string.Empty;
        }

        private void _journal_JournalModified()
        {
            this.Reload();
        }

        public void Reload()
        {
            this.Clear();
            if (_journal.TransactionList?.Any() != true) return;

            // Testing against NULL to force the VAR class type
            var records = _journal.TransactionList.Where(x => x != null);
            if (_filter?.IsFilterEnabled == true)
            {
                if (this.FilterAccount != null)
                {
                    records = records.Where(x => x.DebitAccountId == this.FilterAccount.Id || x.CreditAccountId == this.FilterAccount.Id);
                }

                if (_filter.FilterDates != null)
                {
                    records = records.Where(x => x.TransactionDate >= FilterBeginDate && x.TransactionDate <= FilterEndDate);
                }

                if (!string.IsNullOrWhiteSpace(FilterText))
                {
                    records = records.Where(x => x.Description.Contains(FilterText.Trim()));
                }
            }
            this.LoadRecords(records);
        }

        public void Clear()
        {
            _listRecords.Clear();
        }

        private void LoadRecords(IEnumerable<IJournalEntry> records)
        {
            if (records?.Any() != true) return;

            foreach (var rec in records.OrderBy(o => o.TransactionDate).ThenBy(o => o.Description))
            {
                if (rec is JournalEntry je)
                {
                    _listRecords.Add(je);
                }
            }
        }
    }

    public class TransDetailFilter
    {
        public IJournalAccount? Account;
        public DateRange FilterDates;
        public string SearchText;
        public bool AreFilterControlsVisible;

        public bool IsFilterEnabled
        {
            get
            {
                if (this.FilterDates != null) return true;
                if (this.FilterDates?.Begin > DateTime.MinValue || this.FilterDates?.End < DateTime.MaxValue) return true;
                if (!string.IsNullOrWhiteSpace(this.SearchText)) return true;

                return false;
            }
        }

        public TransDetailFilter()
        {
            this.FilterDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            this.SearchText = string.Empty;
            AreFilterControlsVisible = true;
        }

        public TransDetailFilter(IJournalAccount account, DateRange dates, string search)
        {
            this.Account = account;
            this.FilterDates = dates ?? new DateRange(DateTime.MinValue, DateTime.MaxValue);
            this.SearchText = search;
            AreFilterControlsVisible = true;
        }

        public void Clear()
        {
            this.FilterDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            this.SearchText = string.Empty;
        }
    }
}