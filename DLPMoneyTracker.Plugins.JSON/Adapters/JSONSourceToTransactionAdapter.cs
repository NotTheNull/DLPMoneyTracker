using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;
using DLPMoneyTracker.Core.Models.LedgerAccounts;
using DLPMoneyTracker.Plugins.JSON.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DLPMoneyTracker.Plugins.JSON.Adapters
{
    internal class JSONSourceToTransactionAdapter : ISourceToTransactionAdapter<JournalEntryJSON>
    {
        private readonly ILedgerAccountRepository accountRepository;

        public JSONSourceToTransactionAdapter(ILedgerAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }


        private SingleAccountTransaction[] _records = { new SingleAccountTransaction(), new SingleAccountTransaction() };
        public SingleAccountTransaction[] Records { get { return _records; } }


        public Guid UID
        {
            get { return _records[0].TransactionUID; }
            set
            {
                _records[0].TransactionUID = value;
                _records[1].TransactionUID = value;
            }
        }
        public DateTime TransactionDate
        {
            get { return _records[0].TransactionDate; }
            set
            {
                _records[0].TransactionDate = value;
                _records[1].TransactionDate = value;
            }
        }
        public TransactionType JournalEntryType
        {
            get { return _records[0].JournalEntryType; }
            set
            {
                _records[0].JournalEntryType = value;
                _records[1].JournalEntryType = value;
            }
        }
        public string Description
        {
            get { return _records[0].Description; }
            set
            {
                _records[0].Description = value;
                _records[1].Description = value;
            }
        }
        public decimal TransactionAmount
        {
            get { return _records[0].TransactionAmount; }
            set
            {
                _records[0].TransactionAmount = value;
                _records[1].TransactionAmount = value * -1;
            }
        }


        public IJournalAccount DebitAccount { get { return _records[0].Account; } set { _records[0].Account = value; } }
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }
        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }
        public DateTime? DebitBankDate { get { return _records[0].BankDate; } set { _records[0].BankDate = value; } }


        public IJournalAccount CreditAccount { get { return _records[1].Account; } set { _records[1].Account = value; } }
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }
        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }
        public DateTime? CreditBankDate { get { return _records[1].BankDate; } set { _records[1].BankDate = value; } }


        public void ExportSource(ref JournalEntryJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            acct.Id = this.UID;
            acct.TransactionDate = this.TransactionDate;
            acct.JournalEntryType = this.JournalEntryType;
            acct.Description = this.Description;
            acct.TransactionAmount = this.TransactionAmount;
            acct.DebitAccountId = this.DebitAccountId;
            acct.DebitBankDate = this.DebitBankDate;
            acct.CreditAccountId = this.CreditAccountId;
            acct.CreditBankDate = this.CreditBankDate;
        }

        public void Copy(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            this.UID = transaction.UID;
            this.TransactionDate = transaction.TransactionDate;
            this.JournalEntryType = transaction.JournalEntryType;
            this.Description = transaction.Description;
            this.TransactionAmount = transaction.TransactionAmount;
            this.DebitBankDate = transaction.DebitBankDate;
            this.CreditBankDate = transaction.CreditBankDate;

            this.DebitAccount = accountRepository.GetAccountByUID(transaction.DebitAccountId);
            this.CreditAccount = accountRepository.GetAccountByUID(transaction.CreditAccountId);
        }

        public void ImportSource(JournalEntryJSON acct)
        {
            ArgumentNullException.ThrowIfNull(acct);

            this.UID = acct.Id;
            this.TransactionDate = acct.TransactionDate;
            this.JournalEntryType = acct.JournalEntryType;
            this.Description = acct.Description;
            this.TransactionAmount = acct.TransactionAmount;
            this.DebitBankDate = acct.DebitBankDate;
            this.CreditBankDate = acct.CreditBankDate;

            if (acct.DebitAccountId == Guid.Empty)
            {
                this.DebitAccount = SpecialAccount.InitialBalance;
                this.DebitBankDate = Common.MINIMUM_DATE;
            }
            else
            {
                this.DebitAccount = accountRepository.GetAccountByUID(acct.DebitAccountId);
            }

            if (acct.CreditAccountId == Guid.Empty)
            {
                this.CreditAccount = SpecialAccount.InitialBalance;
                this.CreditBankDate = Common.MINIMUM_DATE;
            }
            else
            {
                this.CreditAccount = accountRepository.GetAccountByUID(acct.CreditAccountId);
            }
        }
    }
}
