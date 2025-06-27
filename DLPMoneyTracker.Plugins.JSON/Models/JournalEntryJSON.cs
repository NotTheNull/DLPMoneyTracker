using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.Plugins.JSON.Models
{
    internal sealed class JournalEntryJSON
    {
        public Guid Id { get; set; }

        public DateTime TransactionDate { get; set; }
        public TransactionType JournalEntryType { get; set; } = TransactionType.NotSet;

        public Guid DebitAccountId { get; set; }
        public DateTime? DebitBankDate { get; set; }

        public Guid CreditAccountId { get; set; }
        public DateTime? CreditBankDate { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
    }
}