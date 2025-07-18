﻿using DLPMoneyTracker.Core.Models.LedgerAccounts;

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
        { }

        public MoneyTransaction(IMoneyTransaction transaction)
        {
            this.Copy(transaction);
        }

        public Guid UID { get; set; } = Guid.NewGuid();
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;
        public string Description { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; } = decimal.Zero;

        public IJournalAccount DebitAccount { get; set; } = SpecialAccount.InvalidAccount;
        public Guid DebitAccountId => DebitAccount.Id;
        public string DebitAccountName => DebitAccount.Description;
        public DateTime? DebitBankDate { get; set; }

        public IJournalAccount CreditAccount { get; set; } = SpecialAccount.InvalidAccount;
        public Guid CreditAccountId => CreditAccount.Id;
        public string CreditAccountName => CreditAccount.Description;
        public DateTime? CreditBankDate { get; set; }

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
}