using DLPMoneyTracker.Data;
using DLPMoneyTracker.Data.LedgerAccounts;
using DLPMoneyTracker.Data.TransactionModels;
using DLPMoneyTracker.Data.TransactionModels.JournalPlan;
using DLPMoneyTracker2.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DLPMoneyTracker2.LedgerEntry
{
    public interface IJournalEntryVM
    {
        bool IsValidTransaction { get; }
        DateTime TransactionDate { get; }

        string DebitHeader { get; }
        ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts { get; }
        IJournalAccount SelectedDebitAccount { get; }


        string CreditHeader { get; }
        ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts { get; }
        IJournalAccount SelectedCreditAccount { get; }

        bool IsCreditEnabled { get; }

        string Description { get; }
        decimal Amount { get; }

        void Clear();

        void LoadAccounts();

        void SaveTransaction();

        void FillFromPlan(IJournalPlan plan);
    }

    public abstract class BaseRecordJournalEntryVM : BaseViewModel, IJournalEntryVM
    {
        protected readonly IJournal _journal;
        protected readonly ITrackerConfig _config;

        private readonly List<JournalAccountType> _validDebitTypes = new List<JournalAccountType>();
        private readonly List<JournalAccountType> _validCreditTypes = new List<JournalAccountType>();

        public BaseRecordJournalEntryVM(IJournal journal, ITrackerConfig config, IEnumerable<JournalAccountType> validDebitTypes, IEnumerable<JournalAccountType> validCreditTypes)
        {
            _config = config;
            _journal = journal;
            _date = DateTime.Today;
            _validDebitTypes.AddRange(validDebitTypes);
            _validCreditTypes.AddRange(validCreditTypes);
            this.LoadAccounts();
            this.NotifyAll();
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
            }
        }

        public virtual bool IsCreditEnabled { get { return true; } }


        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = new ObservableCollection<SpecialDropListItem<IJournalAccount>>();
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts
        { get { return _listValidCredits; } }

        public abstract string CreditHeader { get; }

        protected IJournalAccount _credit;

        public IJournalAccount? SelectedCreditAccount
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

        public virtual void SaveTransaction()
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