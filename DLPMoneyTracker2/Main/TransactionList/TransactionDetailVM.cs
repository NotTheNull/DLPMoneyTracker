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

        private JournalSearchFilter _filter = new JournalSearchFilter();

        public IJournalAccount? FilterAccount
        {
            get { return _filter.Account; }
        }

        public DateTime FilterBeginDate
        {
            get { return _filter.DateRange.Begin; }
            set
            {
                _filter.DateRange.Begin = value;
                NotifyPropertyChanged(nameof(FilterBeginDate));
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filter.DateRange.End; }
            set
            {
                _filter.DateRange.End = value;
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
                        _filter ??= new JournalSearchFilter();
                        _filter.Clear();
                        NotifyPropertyChanged(nameof(FilterBeginDate));
                        NotifyPropertyChanged(nameof(FilterEndDate));
                        NotifyPropertyChanged(nameof(FilterText));
                        NotifyPropertyChanged(nameof(FilterAccount));
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
        public void ApplyFilters(JournalSearchFilter filter)
        {
            _filter.Account = filter.Account;
            this.FilterBeginDate = filter.DateRange?.Begin ?? DateTime.MinValue;
            this.FilterEndDate = filter.DateRange?.End ?? DateTime.MaxValue;
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
            if (this.IsFiltersVisible)
            {
                if (this.FilterAccount != null)
                {
                    records = records.Where(x => x.DebitAccountId == this.FilterAccount.Id || x.CreditAccountId == this.FilterAccount.Id);
                }

                if (_filter.DateRange != null)
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

    
}