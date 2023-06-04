using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace DLPMoneyTracker.ReportViews.JournalViews
{

    public class JournalDetailVM : BaseViewModel
    {
        private readonly ITrackerConfig _config;
        private readonly IJournal _journal;

        public JournalDetailVM(IJournal journal, ITrackerConfig config)
        {
            _config = config;
            _journal = journal;

            _journal.JournalModified += _journal_JournalModified;
        }

        private ObservableCollection<IJournalEntry> _listRecords;
        public ObservableCollection<IJournalEntry> DisplayRecordsList { get { return _listRecords; } }

        #region Filter Related


        private JournalDetailFilter _filter = new JournalDetailFilter();

        public IJournalAccount FilterAccount
        {
            get { return _filter.LedgerAccount; }
            set
            {
                _filter.LedgerAccount = value;
                NotifyPropertyChanged(nameof(FilterAccount));
            }
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
                        _filter ??= new JournalDetailFilter();
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
        public void ApplyFilters(JournalDetailFilter filter)
        {
            this.FilterAccount = filter.LedgerAccount;
            this.FilterBeginDate = filter.FilterDates?.Begin ?? DateTime.MinValue;
            this.FilterEndDate = filter.FilterDates?.End ?? DateTime.MaxValue;
            this.FilterText = filter.SearchText?.Trim();
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
                if (FilterAccount != null)
                {
                    records = records.Where(x => x.CreditAccountId == FilterAccount.Id || x.DebitAccountId == FilterAccount.Id);
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
                    //if (FilterAccount is null)
                    //{
                    //    _listRecords.Add(new SingleAccountJournalEntry(je, false));
                    //    _listRecords.Add(new SingleAccountJournalEntry(je, true));
                    //}
                    //else
                    //{
                    //    _listRecords.Add(new SingleAccountJournalEntry(je, FilterAccount.Id == je.CreditAccountId))
                    //}
                    _listRecords.Add(je);
                }
            }
        }
    }

    public class JournalDetailFilter
    {
        public IJournalAccount LedgerAccount;
        public DateRange FilterDates;
        public string SearchText;

        public bool IsFilterEnabled
        {
            get
            {
                if (this.LedgerAccount != null) return true;
                if (this.FilterDates != null) return true;
                if (this.FilterDates.Begin > DateTime.MinValue || this.FilterDates.End < DateTime.MaxValue) return true;
                if (!string.IsNullOrWhiteSpace(this.SearchText)) return true;

                return false;
            }
        }

        public JournalDetailFilter() { }
        public JournalDetailFilter(IJournalAccount account, DateRange dates, string search)
        {
            this.LedgerAccount = account;
            this.FilterDates = dates ?? new DateRange(DateTime.MinValue, DateTime.MaxValue);
            this.SearchText = search;
        }

        public void Clear()
        {
            this.LedgerAccount = null;
            this.FilterDates = new DateRange(DateTime.MinValue, DateTime.MaxValue);
            this.SearchText = string.Empty;
        }

    }

    //public class SingleAccountJournalEntry : BaseViewModel
    //{
    //    public SingleAccountJournalEntry() : base()
    //    {
    //        this.UID = Guid.NewGuid();
    //    }
    //    public SingleAccountJournalEntry(JournalEntry je, bool isCredit) : base()
    //    {
    //        this.UID = Guid.NewGuid();
    //        this.Copy(je, isCredit);
    //    }

    //    public Guid UID { get; private set; }

    //    public Guid JournalEntryId { get; set; }


    //    private DateTime _transDate;

    //    public DateTime TransactionDate
    //    {
    //        get { return _transDate; }
    //        set
    //        {
    //            _transDate = value;
    //            NotifyPropertyChanged(nameof(TransactionDate));
    //        }
    //    }

    //    private ILedgerAccount _account;

    //    public ILedgerAccount LedgerAccount
    //    {
    //        get { return _account; }
    //        set
    //        {
    //            _account = value;
    //            NotifyPropertyChanged(nameof(LedgerAccount));
    //            NotifyPropertyChanged(nameof(AccountName));
    //        }
    //    }

    //    public string AccountName { get { return _account?.Description ?? string.Empty; } }


    //    private string _desc;

    //    public string TransactionDescription
    //    {
    //        get { return _desc; }
    //        set
    //        {
    //            _desc = value;
    //            NotifyPropertyChanged(nameof(TransactionDescription));
    //        }
    //    }

    //    private decimal _amt;

    //    public decimal TransactionAmount
    //    {
    //        get { return _amt; }
    //        set
    //        {
    //            _amt = value;
    //            NotifyPropertyChanged(nameof(TransactionAmount));
    //        }
    //    }


    //    public void Copy(JournalEntry entry, bool isCreditAccount)
    //    {
    //        this.JournalEntryId = entry.Id;
    //        this.TransactionDate = entry.TransactionDate;
    //        this.TransactionDescription = entry.Description;
    //        this.LedgerAccount = isCreditAccount ? entry.CreditAccount : entry.DebitAccount;
    //        this.TransactionAmount = isCreditAccount ? entry.TransactionAmount * -1 : entry.TransactionAmount;
    //    }
    //}
}
