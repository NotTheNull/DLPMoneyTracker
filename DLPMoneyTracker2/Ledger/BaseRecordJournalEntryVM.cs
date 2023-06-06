using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Ledger
{
    public abstract class BaseRecordJournalEntryVM : BaseViewModel
    {
        protected readonly IJournal _journal;
        protected readonly ITrackerConfig _config;

        public BaseRecordJournalEntryVM(IJournal journal, ITrackerConfig config)
        {
            _config = config;
            _journal = journal;
            _date = DateTime.Today;
        }

        private DateTime _date;

        public DateTime TransactionDate
        {
            get { return _date; }
            set 
            {
                _date = value;
                NotifyPropertyChanged(nameof(TransactionDate));
            }
        }

        public abstract bool IsValidTransaction { get; }

        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidDebits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts { get { return _listValidDebits; } }

        public abstract string DebitHeader { get; }

        protected IJournalAccount _debit;

        public IJournalAccount SelectedDebitAccount
        {
            get { return _debit; }
            set 
            {
                _debit = value;
                NotifyPropertyChanged(nameof(SelectedDebitAccount));
            }
        }

        public virtual bool IsCreditEnabled { get { return true; } }

        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts { get { return _listValidCredits; } }

        public abstract string CreditHeader { get; }

        protected IJournalAccount _credit;

        public IJournalAccount SelectedCreditAccount
        {
            get { return _credit; }
            set 
            {
                _credit = value;
                NotifyPropertyChanged(nameof(SelectedCreditAccount));
            }
        }

        protected string _desc;

        public string Description
        {
            get { return _desc; }
            set 
            {
                _desc = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }


        protected decimal _amt;

        public decimal Amount
        {
            get { return _amt; }
            set 
            { 
                _amt = value;
                NotifyPropertyChanged(nameof(Amount));
            }
        }



        public void Clear()
        {
            this.TransactionDate = DateTime.Now;
            this.SelectedCreditAccount = null;
            this.SelectedDebitAccount = null;
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
        }

        public abstract void LoadAccounts();

        public void SaveTransaction()
        {
            if (!IsValidTransaction) return;

            JournalEntry record = new JournalEntry(_config)
            {
                CreditAccount = this.SelectedCreditAccount,
                DebitAccount = this.SelectedDebitAccount,
                TransactionAmount = this.Amount,
                TransactionDate = this.TransactionDate,
                Description = this.Description
            };
            _journal.AddTransaction(record);
        }
        

    }
}
