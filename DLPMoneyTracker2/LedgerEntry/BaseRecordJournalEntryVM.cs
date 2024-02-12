using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public interface IJournalEntryVM
    {
        bool IsNew { get; }
        bool IsValidTransaction { get; }
        DateTime TransactionDate { get; }
        TransactionType JournalEntryType { get; }

        string DebitHeader { get; }
        ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts { get; }
        IJournalAccount SelectedDebitAccount { get; }
        string DebitAccountName { get; }
        DateTime? DebitBankDate { get; set; }

        string CreditHeader { get; }
        ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts { get; }
        IJournalAccount SelectedCreditAccount { get; }
        string CreditAccountName { get; }
        DateTime? CreditBankDate { get; set; }

        bool IsCreditEnabled { get; }
        bool IsCreditBankDateVisible { get; }
        bool IsDebitBankDateVisible { get; }

        bool CanUserEditCreditAccount { get; }
        bool CanUserEditDebitAccount { get; }


        string Description { get; }
        decimal Amount { get; }





        void Clear();

        void LoadAccounts();
        void LoadTransaction(IJournalEntry entry);

        void SaveTransaction();

        void FillFromPlan(IJournalPlan plan);
    }

    public abstract class BaseRecordJournalEntryVM : BaseViewModel, IJournalEntryVM
    {
        protected readonly IJournal _journal;
        protected readonly ITrackerConfig _config;

        private readonly List<JournalAccountType> _validDebitTypes = new List<JournalAccountType>();
        private readonly List<JournalAccountType> _validCreditTypes = new List<JournalAccountType>();

        protected BaseRecordJournalEntryVM(
            IJournal journal, 
            ITrackerConfig config,  
            IEnumerable<JournalAccountType> validDebitTypes, 
            IEnumerable<JournalAccountType> validCreditTypes,
            TransactionType transType)
        {
            _config = config;
            _journal = journal;
            _transType = transType;
            _date = DateTime.Today;
            _validDebitTypes.AddRange(validDebitTypes);
            _validCreditTypes.AddRange(validCreditTypes);
            this.LoadAccounts();
            this.NotifyAll();
        }


        protected Guid? ExistingTransactionId { get; set; } = null;


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

        private TransactionType _transType;
        public TransactionType JournalEntryType { get { return _transType; } }


        public bool IsNew { get { return !this.ExistingTransactionId.HasValue; } }
        public abstract bool IsValidTransaction { get; }

        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidDebits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();

        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts
        { get { return _listValidDebits; } }

        public abstract string DebitHeader { get; }

        protected IJournalAccount _debit;

        public IJournalAccount? SelectedDebitAccount
        {
            get { return _debit; }
            set
            {
                _debit = value;
                NotifyPropertyChanged(nameof(SelectedDebitAccount));
                NotifyPropertyChanged(nameof(IsDebitBankDateVisible));
            }
        }
        public string DebitAccountName { get { return this.SelectedDebitAccount?.Description ?? string.Empty; } }


        private DateTime? _debitBankDate;

        public DateTime? DebitBankDate
        {
            get { return _debitBankDate; }
            set 
            {
                _debitBankDate = value;
                NotifyPropertyChanged(nameof(DebitBankDate));
            }
        }
        public virtual bool IsDebitBankDateVisible
        {
            get
            {
                return !IsNew && (_debit is IMoneyAccount);
            }
        }



		public bool CanUserEditCreditAccount 
        {
            get
            {
                if (this.IsNew) return true;
                if(SelectedCreditAccount != null)
                    return !SelectedCreditAccount.DateClosedUTC.HasValue;

                return true;
            } 
        }
		public bool CanUserEditDebitAccount 
        { 
            get
            {
                if (this.IsNew) return true;
                if(SelectedDebitAccount != null)
                    return !SelectedDebitAccount.DateClosedUTC.HasValue;

                return true;
            }
        }
    



		public virtual bool IsCreditEnabled        { get { return CanUserEditCreditAccount; } }

        public virtual bool IsCreditBankDateVisible
        {
            get
            {
                return !IsNew && IsCreditEnabled && (_credit is IMoneyAccount);
            }
        }


        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();

        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts        { get { return _listValidCredits; } }

        public abstract string CreditHeader { get; }

        protected IJournalAccount _credit;

        public IJournalAccount? SelectedCreditAccount
        {
            get { return _credit; }
            set
            {
                _credit = value;
                NotifyPropertyChanged(nameof(SelectedCreditAccount));
                NotifyPropertyChanged(nameof(IsCreditBankDateVisible));
            }
        }
        public string CreditAccountName { get { return this.SelectedCreditAccount?.Description ?? string.Empty; } }

        private DateTime? _creditBankDate;

        public DateTime? CreditBankDate
        {
            get { return _creditBankDate; }
            set 
            { 
                _creditBankDate = value;
                NotifyPropertyChanged(nameof(CreditBankDate));
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
            this.Amount = decimal.Zero;
            this.Description = string.Empty;
            this.SelectedCreditAccount = null;
            this.SelectedDebitAccount = null;
        }



        public virtual void LoadAccounts()
        {
            this.ValidCreditAccounts.Clear();
            var listCredits = _config.GetJournalAccountList(new JournalAccountSearch(_validCreditTypes));
            if (listCredits?.Any() == true)
            {
                foreach (var c in listCredits.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(c.Description, c));
                }
            }

            this.ValidDebitAccounts.Clear();
            var listDebits = _config.GetJournalAccountList(new JournalAccountSearch(_validDebitTypes));
            if (listDebits?.Any() == true)
            {
                foreach (var d in listDebits.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(d.Description, d));
                }
            }
        }

        public virtual void LoadTransaction(IJournalEntry entry)
        {
            if (entry is null) throw new ArgumentNullException(nameof(IJournalEntry));

            this.ExistingTransactionId = entry.Id;
            this.TransactionDate = entry.TransactionDate;
            this.Amount = entry.TransactionAmount;
            this.Description = entry.Description;
            this.SelectedCreditAccount = _config.GetJournalAccount(entry.CreditAccountId);
            this.SelectedDebitAccount = _config.GetJournalAccount(entry.DebitAccountId);
            this.CreditBankDate = entry.CreditBankDate;
            this.DebitBankDate = entry.DebitBankDate;
        }

        /// <summary>
        /// If you override this save, make sure to check for the Existing Transaction
        /// </summary>
        public virtual void SaveTransaction()
        {
            if (!IsValidTransaction) return;

            JournalEntry record = new JournalEntry(_config)
            {
                JournalEntryType = this.JournalEntryType,
                CreditAccount = this.SelectedCreditAccount,
                CreditBankDate = this.CreditBankDate,
                DebitAccount = this.SelectedDebitAccount,
                DebitBankDate = this.DebitBankDate,
                TransactionAmount = this.Amount,
                TransactionDate = this.TransactionDate,
                Description = this.Description
            };

            if(this.ExistingTransactionId.HasValue)
            {
                record.Id = this.ExistingTransactionId.Value;
            }

            _journal.AddUpdateTransaction(record);
        }

        public void FillFromPlan(IJournalPlan plan)
        {
            this.TransactionDate = DateTime.Today;
            this.SelectedCreditAccount = _config.GetJournalAccount(plan.CreditAccountId);
            this.SelectedDebitAccount = _config.GetJournalAccount(plan.DebitAccountId);
            this.Description = plan.Description;
            this.Amount = plan.ExpectedAmount;
        }

        protected void NotifyAll()
        {
            NotifyPropertyChanged(nameof(Amount));
            NotifyPropertyChanged(nameof(Description));
            NotifyPropertyChanged(nameof(this.SelectedDebitAccount));
            NotifyPropertyChanged(nameof(SelectedCreditAccount));
        }
    }
}