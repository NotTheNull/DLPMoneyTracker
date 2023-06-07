﻿using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Main.TransactionList
{
    public class AccountTransactionDetailVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

        public AccountTransactionDetailVM(ITrackerConfig config, IJournal journal)
        {
            _config = config;
            _journal = journal;

            _journal.JournalModified += _journal_JournalModified;
        }

        public string HeaderText
        {
            get { return string.Format("{0}: {1}", this.FilterAccount.JournalType.ToDisplayText().ToUpper(), this.FilterAccount.Description); }
        }

        private ObservableCollection<SingleAccountDetailVM> _listRecords = new ObservableCollection<SingleAccountDetailVM>();
        public ObservableCollection<SingleAccountDetailVM> DisplayRecordsList { get { return _listRecords; } }


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
        
        public bool AreFiltersVisible
        {
            get
            {
                return _filter.AreFilterControlsVisible;
            }
        }
        


        #endregion


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
        #endregion



        /// <summary>
        /// Copies the filter options into the model's filter.
        /// By setting the values in this manner, we avoid the issue of zombie memory objects
        /// with classes being passed by reference.
        /// </summary>
        /// <param name="filter"></param>
        public void ApplyFilters(TransDetailFilter filter)
        {
            _filter = filter;
            NotifyPropertyChanged(nameof(this.FilterAccount));
            NotifyPropertyChanged(nameof(this.FilterBeginDate));
            NotifyPropertyChanged(nameof(this.FilterEndDate));
            NotifyPropertyChanged(nameof(this.FilterText));
            //this.FilterBeginDate = filter.FilterDates?.Begin ?? DateTime.MinValue;
            //this.FilterEndDate = filter.FilterDates?.End ?? DateTime.MaxValue;
            //this.FilterText = filter.SearchText?.Trim() ?? string.Empty;
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
                SingleAccountDetailVM vm = new SingleAccountDetailVM(FilterAccount, rec);
                _listRecords.Add(vm);
            }
        }
    }


    public class SingleAccountDetailVM : BaseViewModel
    {
        private readonly IJournalEntry _je;
        private readonly IJournalAccount _parent;

        public SingleAccountDetailVM(IJournalAccount parent, IJournalEntry entry) : base()
        {
            _je = entry;
            _parent = parent;
        }

        public Guid ParentId { get { return _parent.Id; } }
        public bool IsCredit { get { return _je.CreditAccountId == ParentId; } }
        public string AccountName { get { return IsCredit ? _je.DebitAccountName : _je.CreditAccountName; } }
        public string TransactionDescription { get { return _je.Description; } }
        public decimal TransactionAmount
        {
            get
            {
                if (_parent.JournalType == JournalAccountType.LiabilityCard || _parent.JournalType == JournalAccountType.LiabilityLoan)
                {
                    return IsCredit ? _je.TransactionAmount : _je.TransactionAmount * -1;
                }
                else
                {
                    return IsCredit ? _je.TransactionAmount * -1 : _je.TransactionAmount;
                }
            }
        }

    }
}