using DLPMoneyTracker.Core;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.ConfigModels;
using DLPMoneyTracker.Data.TransactionModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DLPMoneyTracker.ReportViews.LedgerViews
{
    public class LedgerDetailVM : BaseViewModel
    {
        ITrackerConfig _config;
        ILedger _ledger;

        private MoneyAccount _act;
        private TransactionCategory _cat;

        private ObservableCollection<MoneyRecord> _listRecords = new ObservableCollection<MoneyRecord>();
        public ObservableCollection<MoneyRecord> DisplayRecordsList { get { return _listRecords; } }


        public string HeaderText
        {
            get
            {
                if(!(_act is null)) return string.Format("ACCOUNT: {0}", _act.Description);
                if (!(_cat is null)) return string.Format("CATEGORY: {0}", _cat.Name);

                return string.Empty;
            }
        }




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
            _act = null;
            _cat = null;
        }

        public void Reload()
        {
            _listRecords.Clear();
            if(!(_act is null))
            {
                this.LoadRecords(_ledger.TransactionList.Where(x => x.AccountID == _act.ID));
            }
            else if(!(_cat is null))
            {
                this.LoadRecords(_ledger.TransactionList.Where(x => x.CategoryUID == _cat.ID));
            }
            else
            {
                this.LoadRecords(_ledger.TransactionList);
            }
        }
        

        public void ShowFullLedgerDetail()
        {
            this.Clear();
            this.LoadRecords(_ledger.TransactionList);
        }

        public void ShowAccountDetail(MoneyAccount act)
        {
            this.Clear();
            _act = act;
            NotifyPropertyChanged(nameof(this.HeaderText));

            if (_ledger.TransactionList is null) return;
            this.LoadRecords(_ledger.TransactionList.Where(x => x.AccountID == _act.ID));
        }

        public void ShowCategoryDetail(TransactionCategory cat)
        {
            this.Clear();
            _cat = cat;
            NotifyPropertyChanged(nameof(this.HeaderText));

            if (_ledger.TransactionList is null) return;
            this.LoadRecords(_ledger.TransactionList.Where(x => x.CategoryUID == _cat.ID));
        }


        private void LoadRecords(IEnumerable<IMoneyRecord> records)
        {
            if (records is null || !records.Any()) return;
            
            foreach(var rec in records)
            {
                if(rec is MoneyRecord data)
                {
                    _listRecords.Add(data);
                }
            }
        }

    }
}
