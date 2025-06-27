
using DLPMoneyTracker.BusinessLogic.UseCases.JournalAccounts.Interfaces;
using DLPMoneyTracker.BusinessLogic.UseCases.Transactions.Interfaces;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.BudgetPlan;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
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

        string Title { get; }
        string Description { get; }
        decimal Amount { get; }

        void Clear();

        void LoadAccounts();
        void LoadTransaction(IMoneyTransaction entry);

        void SaveTransaction();

        void FillFromPlan(IBudgetPlan plan);
    }

    public abstract class BaseRecordJournalEntryVM : BaseViewModel, IJournalEntryVM
    {

        private readonly List<LedgerType> _validDebitTypes = [];
        private readonly List<LedgerType> _validCreditTypes = [];
        protected readonly IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase;
        protected readonly IGetJournalAccountByUIDUseCase getAccountByUIDUseCase;
        protected readonly ISaveTransactionUseCase saveMoneyRecordUseCase;

        protected BaseRecordJournalEntryVM(
            IGetJournalAccountListByTypesUseCase getAccountsByTypeUseCase,
            IGetJournalAccountByUIDUseCase getAccountByUIDUseCase,
            ISaveTransactionUseCase saveMoneyRecordUseCase,
            IEnumerable<LedgerType> validDebitTypes,
            IEnumerable<LedgerType> validCreditTypes,
            TransactionType transType)
        {
            this.getAccountsByTypeUseCase = getAccountsByTypeUseCase;
            this.getAccountByUIDUseCase = getAccountByUIDUseCase;
            this.saveMoneyRecordUseCase = saveMoneyRecordUseCase;
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

        private readonly TransactionType _transType;
        public TransactionType JournalEntryType { get { return _transType; } }


        public bool IsNew { get { return !this.ExistingTransactionId.HasValue; } }
        public abstract bool IsValidTransaction { get; }

        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidDebits = [];
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidDebitAccounts => _listValidDebits;

        public abstract string DebitHeader { get; }

        protected IJournalAccount? _debit;
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
        public virtual bool IsDebitBankDateVisible => !IsNew && (_debit is IMoneyAccount);


        public bool CanUserEditCreditAccount
        {
            get
            {
                if (this.IsNew) return true;
                if (SelectedCreditAccount != null)
                    return !SelectedCreditAccount.DateClosedUTC.HasValue;

                return true;
            }
        }
        public bool CanUserEditDebitAccount
        {
            get
            {
                if (this.IsNew) return true;
                if (SelectedDebitAccount != null)
                    return !SelectedDebitAccount.DateClosedUTC.HasValue;

                return true;
            }
        }




        public virtual bool IsCreditEnabled { get { return CanUserEditCreditAccount; } }

        public virtual bool IsCreditBankDateVisible
        {
            get
            {
                return !IsNew && IsCreditEnabled && (_credit is IMoneyAccount);
            }
        }


        protected ObservableCollection<SpecialDropListItem<IJournalAccount>> _listValidCredits = [];
        public ObservableCollection<SpecialDropListItem<IJournalAccount>> ValidCreditAccounts { get { return _listValidCredits; } }

        public abstract string CreditHeader { get; }

        protected IJournalAccount? _credit;
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



        public abstract string Title { get; }

        protected string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
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
            var listCredits = getAccountsByTypeUseCase.Execute(_validCreditTypes);
            if (listCredits?.Any() == true)
            {
                foreach (var c in listCredits.OrderBy(o => o.Description))
                {
                    this.ValidCreditAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(c.Description, c));
                }
            }

            this.ValidDebitAccounts.Clear();
            var listDebits = getAccountsByTypeUseCase.Execute(_validDebitTypes);
            if (listDebits?.Any() == true)
            {
                foreach (var d in listDebits.OrderBy(o => o.Description))
                {
                    this.ValidDebitAccounts.Add(new Core.SpecialDropListItem<IJournalAccount>(d.Description, d));
                }
            }
        }

        public virtual void LoadTransaction(IMoneyTransaction entry)
        {
            if (entry is null) throw new ArgumentNullException(nameof(IMoneyTransaction));

            this.ExistingTransactionId = entry.UID;
            this.TransactionDate = entry.TransactionDate;
            this.Amount = entry.TransactionAmount;
            this.Description = entry.Description;
            this.SelectedCreditAccount = entry.CreditAccount;
            this.SelectedDebitAccount = entry.DebitAccount;
            this.CreditBankDate = entry.CreditBankDate;
            this.DebitBankDate = entry.DebitBankDate;
        }

        /// <summary>
        /// If you override this save, make sure to check for the Existing Transaction
        /// </summary>
        public virtual void SaveTransaction()
        {
            if (this.SelectedCreditAccount is null || this.SelectedDebitAccount is null) return;
            if (!IsValidTransaction) return;

            MoneyTransaction record = new()
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

            if (this.ExistingTransactionId.HasValue)
            {
                record.UID = this.ExistingTransactionId.Value;
            }

            saveMoneyRecordUseCase.Execute(record);
        }

        public void FillFromPlan(IBudgetPlan plan)
        {
            this.TransactionDate = DateTime.Today;
            this.SelectedCreditAccount = getAccountByUIDUseCase.Execute(plan.CreditAccountId);
            this.SelectedDebitAccount = getAccountByUIDUseCase.Execute(plan.DebitAccountId);
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