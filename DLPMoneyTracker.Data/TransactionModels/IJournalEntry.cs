using DLPMoneyTracker.Data.LedgerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Data.TransactionModels
{
    public interface IJournalEntry
    {
        Guid Id { get; }
        DateTime TransactionDate { get; }
        Guid DebitAccountId { get; } 
        Guid CreditAccountId { get; }
        string Description { get; }
        decimal TransactionAmount { get; }
    }

    public class JournalEntryJSON : IJournalEntry
    {

        public Guid Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public Guid DebitAccountId { get; set; } 

        public Guid CreditAccountId { get; set; }

        public string Description { get; set; }

        public decimal TransactionAmount { get; set; }
    }

    public class JournalEntry : IJournalEntry
    {
        public JournalEntry()
        {
            this.Id = Guid.NewGuid();
            this.TransactionDate = DateTime.Now;
            this.Description = string.Empty;
            this.TransactionAmount = decimal.Zero;
        }

        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }

        public ILedgerAccount DebitAccount { get; set; }
        public Guid DebitAccountId { get { return DebitAccount.Id; } }
        public ILedgerAccount CreditAccount { get; set; }
        public Guid CreditAccountId { get { return CreditAccount.Id; } }

        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }


    }
}
