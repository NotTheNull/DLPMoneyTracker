using DLPMoneyTracker.Core.Models.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core.Models
{
    public enum TransactionType
    {
        NotSet,
        Expense,
        Income,
        DebtPayment,
        DebtAdjustment,
        Correction,
        Transfer
    }
    public interface IMoneyTransaction
    {
        Guid UID { get; }
        DateTime TransactionDate { get; }
        TransactionType JournalEntryType { get; }

        // For now, I'll keep the one to one relationship going; I may change this once I add subledger Funds accounts
        SingleAccountTransaction[] Records { get; }

        IJournalAccount DebitAccount { get; }
        Guid DebitAccountId { get; }
        string DebitAccountName { get; }
        DateTime? DebitBankDate { get; }

        IJournalAccount CreditAccount { get; }
        Guid CreditAccountId { get; }
        string CreditAccountName { get; }
        DateTime? CreditBankDate { get; }

        string Description { get; }
        decimal TransactionAmount { get; }

        void Copy(IMoneyTransaction transaction);
    }

    public class MoneyTransaction : IMoneyTransaction
    {
        public MoneyTransaction()
        {

        }
        public MoneyTransaction(IMoneyTransaction transaction)
        {
            this.Copy(transaction);
        }

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



        private SingleAccountTransaction[] _records = { new SingleAccountTransaction(), new SingleAccountTransaction() };
        public SingleAccountTransaction[] Records { get { return _records; } }

        public IJournalAccount DebitAccount { get { return _records[0].Account; } set { _records[0].Account = value; } }
        public Guid DebitAccountId { get { return DebitAccount?.Id ?? Guid.Empty; } }
        public string DebitAccountName { get { return DebitAccount?.Description ?? string.Empty; } }
        public DateTime? DebitBankDate { get { return _records[0].BankDate; } set { _records[0].BankDate = value; } }


        public IJournalAccount CreditAccount { get { return _records[1].Account; } set { _records[1].Account = value; } }
        public Guid CreditAccountId { get { return CreditAccount?.Id ?? Guid.Empty; } }
        public string CreditAccountName { get { return CreditAccount?.Description ?? string.Empty; } }
        public DateTime? CreditBankDate { get { return _records[1].BankDate; } set { _records[1].BankDate = value; } }


        public void Copy(IMoneyTransaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            this.UID = transaction.UID;
            this.TransactionDate = transaction.TransactionDate;
            this.JournalEntryType = transaction.JournalEntryType;
            this.Description = transaction.Description;
            this.TransactionAmount = transaction.TransactionAmount;
            this.DebitAccount = transaction.DebitAccount;
            this.DebitBankDate = transaction.DebitBankDate;
            this.CreditAccount = transaction.CreditAccount;
            this.CreditBankDate = transaction.CreditBankDate;
        }
    }

    public class SingleAccountTransaction
    {
        public Guid TransactionUID { get; set; } = Guid.NewGuid();
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;
        public string Description { get; set; } = string.Empty;

        public IJournalAccount Account { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime? BankDate { get; set; }

    }
}
