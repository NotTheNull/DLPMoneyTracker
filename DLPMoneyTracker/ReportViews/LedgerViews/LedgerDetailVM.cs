using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.Common;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker.ReportViews.LedgerViews
{
    public struct LedgerDetailFilter
    {
        public readonly MoneyAccount Account;
        public readonly TransactionCategory Category;
        public readonly DateRange FilterDates;
        public readonly string SearchText;

        public bool IsFilterEnabled
        {
            get
            {
                if (this.Account != null) return true;
                if (this.Category != null) return true;
                if (this.FilterDates != null) return true;
                if (!string.IsNullOrWhiteSpace(this.SearchText)) return true;

                return false;
            }
        }

        public LedgerDetailFilter(MoneyAccount act) : this(act, null, null, string.Empty) { }
        public LedgerDetailFilter(TransactionCategory cat) : this(null, cat, null, string.Empty) { }
        public LedgerDetailFilter(DateRange dates) : this(null, null, dates, string.Empty) { }
        public LedgerDetailFilter(string srch) : this(null, null, null, srch) { }
        public LedgerDetailFilter(TransactionCategory cat, DateRange dates) : this(null, cat, dates, string.Empty) { }

        public LedgerDetailFilter(MoneyAccount act, TransactionCategory cat, DateRange dates, string srch)
        {
            this.Account = act;
            this.Category = cat;
            this.FilterDates = dates;
            this.SearchText = srch;
        }
    }


    public abstract class LedgerDetailVM : BaseViewModel
    {
        protected readonly ITrackerConfig _config;
        protected readonly ILedger _ledger;

        protected ObservableCollection<MoneyRecord> _listRecords = new ObservableCollection<MoneyRecord>();
        public ObservableCollection<MoneyRecord> DisplayRecordsList { get { return _listRecords; } }

        public string LedgerPath { get { return _ledger.FilePath; } }

        public abstract string HeaderText { get; }
        
        public abstract bool IsCloseButtonVisible { get; }

        private RelayCommand _cmdRefresh;

        public RelayCommand CommandRefresh
        {
            get { return _cmdRefresh ?? (_cmdRefresh = new RelayCommand((o) => this.Reload())); }
        }

        public LedgerDetailVM(ILedger ledger, ITrackerConfig config) : base()
        {
            _config = config;
            _ledger = ledger;

            _ledger.LedgerModified += _ledger_LedgerModified;
        }

        private void _ledger_LedgerModified()
        {
            this.Reload();
        }

        public void Clear()
        {
            _listRecords.Clear();
        }

        public abstract void Reload();
        
        protected void LoadRecords(IEnumerable<IMoneyRecord> records)
        {
            if (records?.Any() != true) return;

            foreach (var rec in records.OrderBy(o => o.TransDate).ThenBy(o => o.Description))
            {
                if (rec is MoneyRecord data)
                {
                    _listRecords.Add(data);
                }
            }
        }
    }

    public class MoneyAccountLedgerDetailVM : LedgerDetailVM
    {
        private readonly MoneyAccount _act;
        private readonly DateRange _dates;

        public MoneyAccountLedgerDetailVM(MoneyAccount act, ILedger ledger, ITrackerConfig config) : this(act, null, ledger, config) { }

        public MoneyAccountLedgerDetailVM(MoneyAccount act, DateRange dates, ILedger ledger, ITrackerConfig config) : base(ledger, config)
        {
            _act = act;
            _dates = dates;
            NotifyPropertyChanged(nameof(this.HeaderText));
            this.Reload();
        }

        public override string HeaderText { get { return string.Format("ACCOUNT: {0}", _act?.Description ?? "** N/A **"); } }

        public override bool IsCloseButtonVisible { get { return true; } }

        public override void Reload()
        {
            this.Clear();
            if (_ledger.TransactionList is null) return;

            var records = _ledger.TransactionList.Where(x => x.AccountID == _act.ID);
            if (_dates != null)
            {
                records = records.Where(x => x.TransDate >= _dates.Begin && x.TransDate <= _dates.End);
            }

            this.LoadRecords(records);
        }
    }

    public class TransactionCategoryLedgerDetailVM : LedgerDetailVM
    {
        private readonly TransactionCategory _cat;
        private readonly DateRange _dates;

        public TransactionCategoryLedgerDetailVM(TransactionCategory cat, ILedger ledger, ITrackerConfig config) : this(cat, null, ledger, config) { }
        public TransactionCategoryLedgerDetailVM(TransactionCategory cat, DateRange range, ILedger ledger, ITrackerConfig config) : base(ledger, config)
        {
            _cat = cat;
            _dates = range;
            NotifyPropertyChanged(nameof(this.HeaderText));
            this.Reload();
        }

        public override string HeaderText { get { return string.Format("CATEGORY: {0}", _cat?.Name ?? "** N/A **"); } }

        public override bool IsCloseButtonVisible { get { return true; } }

        public override void Reload()
        {
            this.Clear();
            if (_ledger.TransactionList is null) return;

            var records = _ledger.TransactionList.Where(x => x.CategoryUID == _cat.ID);
            if(_dates != null)
            {
                records = records.Where(x => x.TransDate >= _dates.Begin && x.TransDate <= _dates.End);
            }

            this.LoadRecords(records);
        }
    }

    public class StandardLedgerDetailVM : LedgerDetailVM
    {
        private LedgerDetailFilter _filter;

        public StandardLedgerDetailVM(ILedger ledger, ITrackerConfig config) : base(ledger, config)
        {
            this.LoadRecords(ledger.TransactionList);
        }
        public StandardLedgerDetailVM(LedgerDetailFilter filter, ILedger ledger, ITrackerConfig config) : base(ledger, config)
        {
            _filter = filter;
            this.Reload();
        }

        public override string HeaderText { get { return string.Empty; } }

        public override bool IsCloseButtonVisible { get { return false; } }

        public override void Reload()
        {
            this.Clear();
            var records = _ledger.TransactionList.ToList();

            if(_filter.IsFilterEnabled)
            {
                if(_filter.Account != null)
                {
                    records = records.Where(x => x.AccountID == _filter.Account.ID).ToList();
                }

                if(_filter.Category != null)
                {
                    records = records.Where(x => x.CategoryUID == _filter.Category.ID).ToList();
                }

                if(_filter.FilterDates != null)
                {
                    records = records.Where(x => x.TransDate >= _filter.FilterDates.Begin && x.TransDate <= _filter.FilterDates.End).ToList();
                }

                if(!string.IsNullOrWhiteSpace(_filter.SearchText))
                {
                    records = records.Where(x => x.Description.Contains(_filter.SearchText)).ToList();
                }
            }

            this.LoadRecords(records);
        }

        public void SetFilter(LedgerDetailFilter filter)
        {
            _filter = filter;
            this.Reload();
        }
    }
}